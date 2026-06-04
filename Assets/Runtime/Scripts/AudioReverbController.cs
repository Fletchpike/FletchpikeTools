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
        public void PlayOneShot(AudioClip clip, float volumeScale)
        {
            clip = SetupAudioClip(clip);
            audio.PlayOneShot(clip, volumeScale);
        }
        public void PlayOneShot(AudioClip clip)
        {
            PlayOneShot(clip, 1f);
        }
        public AudioClip SetupAudioClip(AudioClip clip)
        {
            if (extendWithSilence)
            {
                if (clip.loadState == AudioDataLoadState.Unloaded)
                {
                    clip.LoadAudioData();
                }
                if (silenceAudioClipCache.TryGetValue(clip, out var silence))
                {
                    return silence;
                }
                else
                {
                    var si = clip.CopyWithSilence();
                    silenceAudioClipCache.Add(clip, si);
                    return si;
                }
            }
            else
            {
                return clip;
            }
        }
        private void Start()
        {
            settings ??= new() { enabled = true, updateMode = defaultUpdateMode};
            ComponentRefresh();
            if (extendWithSilence)
            {
                audio.clip = SetupAudioClip(audio.clip);
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
