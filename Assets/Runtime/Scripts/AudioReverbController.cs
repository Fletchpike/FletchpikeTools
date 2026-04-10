using UnityEngine;

namespace Fletchpike
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioReverbController : MonoBehaviour
    {
        public AudioContainer.DistanceReverbUpdateMode defaultUpdateMode;
        public new AudioSource audio { get; private set; }
        public AudioReverbFilter filter { get; private set; }
        public AudioContainer.DistanceReverb settings { get; set; }
        private float lastDistance;
        private void Start()
        {
            if (settings == null)
            {
                settings = new() { enabled = true, updateMode = defaultUpdateMode};
            }
            audio = GetComponent<AudioSource>();
            filter = GetComponent<AudioReverbFilter>();
            if (filter == null)
            {
                filter = gameObject.AddComponent<AudioReverbFilter>();
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
            var dist = Mathf.Clamp(DistanceFromListener() / audio.maxDistance / settings.end, 0, settings.max);
            filter.SetParameters(AudioReverbParameters.Lerp(AudioExtensions.ReverbParametersOff, AudioExtensions.ReverbParametersPsychotic, dist));
        }
        public float DistanceFromListener() => AudioExtensions.DistanceFromListener(transform.position);
    }
}
