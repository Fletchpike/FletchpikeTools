using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System;

namespace Fletchpike.Rendering
{
    [Serializable]
    [PostProcess(typeof(EdgeDetectRenderer), PostProcessEvent.AfterStack, "Fletchpike/Edge Detect")]
    public sealed class EdgeDetect : PostProcessEffectSettings
    {
        [Tooltip("The Color Of The Edges.")]
        public ColorParameter edgeColor = new ColorParameter { value = Color.black };
        public FloatParameter outlineThickness = new FloatParameter { value = 1f };
        public FloatParameter sensitivity = new FloatParameter { value = 200f };
    }
    public sealed class EdgeDetectRenderer : PostProcessEffectRenderer<EdgeDetect>
    {
        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(Shader.Find("Hidden/Fletchpike/EdgeDetect"));
            sheet.properties.SetColor("_EdgeColor", settings.edgeColor);
            sheet.properties.SetFloat("_OutlineThickness", settings.outlineThickness);
            sheet.properties.SetFloat("_Sensitivity", settings.sensitivity);
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}
