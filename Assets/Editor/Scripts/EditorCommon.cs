using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using System.IO;

namespace Fletchpike.Editor
{
    [CustomPropertyDrawer(typeof(SingleRange))]
    public class SingleRangePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.Space(-20);
            var min = property.FindPropertyRelative("_min");
            var max = property.FindPropertyRelative("_max");
            var lmin = property.FindPropertyRelative("_limitMin");
            var lmax = property.FindPropertyRelative("_limitMax");
            var miv = min.floatValue;
            var mav = max.floatValue;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.MinMaxSlider(property.displayName, ref miv, ref mav, lmin.floatValue, lmax.floatValue);
            miv = EditorGUILayout.FloatField(miv, GUILayout.Width(50));
            mav = EditorGUILayout.FloatField(mav, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
            min.floatValue = miv;
            max.floatValue = mav;
        }
    }
    [CustomPropertyDrawer(typeof(IntegerRange))]
    public class IntegerRangePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.Space(-20);
            var min = property.FindPropertyRelative("_min");
            var max = property.FindPropertyRelative("_max");
            var lmin = property.FindPropertyRelative("_limitMin");
            var lmax = property.FindPropertyRelative("_limitMax");
            float miv = min.intValue;
            float mav = max.intValue;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.MinMaxSlider(property.displayName, ref miv, ref mav, lmin.intValue, lmax.intValue);
            miv = EditorGUILayout.IntField(Mathf.RoundToInt(miv), GUILayout.Width(50));
            mav = EditorGUILayout.IntField(Mathf.RoundToInt(mav), GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
            min.intValue = Mathf.RoundToInt(miv);
            max.intValue = Mathf.RoundToInt(mav);
        }
    }
    [CustomPropertyDrawer(typeof(SingleSlider))]
    public class SingleSliderPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.Space(-20);
            var min = property.FindPropertyRelative("_min");
            var max = property.FindPropertyRelative("_max");
            var value = property.FindPropertyRelative("_value");
            EditorGUILayout.BeginHorizontal();
            value.floatValue = EditorGUILayout.Slider(property.displayName, value.floatValue, min.floatValue, max.floatValue);
            //value.floatValue = EditorGUILayout.FloatField(value.floatValue, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
        }
    }
    [CustomPropertyDrawer(typeof(IntegerSlider))]
    public class IntegerSliderPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.Space(-20);
            var min = property.FindPropertyRelative("_min");
            var max = property.FindPropertyRelative("_max");
            var value = property.FindPropertyRelative("_value");
            EditorGUILayout.BeginHorizontal();
            value.intValue = EditorGUILayout.IntSlider(property.displayName, value.intValue, min.intValue, max.intValue);
            //value.intValue = EditorGUILayout.IntField(value.intValue, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
        }
    }
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AudioContainer))]
    public class AudioContainerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var self = serializedObject.targetObject as AudioContainer;
            var clips = serializedObject.FindProperty("clips");
            var volumeRange = serializedObject.FindProperty("volumeRange");
            var pitchRange = serializedObject.FindProperty("pitchRange");
            var selectOrder = serializedObject.FindProperty("selectOrder");
            var avoidRepeatingLast = serializedObject.FindProperty("avoidRepeatingLast");
            EditorGUILayout.PropertyField(clips);
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(volumeRange);
            EditorGUILayout.PropertyField(pitchRange);
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(selectOrder);
            if (selectOrder.enumValueIndex == (int)AudioContainer.ClipSelection.Random)
            {
                if (clips.arraySize < 1)
                {
                    avoidRepeatingLast.intValue = 0;
                }
                else
                {
                    avoidRepeatingLast.intValue = Mathf.Clamp(avoidRepeatingLast.intValue, 0, clips.arraySize - 1);
                }
                EditorGUILayout.PropertyField(avoidRepeatingLast);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
    namespace Debug
    { 
        public static class Debug
        {
            [MenuItem("Tools/Debug/View Class")]
            public static void ViewClass()
            {
                var t = Selection.activeObject.GetType();
                var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                foreach (var ass in assemblies)
                {
                    foreach (var item in ass.GetTypes())
                    {
                        if (item != t && item.IsAssignableFrom(t))
                        {
                            Log(item.FullName);
                        }
                    }
                }
            }
            public static void Log(object message) => UnityEngine.Debug.Log(message);
        }
    }
}
