using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;
using System.Collections.Generic;

namespace Fletchpike.Editor
{
    [CustomPropertyDrawer(typeof(SingleRange))]
    public class SingleRangePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Old Layout
            /**
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
             **/
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var min = property.FindPropertyRelative("_min");
            var max = property.FindPropertyRelative("_max");
            var lmin = property.FindPropertyRelative("_limitMin");
            var lmax = property.FindPropertyRelative("_limitMax");
            var miv = min.floatValue;
            var mav = max.floatValue;
            Rect minMaxRect = new Rect(position.x + (position.width * 0.16f), position.y, position.width * 0.68f, position.height);
            Rect minRect = new Rect(position.x, position.y, position.width * 0.15f, position.height);
            Rect maxRect = new Rect(position.x + (position.width * 0.85f), position.y, position.width * 0.15f, position.height);
            EditorGUI.MinMaxSlider(minMaxRect, ref miv, ref mav, lmin.floatValue, lmax.floatValue);
            miv = EditorGUI.FloatField(minRect, miv);
            mav = EditorGUI.FloatField(maxRect, mav);
            min.floatValue = miv;
            max.floatValue = mav;
            EditorGUI.EndProperty();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
    [CustomPropertyDrawer(typeof(IntegerRange))]
    public class IntegerRangePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Old Layout
            /**
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
            **/
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var min = property.FindPropertyRelative("_min");
            var max = property.FindPropertyRelative("_max");
            var lmin = property.FindPropertyRelative("_limitMin");
            var lmax = property.FindPropertyRelative("_limitMax");
            var miv = (float)min.intValue;
            var mav = (float)max.intValue;
            Rect minMaxRect = new Rect(position.x + (position.width * 0.16f), position.y, position.width * 0.68f, position.height);
            Rect minRect = new Rect(position.x, position.y, position.width * 0.15f, position.height);
            Rect maxRect = new Rect(position.x + (position.width * 0.85f), position.y, position.width * 0.15f, position.height);
            EditorGUI.MinMaxSlider(minMaxRect, ref miv, ref mav, lmin.intValue, lmax.intValue);
            miv = EditorGUI.IntField(minRect, (int)miv);
            mav = EditorGUI.IntField(maxRect, (int)mav);
            min.intValue = (int)miv;
            max.intValue = (int)mav;
            EditorGUI.EndProperty();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
    [CustomPropertyDrawer(typeof(SingleSlider))]
    public class SingleSliderPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            /**
            EditorGUILayout.Space(-20);
            var min = property.FindPropertyRelative("_min");
            var max = property.FindPropertyRelative("_max");
            var value = property.FindPropertyRelative("_value");
            EditorGUILayout.BeginHorizontal();
            value.floatValue = EditorGUILayout.Slider(property.displayName, value.floatValue, min.floatValue, max.floatValue);
            EditorGUILayout.EndHorizontal();
            **/
            EditorGUI.BeginProperty(position, label, property);
            var min = property.FindPropertyRelative("_min");
            var max = property.FindPropertyRelative("_max");
            var value = property.FindPropertyRelative("_value");
            Rect sliderRect = new Rect(position.x, position.y, position.width, position.height);
            value.floatValue = EditorGUI.Slider(sliderRect, label, value.floatValue, min.floatValue, max.floatValue);
            EditorGUI.EndProperty();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
    [CustomPropertyDrawer(typeof(IntegerSlider))]
    public class IntegerSliderPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Old Layout
            /**
            EditorGUILayout.Space(-20);
            var min = property.FindPropertyRelative("_min");
            var max = property.FindPropertyRelative("_max");
            var value = property.FindPropertyRelative("_value");
            EditorGUILayout.BeginHorizontal();
            value.intValue = EditorGUILayout.IntSlider(property.displayName, value.intValue, min.intValue, max.intValue);
            EditorGUILayout.EndHorizontal();
            **/
            EditorGUI.BeginProperty(position, label, property);
            var min = property.FindPropertyRelative("_min");
            var max = property.FindPropertyRelative("_max");
            var value = property.FindPropertyRelative("_value");
            Rect sliderRect = new Rect(position.x, position.y, position.width, position.height);
            value.intValue = EditorGUI.IntSlider(sliderRect, label, value.intValue, min.intValue, max.intValue);
            EditorGUI.EndProperty();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
    [CustomPropertyDrawer(typeof(AudioContainerClip))]
    public class AudioContainerClipPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = new(position.position, new(position.width, EditorGUIUtility.singleLineHeight));
            var enabled = property.FindPropertyRelative("_enabled");
            var clip = property.FindPropertyRelative("_clip");
            Rect clipRect = new Rect(position.x + (position.height), position.y, position.width - position.height, position.height);
            Rect enabledRect = new Rect(position.x, position.y, position.height, position.height);
            enabled.boolValue = EditorGUI.Toggle(enabledRect, enabled.boolValue);
            clip.objectReferenceValue = EditorGUI.ObjectField(clipRect, clip.objectReferenceValue, typeof(AudioClip), false);
            EditorGUI.EndProperty();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
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
            var clips = serializedObject.FindProperty("_clips");
            var volumeRange = serializedObject.FindProperty("_volumeRange");
            var pitchRange = serializedObject.FindProperty("_pitchRange");
            var prefVR = serializedObject.FindProperty("useVolumeRandom");
            var prefPR = serializedObject.FindProperty("usePitchRandom");
            var selectOrder = serializedObject.FindProperty("_selectOrder");
            var avoidRepeatingLast = serializedObject.FindProperty("_avoidRepeatingLast");
            EditorGUILayout.PropertyField(clips, true);
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            prefVR.boolValue = EditorGUILayout.Toggle(prefVR.boolValue, GUILayout.Width(16));
            if (prefVR.boolValue)
            {
                EditorGUILayout.PropertyField(volumeRange);
            }
            else
            {
                var vol = EditorGUILayout.Slider("Volume", volumeRange.FindPropertyRelative("_max").floatValue, 0, 1);
                volumeRange.FindPropertyRelative("_min").floatValue = vol;
                volumeRange.FindPropertyRelative("_max").floatValue = vol;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            prefPR.boolValue = EditorGUILayout.Toggle(prefPR.boolValue, GUILayout.Width(16));
            if (prefPR.boolValue)
            {
                EditorGUILayout.PropertyField(pitchRange);
            }
            else
            {
                var pit = EditorGUILayout.Slider("Pitch", pitchRange.FindPropertyRelative("_max").floatValue, -3, 3);
                pitchRange.FindPropertyRelative("_min").floatValue = pit;
                pitchRange.FindPropertyRelative("_max").floatValue = pit;
            }
            EditorGUILayout.EndHorizontal();
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
            EditorGUILayout.Separator();
            if (GUILayout.Button("Preview"))
            {
                var source = new GameObject("PreviewAudioContainer").AddComponent<AudioSource>();
                source.gameObject.hideFlags = HideFlags.HideAndDontSave;
                source.playOnAwake = false;
                self.ApplyProperties(source);
                if (source.clip != null)
                {
                    source.Play();
                    var del = source.gameObject.AddComponent<DeletionMark>();
                    del.markType = DeletionMarkType.AudioSource;
                    del.waitTime = source.clip.length;
                }
                else
                {
                    Object.Destroy(source.gameObject);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
        [MenuItem("Assets/Create/Audio/Convert Audio Random Container")]
        public static void CreateAudioContainerFromARC()
        {
            var arcs = new List<AudioResource>(Selection.GetFiltered<AudioResource>(SelectionMode.Assets));
            arcs.RemoveAll((item) => item.GetType().FullName != "UnityEngine.Audio.AudioRandomContainer");
            if (arcs.Count > 0)
            {
                var inst = CreateInstance<AudioContainer>();
                var con = new AudioRandomContainerAccess(arcs[0]);
                con.CreateAudioContainer(inst);
                string path = "Assets/Converted Container.asset";
                path = AssetDatabase.GenerateUniqueAssetPath(path);
                AssetDatabase.CreateAsset(inst, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = inst;
            }
            else
            {
                Debug.Debug.LogWarning("No AudioRandomContainer Selected!");
            }
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
            public static void Log(object message) => Logging.Log(message);
            public static void LogWarning(object message) => Logging.LogWarning(message);
            public static void LogError(object message) => Logging.LogError(message);
        }
    }
}
