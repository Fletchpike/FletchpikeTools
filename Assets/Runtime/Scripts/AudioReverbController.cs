using UnityEngine;
using System.Collections.Generic;

namespace Fletchpike
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioReverbController : MonoBehaviour
    {
        public bool extendWithSilence = true;
        [Tooltip("Play Clip After Silence Clip Is Added")]
        public bool playOnStart = true;
        public AudioContainer.DistanceReverbUpdateMode defaultUpdateMode;
        public new AudioSource audio { get; private set; }
        public AudioReverbFilter filter { get; private set; }
        public AudioContainer.DistanceReverb settings { get; set; }
        private float lastDistance;
        public static Dictionary<AudioClip, AudioClip> silenceAudioClipCache;
        public void ComponentRefresh()
        {
            if (audio == null || filter == null)
            {
                audio = GetComponent<AudioSource>();
                filter = GetComponent<AudioReverbFilter>();
                if (filter == null)
                {
                    filter = gameObject.AddComponent<AudioReverbFilter>();
                }
            }
        }
        private void Start()
        {
            if (settings == null)
            {
                settings = new() { enabled = true, updateMode = defaultUpdateMode};
            }
            ComponentRefresh();
            if (extendWithSilence)
            {
                if (audio.clip.loadState == AudioDataLoadState.Unloaded)
                {
                    audio.clip.LoadAudioData();
                }
                if (silenceAudioClipCache.TryGetValue(audio.clip, out var silence))
                {
                    audio.clip = silence;
                }
                else
                {
                    var si = audio.clip.CopyWithSilence();
                    silenceAudioClipCache.Add(audio.clip, si);
                    audio.clip = si;
                }
            }
            if (playOnStart)
            {
                audio.Play();
            }
            RefreshReverb();
        }
        private void Update()
        {
            if (settings.updateMode == AudioContainer.DistanceReverbUpdateMode.Update)
            {
                var listDist = DistanceFromListener();
                if (Mathf.Abs(lastDistance - listDist) > 0.5f)
                {
                    lastDistance = listDist;
                    RefreshReverb();
                }
            }
        }
        private void OnEnable()
        {
            if (settings == null) return;
            if (settings.updateMode == AudioContainer.DistanceReverbUpdateMode.Awake)
            {
                RefreshReverb();
            }
        }
        public void RefreshReverb()
        {
            if (settings == null) return;
            ComponentRefresh();
            var dist = Mathf.Clamp(DistanceFromListener() / audio.maxDistance / settings.end, 0, settings.max);
            filter.SetParameters(AudioReverbParameters.Lerp(AudioExtensions.ReverbParametersOff, AudioExtensions.ReverbParametersPsychotic, dist));
        }
        public float DistanceFromListener() => AudioExtensions.DistanceFromListener(transform.position);
    }
}
