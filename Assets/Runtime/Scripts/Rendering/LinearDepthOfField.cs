using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Fletchpike.Rendering
{

    /// <summary>
    /// A volume parameter holding a <see cref="KernelSize"/> value.
    /// </summary>
    [Serializable]
    public sealed class KernelSizeParameter : ParameterOverride<KernelSize> { }

    /// <summary>
    /// This class holds settings for the Depth of Field effect.
    /// </summary>
    [Serializable]
    [PostProcess(typeof(LinearDepthOfFieldRenderer), PostProcessEvent.AfterStack, "Fletchpike/Linear Depth of Field")]
    public sealed class LinearDepthOfField : PostProcessEffectSettings
    {
        [Tooltip("Start Blur Distance")]
        public FloatParameter minDistance = new FloatParameter { value = 5f };
        [Tooltip("Max Blur Distance")]
        public FloatParameter maxDistance = new FloatParameter { value = 20f };
        [DisplayName("Max Blur Size"), Tooltip("The Maximum Blur Size"), MinMax(0, 100)]
        public FloatParameter blurSize = new FloatParameter { value = 10f };
    }

    [UnityEngine.Scripting.Preserve]
    // TODO: Doesn't play nice with alpha propagation, see if it can be fixed without killing performances
    public class LinearDepthOfFieldRenderer : PostProcessEffectRenderer<LinearDepthOfField>
    {
        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(Shader.Find("Hidden/Fletchpike/LinearDepthOfField"));
            sheet.properties.SetFloat("_BlurSize", settings.blurSize);
            sheet.properties.SetFloat("_MinDistance", settings.minDistance);
            sheet.properties.SetFloat("_MaxDistance", settings.maxDistance);
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}
