using UnityEngine;
using UnityEditor;

namespace Fletchpike.Editor
{
    [CustomEditor(typeof(FrostedGlassBuffer))]
    [CanEditMultipleObjects]
    public class FrostedGlassBufferEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
