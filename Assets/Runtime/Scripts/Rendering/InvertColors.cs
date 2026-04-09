using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System;

namespace Fletchpike.Rendering
{
    [Serializable]
    [PostProcess(typeof(InvertRenderer), PostProcessEvent.AfterStack, "Fletchpike/Invert Colors")]
    public sealed class InvertColors : PostProcessEffectSettings
    {
        [Range(0f, 1f), Tooltip("Invert effect intensity.")]
        public FloatParameter blend = new FloatParameter { value = 1f };
    }
    public sealed class InvertRenderer : PostProcessEffectRenderer<InvertColors>
    {
        public override void Render(PostProcessRenderContext context)
        {
            var sheet = context.propertySheets.Get(Shader.Find("Hidden/Fletchpike/InvertColors"));
            sheet.properties.SetFloat("_Blend", settings.blend);
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}
