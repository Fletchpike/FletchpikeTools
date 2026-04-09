using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace Fletchpike
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Audio Container", menuName = "Audio/Audio Container")]
    public class AudioContainer : ScriptableObject
    {
        public AudioClip[] clips;
        public SingleRange volumeRange = new(0f, 1f, 1f, 1f);
        public SingleRange pitchRange = new(-3f, 3f, 1f, 1f);
        public int avoidRepeatingLast;
        public ClipSelection selectOrder;
        private int index;
        private List<int> lastPicks;

        public AudioClip Next()
        {
            if (clips == null || clips.Length <= 0) return null;
            AudioClip clip = null;
            switch (selectOrder)
            {
                case ClipSelection.Forward:
                    clip = clips[index];
                    index++;
                    index %= clips.Length;
                    return clip;
                case ClipSelection.Backward:
                    clip = clips[index];
                    index--;
                    if (index < 0) index = clips.Length - 1;
                    return clip;
                case ClipSelection.Random:
                    if (lastPicks == null) lastPicks = new();
                    int rng() => Random.Range(0, clips.Length);
                    index = rng();
                    while (lastPicks.Contains(index))
                    {
                        index = rng();
                    }
                    lastPicks.Add(index);
                    if (lastPicks.Count > avoidRepeatingLast) lastPicks.RemoveAt(0);
                    clip = clips[index];
                    return clip;
                default:
                    return clip;
            }
        }
        public enum ClipSelection
        {
            Forward,
            Backward,
            Random
        }
    }
}
