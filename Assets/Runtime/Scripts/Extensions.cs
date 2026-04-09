using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using PRandom = System.Random;

namespace Fletchpike
{
    public static class CommonExtensions
    {
        public static float ToDB(this float value)
        {
            return Mathf.Log10(value == 0 ? value : 0.001f) * 20;
        }
        public static float FromDB(this float value)
        {
            return Mathf.Pow(10, value / 20f);
        }
        /// <summary>
        /// Cut out Decimals of a Number string. (example: 1.24628 => 1.24)
        /// </summary>
        public static string CutOutDigits(string txt, int digits)
        {
            digits++;
            if (txt.IndexOf('.') != -1)
            {
                txt = txt.Substring(0, Mathf.Clamp(txt.IndexOf('.') + digits, 0, txt.Length));
            }
            return txt;
        }
        public static double NextDouble(this PRandom random, double min, double max)
        {
            var d = random.NextDouble();
            return d * (max - min) + min;
        }
        public static float NextFloat(this PRandom random, float min, float max)
        {
            var d = random.NextDouble();
            return (float)(d * (max - min) + min);
        }
        public static float NextFloat(this PRandom random)
        {
            return (float)random.NextDouble();
        }
    }
    public static class ArrayListExtensions
    {
        public static void Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
    public static class AudioExtensions
    {
        public static void PlayOneShot(this AudioSource audio, AudioClip[] clips)
        {
            if (clips.Length < 1) return;
            audio.PlayOneShot(clips[Random.Range(0, clips.Length)]);
        }
        public static void PlayOneShot(this AudioSource audio, AudioClip[] clips, float volumeScale)
        {
            if (clips.Length < 1) return;
            audio.PlayOneShot(clips[Random.Range(0, clips.Length)], volumeScale);
        }
        public static bool IsAudioFilter(Component component)
        {
            var t = component.GetType();
            foreach (var item in builtInAudioFilters)
            {
                if (item.FullName == t.FullName) return true;
            }
            if (IsCustomAudioFilter(component)) return true;
            return false;
        }
        internal static bool IsCustomAudioFilter(Component component)
        {
            MethodInfo method = component.GetType().GetMethod("OnAudioFilterRead",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return method != null;
        }
        public static List<Type> builtInAudioFilters { get; } = new(new Type[] {
            typeof(AudioChorusFilter),
            typeof(AudioDistortionFilter),
            typeof(AudioEchoFilter),
            typeof(AudioHighPassFilter),
            typeof(AudioLowPassFilter),
            typeof(AudioReverbFilter)
        });
    }
    public static class TextureExtensions
    {
        public static Texture2D ToTexture2D(this RenderTexture rTex)
        {
            Texture2D tex = new Texture2D(rTex.width, rTex.height);
            RenderTexture old_rt = RenderTexture.active;
            RenderTexture.active = rTex;
            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();
            RenderTexture.active = old_rt;
            return tex;
        }
        public static void RemoveTransparent(this Texture2D texture)
        {
            var pixels = texture.GetPixels32();
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].a < 255)
                {
                    pixels[i].a = 0;
                }
            }
            texture.SetPixels32(pixels);
        }
        public static void SolidifyTransparent(this Texture2D texture)
        {
            var pixels = texture.GetPixels32();
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].a > 0)
                {
                    pixels[i].a = 255;
                }
            }
            texture.SetPixels32(pixels);
        }
        public static void SetAllSolidColors(this Texture2D texture, Color color)
        {
            var pixels = texture.GetPixels32();
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].a > 0)
                {
                    pixels[i] = color;
                }
            }
            texture.SetPixels32(pixels);
        }
        public static void CutOutColor(this Texture2D texture, Color color)
        {
            var pixels = texture.GetPixels32();
            for (int i = 0; i < pixels.Length; i++)
            {
                if (Equals(pixels[i], (Color32)color))
                {
                    pixels[i] = new(0, 0, 0, 0);
                }
            }
            texture.SetPixels32(pixels);
        }
        public static bool Equals(Color32 a, Color32 b)
        {
            return a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
        }
        public static Texture2D CropToMin(this Texture2D source)
        {
            int size = Mathf.Min(source.width, source.height);
            int xOffset = (source.width - size) / 2;
            int yOffset = (source.height - size) / 2;
            Color[] pixels = source.GetPixels(xOffset, yOffset, size, size);
            Texture2D result = new Texture2D(size, size);
            result.SetPixels(pixels);
            result.Apply();
            return result;
        }
        public static Texture2D CropToMax(this Texture2D source)
        {
            var h = Mathf.Max(source.width, source.height);
            var wh = h == source.width;
            var offset = wh ? (h / 2) - (source.height / 2) : (h / 2) - (source.width / 2);
            Color32[] pixels = source.GetPixels32();
            Texture2D croppedTexture = new Texture2D(h, h);
            croppedTexture.SetAllSolidColors(Color.clear);
            croppedTexture.SetPixels32(wh ? 0 : offset, wh ? offset : 0, source.width, source.height, pixels);
            return croppedTexture;
        }

        public static Texture2D CropToOpaque(this Texture2D source)
        {
            Color[] pixels = source.GetPixels();
            int width = source.width;
            int height = source.height;

            int minX = width, minY = height, maxX = 0, maxY = 0;
            bool anyOpaque = false;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (pixels[y * width + x].a > 0)
                    {
                        if (x < minX) minX = x;
                        if (y < minY) minY = y;
                        if (x > maxX) maxX = x;
                        if (y > maxY) maxY = y;
                        anyOpaque = true;
                    }
                }
            }

            if (!anyOpaque) return null;
            int newWidth = maxX - minX + 1;
            int newHeight = maxY - minY + 1;
            Color[] croppedPixels = source.GetPixels(minX, minY, newWidth, newHeight);
            Texture2D croppedTexture = new Texture2D(newWidth, newHeight);
            croppedTexture.SetPixels(croppedPixels);
            croppedTexture.Apply();
            return croppedTexture;
        }
        public static Texture2D ResizeNearestNeighbor(this Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
            for (int y = 0; y < targetHeight; y++)
            {
                for (int x = 0; x < targetWidth; x++)
                {
                    float xPercent = (float)x / targetWidth;
                    float yPercent = (float)y / targetHeight;
                    int sourceX = Mathf.FloorToInt(xPercent * source.width);
                    int sourceY = Mathf.FloorToInt(yPercent * source.height);
                    Color pixelColor = source.GetPixel(sourceX, sourceY);
                    result.SetPixel(x, y, pixelColor);
                }
            }
            return result;
        }
        public static Texture2D ResizeNearestNeighbor(this Texture2D source, int targetSize)
        {
            var h = Mathf.Max(source.width, source.height);
            var r = (float)h / (float)targetSize;
            return source.ResizeNearestNeighbor(Mathf.FloorToInt((float)source.width / r), Mathf.FloorToInt((float)source.height / r));
        }
    }
}
