using System;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using static UnityEngine.Rendering.PostProcessing.PostProcessResources;
using Min = UnityEngine.Rendering.PostProcessing.MinAttribute;
using Max = UnityEngine.Rendering.PostProcessing.MaxAttribute;

namespace Fletchpike.Rendering.PostProcessing
{

    /// <summary>
    /// This class holds settings for the Depth of Field effect.
    /// </summary>
    [Serializable]
    [PostProcess(typeof(LinearDepthOfFieldRenderer), PostProcessEvent.AfterStack, "Fletchpike/Linear Depth of Field", false)]
    public sealed class LinearDepthOfField : PostProcessEffectSettings
    {
        [Tooltip("Minimum Distance For The Blurring")]
        public FloatParameter minDistance = new() { value = 10f };

        [Tooltip("Maximum Distance For The Blurring")]
        public FloatParameter maxDistance = new() { value = 30f };

        [DisplayName("Max Blur Size"), Tooltip("Convolution kernel size of the bokeh filter, which determines the maximum radius of bokeh. It also affects performances (the larger the kernel is, the longer the GPU time is required).")]
        public KernelSizeParameter kernelSize = new KernelSizeParameter { value = KernelSize.Medium };

        /// <summary>
        /// Returns <c>true</c> if the effect is currently enabled and supported.
        /// </summary>
        /// <param name="context">The current post-processing render context</param>
        /// <returns><c>true</c> if the effect is currently enabled and supported</returns>
        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            return enabled.value
                && SystemInfo.graphicsShaderLevel >= 35;
        }
    }

    [UnityEngine.Scripting.Preserve]
    // TODO: Doesn't play nice with alpha propagation, see if it can be fixed without killing performances
    internal sealed class LinearDepthOfFieldRenderer : PostProcessEffectRenderer<LinearDepthOfField>
    {
        private static Shader _ldof;
        public static Shader linearDepthOfField
        {
            get
            {
                if (_ldof == null) _ldof = Shader.Find("Hidden/Fletchpike/LinearDepthOfField");
                return _ldof;
            }
        }
        enum Pass
        {
            CoCCalculation,
            CoCTemporalFilter,
            DownsampleAndPrefilter,
            BokehSmallKernel,
            BokehMediumKernel,
            BokehLargeKernel,
            BokehVeryLargeKernel,
            PostFilter,
            Combine,
            DebugOverlay
        }

        // Ping-pong between two history textures as we can't read & write the same target in the
        // same pass
        const int k_NumEyes = 2;
        const int k_NumCoCHistoryTextures = 2;
        readonly RenderTexture[][] m_CoCHistoryTextures = new RenderTexture[k_NumEyes][];
        int[] m_HistoryPingPong = new int[k_NumEyes];

        // Height of the 35mm full-frame format (36mm x 24mm)
        // TODO: Should be set by a physical camera
        const float k_FilmHeight = 0.024f;

        public LinearDepthOfFieldRenderer()
        {
            for (int eye = 0; eye < k_NumEyes; eye++)
            {
                m_CoCHistoryTextures[eye] = new RenderTexture[k_NumCoCHistoryTextures];
                m_HistoryPingPong[eye] = 0;
            }
        }

        public override DepthTextureMode GetCameraFlags()
        {
            return DepthTextureMode.Depth;
        }

        RenderTextureFormat SelectFormat(RenderTextureFormat primary, RenderTextureFormat secondary)
        {
            if (primary.IsSupported())
                return primary;

            if (secondary.IsSupported())
                return secondary;

            return RenderTextureFormat.Default;
        }

        float CalculateMaxCoCRadius(int screenHeight)
        {
            // Estimate the allowable maximum radius of CoC from the kernel
            // size (the equation below was empirically derived).
            float radiusInPixels = (float)settings.kernelSize.value * 4f + 6f;

            // Applying a 5% limit to the CoC radius to keep the size of
            // TileMax/NeighborMax small enough.
            return Mathf.Min(0.05f, radiusInPixels / screenHeight);
        }

        RenderTexture CheckHistory(int eye, int id, PostProcessRenderContext context, RenderTextureFormat format)
        {
            var rt = m_CoCHistoryTextures[eye][id];

            if (m_ResetHistory || rt == null || !rt.IsCreated() || rt.width != context.width || rt.height != context.height)
            {
                RenderTexture.ReleaseTemporary(rt);

                rt = context.GetScreenSpaceTemporaryRT(0, format, RenderTextureReadWrite.Linear);
                rt.name = "CoC History, Eye: " + eye + ", ID: " + id;
                rt.filterMode = FilterMode.Bilinear;
                rt.Create();
                m_CoCHistoryTextures[eye][id] = rt;
            }

            return rt;
        }

        internal static readonly int DepthOfFieldTemp = Shader.PropertyToID("_DepthOfFieldTemp");
        internal static readonly int DepthOfFieldTex = Shader.PropertyToID("_DepthOfFieldTex");
        internal static readonly int Distance = Shader.PropertyToID("_Distance");
        internal static readonly int LensCoeff = Shader.PropertyToID("_LensCoeff");
        internal static readonly int MaxCoC = Shader.PropertyToID("_MaxCoC");
        internal static readonly int RcpMaxCoC = Shader.PropertyToID("_RcpMaxCoC");
        internal static readonly int RcpAspect = Shader.PropertyToID("_RcpAspect");
        internal static readonly int CoCTex = Shader.PropertyToID("_CoCTex");
        internal static readonly int TaaParams = Shader.PropertyToID("_TaaParams");
        internal static readonly int MinDistance = Shader.PropertyToID("_MinDistance");
        internal static readonly int MaxDistance = Shader.PropertyToID("_MaxDistance");
        public const float focalDistance = 10f;
        public const float focalLength = 50f;
        public const float aperture = 5.6f;

        public override void Render(PostProcessRenderContext context)
        {
            // The coc is stored in alpha so we need a 4 channels target. Note that using ARGB32
            // will result in a very weak near-blur.
            var colorFormat = context.camera.allowHDR ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
            var cocFormat = SelectFormat(RenderTextureFormat.R8, RenderTextureFormat.RHalf);

            // Material setup
            float scaledFilmHeight = k_FilmHeight * (context.height / 1080f);
            var f = focalLength / 1000f;
            var s1 = Mathf.Max(focalDistance, f);
            var aspect = (float)context.screenWidth / (float)context.screenHeight;
            var coeff = f * f / (aperture * (s1 - f) * scaledFilmHeight * 2f);
            var maxCoC = CalculateMaxCoCRadius(context.screenHeight);

            var sheet = context.propertySheets.Get(linearDepthOfField);
            sheet.properties.Clear();
            sheet.properties.SetFloat(Distance, s1);
            sheet.properties.SetFloat(LensCoeff, coeff);
            sheet.properties.SetFloat(MaxCoC, maxCoC);
            sheet.properties.SetFloat(RcpMaxCoC, 1f / maxCoC);
            sheet.properties.SetFloat(RcpAspect, 1f / aspect);
            sheet.properties.SetFloat(MinDistance, settings.minDistance);
            sheet.properties.SetFloat(MaxDistance, settings.maxDistance);

            var cmd = context.command;
            cmd.BeginSample("LinearDepthOfField");

            // CoC calculation pass
            context.GetScreenSpaceTemporaryRT(cmd, CoCTex, 0, cocFormat, RenderTextureReadWrite.Linear);
            cmd.BlitFullscreenTriangle(BuiltinRenderTextureType.None, CoCTex, sheet, (int)Pass.CoCCalculation);

            // CoC temporal filter pass when TAA is enabled
            if (context.IsTemporalAntialiasingActive())
            {
                float motionBlending = context.temporalAntialiasing.motionBlending;
                float blend = m_ResetHistory ? 0f : motionBlending; // Handles first frame blending
                var jitter = context.temporalAntialiasing.jitter;

                sheet.properties.SetVector(TaaParams, new Vector3(jitter.x, jitter.y, blend));

                int pp = m_HistoryPingPong[context.xrActiveEye];
                var historyRead = CheckHistory(context.xrActiveEye, ++pp % 2, context, cocFormat);
                var historyWrite = CheckHistory(context.xrActiveEye, ++pp % 2, context, cocFormat);
                m_HistoryPingPong[context.xrActiveEye] = ++pp % 2;

                cmd.BlitFullscreenTriangle(historyRead, historyWrite, sheet, (int)Pass.CoCTemporalFilter);
                cmd.ReleaseTemporaryRT(CoCTex);
                cmd.SetGlobalTexture(CoCTex, historyWrite);
            }

            // Downsampling and prefiltering pass
            context.GetScreenSpaceTemporaryRT(cmd, DepthOfFieldTex, 0, colorFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, context.width / 2, context.height / 2);
            cmd.BlitFullscreenTriangle(context.source, DepthOfFieldTex, sheet, (int)Pass.DownsampleAndPrefilter);

            // Bokeh simulation pass
            context.GetScreenSpaceTemporaryRT(cmd, DepthOfFieldTemp, 0, colorFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, context.width / 2, context.height / 2);
            cmd.BlitFullscreenTriangle(DepthOfFieldTex, DepthOfFieldTemp, sheet, (int)Pass.BokehSmallKernel + (int)settings.kernelSize.value);

            // Postfilter pass
            cmd.BlitFullscreenTriangle(DepthOfFieldTemp, DepthOfFieldTex, sheet, (int)Pass.PostFilter);
            cmd.ReleaseTemporaryRT(DepthOfFieldTemp);

            // Debug overlay pass
            if (context.IsDebugOverlayEnabled(DebugOverlay.DepthOfField))
                context.PushDebugOverlay(cmd, context.source, sheet, (int)Pass.DebugOverlay);

            // Combine pass
            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, (int)Pass.Combine);
            cmd.ReleaseTemporaryRT(DepthOfFieldTex);

            if (!context.IsTemporalAntialiasingActive())
                cmd.ReleaseTemporaryRT(CoCTex);

            cmd.EndSample("LinearDepthOfField");

            m_ResetHistory = false;
        }

        public override void Release()
        {
            for (int eye = 0; eye < k_NumEyes; eye++)
            {
                for (int i = 0; i < m_CoCHistoryTextures[eye].Length; i++)
                {
                    RenderTexture.ReleaseTemporary(m_CoCHistoryTextures[eye][i]);
                    m_CoCHistoryTextures[eye][i] = null;
                }
                m_HistoryPingPong[eye] = 0;
            }

            ResetHistory();
        }
    }
}
