using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using PRandom = System.Random;

namespace Fletchpike
{
    /// <summary>
    /// A basic class for a float range
    /// </summary>
    [Serializable]
    public class SingleRange
    {
        [SerializeField] private float _min;
        [SerializeField] private float _max;
        [SerializeField] private float _limitMin;
        [SerializeField] private float _limitMax;
        public float min { get => _min; set => _min = value; }
        public float max { get => _max; set => _max = value; }
        public float limitMin { get => _limitMin; set => _limitMin = value; }
        public float limitMax { get => _limitMax; set => _limitMax = value; }
        public SingleRange(float limitMin, float limitMax)
        {
            _limitMin = limitMin;
            _limitMax = limitMax;
            _min = limitMin;
            _max = limitMax;
        }
        public SingleRange(float limitMin, float limitMax, float min, float max)
        {
            _limitMin = limitMin;
            _limitMax = limitMax;
            _min = min;
            _max = max;
        }
        public SingleRange()
        {
            _limitMin = 0;
            _limitMax = 1;
            _min = 0;
            _max = 1;
        }
        public float random
        {
            get
            {
                return Random.Range(min, max);
            }
        }
    }
    /// <summary>
    /// A basic class for a integer range
    /// </summary>
    [Serializable]
    public class IntegerRange
    {
        [SerializeField] private int _min;
        [SerializeField] private int _max;
        [SerializeField] private int _limitMin;
        [SerializeField] private int _limitMax;
        public int min { get => _min; set => _min = value; }
        public int max { get => _max; set => _max = value; }
        public int limitMin { get => _limitMin; set => _limitMin = value; }
        public int limitMax { get => _limitMax; set => _limitMax = value; }
        public IntegerRange(int limitMin, int limitMax)
        {
            _limitMin = limitMin;
            _limitMax = limitMax;
            _min = limitMin;
            _max = limitMax;
        }
        public IntegerRange(int limitMin, int limitMax, int min, int max)
        {
            _limitMin = limitMin;
            _limitMax = limitMax;
            _min = min;
            _max = max;
        }
        public IntegerRange()
        {
            _limitMin = 0;
            _limitMax = 10;
            _min = 5;
            _max = 10;
        }
        public int random
        {
            get
            {
                return Random.Range(min, max);
            }
        }
        public float randomFloat
        {
            get
            {
                return Random.Range((float)min, (float)max);
            }
        }
    }
    [Serializable]
    public class SingleSlider
    {
        [SerializeField] private float _min;
        [SerializeField] private float _max;
        [SerializeField] private float _value;
        public float min { get => _min; set => _min = value; }
        public float max { get => _max; set => _max = value; }
        public float value { get => _value; set => _value = value; }
        public SingleSlider(float min, float max)
        {
            _min = min;
            _max = max;
            _value = max / 2f;
        }
        public SingleSlider(float min, float max, float value)
        {
            _min = min;
            _max = max;
            _value = value;
        }
        public static implicit operator float(SingleSlider slider)
        {
            return slider.value;
        }
    }
    [Serializable]
    public class IntegerSlider
    {
        [SerializeField] private int _min;
        [SerializeField] private int _max;
        [SerializeField] private int _value;
        public int min { get => _min; set => _min = value; }
        public int max { get => _max; set => _max = value; }
        public int value { get => _value; set => _value = value; }
        public IntegerSlider(int min, int max)
        {
            _min = min;
            _max = max;
            _value = max / 2;
        }
        public IntegerSlider(int min, int max, int value)
        {
            _min = min;
            _max = max;
            _value = value;
        }
        public static implicit operator int(IntegerSlider slider)
        {
            return slider.value;
        }
    }
    [System.Serializable]
    public struct HSV
    {
        [SerializeField]
        private float _h;
        [SerializeField]
        private float _s;
        [SerializeField]
        private float _v;
        public float h
        {
            get
            {
                return _h;
            }
            set
            {
                _h = value;
            }
        }
        public float s
        {
            get
            {
                return _s;
            }
            set
            {
                _s = value;
            }
        }
        public float v
        {
            get
            {
                return _v;
            }
            set
            {
                _v = value;
            }
        }
        public Color color
        {
            get
            {
                return Color.HSVToRGB(h, s, v);
            }
        }
        public HSV(float h, float s, float v)
        {
            _h = h;
            _s = s;
            _v = v;
        }
        public static HSV RGBtoHSV(Color color)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            return new HSV(h, s, v);
        }
        public static HSV LerpUnclamped(HSV a, HSV b, float t)
        {
            return a + (b - a) * t;
        }
        public static HSV Lerp(HSV a, HSV b, float t)
        {
            return LerpUnclamped(a, b, Mathf.Clamp01(t));
        }
        public static implicit operator Color(HSV hsv)
        {
            return hsv.color;
        }
        public static implicit operator HSV(Color color)
        {
            return RGBtoHSV(color);
        }
        public static HSV operator +(HSV a, HSV b)
        {
            return new()
            {
                h = a.h + b.h,
                s = a.s + b.s,
                v = a.v + b.v
            };
        }
        public static HSV operator -(HSV a, HSV b)
        {
            return new()
            {
                h = a.h - b.h,
                s = a.s - b.s,
                v = a.v - b.v
            };
        }
        public static HSV operator *(HSV a, HSV b)
        {
            return new()
            {
                h = a.h * b.h,
                s = a.s * b.s,
                v = a.v * b.v
            };
        }
        public static HSV operator *(HSV a, float b)
        {
            return new()
            {
                h = a.h * b,
                s = a.s * b,
                v = a.v * b
            };
        }
        public static HSV operator /(HSV a, HSV b)
        {
            return new()
            {
                h = a.h / b.h,
                s = a.s / b.s,
                v = a.v / b.v
            };
        }
        public static HSV operator /(HSV a, float b)
        {
            return new()
            {
                h = a.h / b,
                s = a.s / b,
                v = a.v / b
            };
        }

        public static HSV white => new(0, 0, 1);
        public static HSV black => new(0, 0, 0);
    }
    public static class Tools
    {
        public static Dictionary<string, Object> LoadCache { get; } = new();
        /// <summary>
        /// A Resources.Load<T>() that stores objects in a dictionary for faster performance
        /// </summary>
        public static T Load<T>(string path) where T : Object
        {
            if (LoadCache.TryGetValue(path, out var obj))
            {
                return obj as T;
            }
            else
            {
                var nobj = Resources.Load<T>(path);
                LoadCache.Add(path, nobj);
                return nobj;
            }
        }
        [RuntimeInitializeOnLoadMethod]
        public static void RuntimeLoad()
        {
            LoadCache.Clear();
        }
    }
    public static class Logging
    {
        public static void Log(object message) => UnityEngine.Debug.Log(message);
        public static void LogWarning(object message) => UnityEngine.Debug.LogWarning(message);
        public static void LogError(object message) => UnityEngine.Debug.LogError(message);
    }
}
