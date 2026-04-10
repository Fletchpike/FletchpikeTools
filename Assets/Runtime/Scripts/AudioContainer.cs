using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

namespace Fletchpike
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Audio Container", menuName = "Audio/Audio Container")]
    public class AudioContainer : ScriptableObject
    {
        [SerializeField] private AudioContainerClip[] _clips = new AudioContainerClip[] { new() };
        [SerializeField] private SingleRange _volumeRange = new(0f, 1f, 1f, 1f);
        [SerializeField] private SingleRange _pitchRange = new(-3f, 3f, 1f, 1f);
        [SerializeField] private int _avoidRepeatingLast;
        [SerializeField] private ClipSelection _selectOrder;
        [SerializeField] private DistanceReverb _distanceReverb = new();
        public bool useVolumeRandom;
        public bool usePitchRandom;
        private int index;
        private List<int> lastPicks;
        /// <summary>
        /// returns the AudioClips the elements contain
        /// </summary>
        public AudioClip[] audioClips => clips.ExtractAudioClips().ToArray();

        /// <summary>
        /// returns the volume (if using a range it returns the average, setting sets both min and max)
        /// </summary>
        public float volume
        {
            get
            {
                return (volumeRange.max + volumeRange.min) / 2f;
            }
            set
            {
                volumeRange.min = value;
                volumeRange.max = value;
            }
        }

        /// <summary>
        /// returns the pitch (if using a range it returns the average, setting sets both min and max)
        /// </summary>
        public float pitch
        {
            get
            {
                return (pitchRange.max + pitchRange.min) / 2f;
            }
            set
            {
                pitchRange.min = value;
                pitchRange.max = value;
            }
        }
        /// <summary>
        /// Settings For The Distance Based Reverb Effect
        /// </summary>
        public DistanceReverb distanceReverb
        {
            get
            {
                return _distanceReverb;
            }
            set
            {
                _distanceReverb = value;
            }
        }

        public SingleRange volumeRange
        {
            get => _volumeRange;
            set => _volumeRange = value;
        }
        public SingleRange pitchRange
        {
            get => _pitchRange;
            set => _pitchRange = value;
        }
        public AudioContainerClip[] clips
        {
            get => _clips;
            set => _clips = value;
        }
        public ClipSelection selectOrder
        {
            get => _selectOrder;
            set => _selectOrder = value;
        }
        public int avoidRepeatingLast
        {
            get => _avoidRepeatingLast;
            set => _avoidRepeatingLast = value;
        }

        public AudioClip Next()
        {
            if (this.clips == null || this.clips.Length <= 0) return null;
            var clips = new List<AudioContainerClip>(this.clips);
            clips.RemoveAll((item) => !item.enabled);
            AudioClip clip = null;
            switch (selectOrder)
            {
                case ClipSelection.Forward:
                    clip = clips[index].clip;
                    index++;
                    index %= clips.Count;
                    return clip;
                case ClipSelection.Backward:
                    clip = clips[index].clip;
                    index--;
                    if (index < 0) index = clips.Count - 1;
                    return clip;
                case ClipSelection.Random:
                    if (lastPicks == null) lastPicks = new();
                    int rng() => Random.Range(0, clips.Count);
                    index = rng();
                    var fails = 0;
                    while (lastPicks.Contains(index))
                    {
                        index = rng();
                        fails++;
                        // Just In Case
                        if (fails > 100000)
                        {
                            lastPicks.Clear();
                            return clips[index].clip;
                        }
                    }
                    lastPicks.Add(index);
                    if (lastPicks.Count > avoidRepeatingLast) lastPicks.RemoveAt(0);
                    clip = clips[index].clip;
                    return clip;
                default:
                    return clip;
            }
        }
        public int getActiveClips
        {
            get
            {
                int i = 0;
                foreach (var item in clips)
                {
                    if (item.enabled) i++;
                }
                return i;
            }
        }
        public void ApplyProperties(AudioSource audio)
        {
            audio.resource = Next();
            audio.volume *= volumeRange.random;
            audio.pitch *= pitchRange.random;
            if (distanceReverb.enabled)
            {
                var arc = audio.GetComponent<AudioReverbController>();
                if (arc == null) arc = audio.gameObject.AddComponent<AudioReverbController>();
                arc.settings = distanceReverb;
            }
        }
        public enum ClipSelection
        {
            [InspectorName("Sequential")]
            Forward,
            [InspectorName("Reverse")]
            Backward,
            Random
        }
        public enum DistanceReverbUpdateMode
        {
            [Tooltip("Update Whenever The GameObject Becomes Active")] Awake,
            [Tooltip("More Performance Heavy")] Update
        }
        /// <summary>
        /// A System To Add More Reverb The Farther A Object Is From The Audio Listener
        /// </summary>
        [System.Serializable]
        public class DistanceReverb
        {
            [SerializeField, Tooltip("Will This Effect Exist")] private bool _enabled = false;
            [Range(0, 1f), SerializeField, Tooltip("Max Amount Of Reverb It Can Apply")] private float _max = 1f;
            [Range(0, 1f), SerializeField, Tooltip("Lower This Value Is Faster The Reverb Maxes Out")] private float _end = 0.75f;
            [SerializeField, Tooltip("Times It Will Update The Reverb Amount")] private DistanceReverbUpdateMode _updateMode = DistanceReverbUpdateMode.Awake;

            public bool enabled
            {
                get => _enabled;
                set => _enabled = value;
            }
            public float max
            {
                get => _max;
                set => _max = value;
            }
            public float end
            {
                get => _end;
                set => _end = value;
            }
            public DistanceReverbUpdateMode updateMode
            {
                get => _updateMode;
                set => _updateMode = value;
            }
        }
    }
    [System.Serializable]
    public class AudioContainerClip
    {
        [SerializeField] private AudioClip _clip;
        [SerializeField] private bool _enabled;
        public AudioClip clip
        {
            get
            {
                return _clip;
            }
            set
            {
                _clip = value;
            }
        }
        public bool enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }
        public AudioContainerClip()
        {
            this.clip = null;
            this.enabled = true;
        }
        public AudioContainerClip(AudioClip clip, bool enabled)
        {
            this.clip = clip;
            this.enabled = enabled;
        }
    }
    public class AudioRandomContainerAccess
    {
        public AudioResource resource;
        private System.Type type;
        private PropertyInfo elementsField;
        private PropertyInfo volumeRangeField;
        private PropertyInfo volumeField;
        private PropertyInfo pitchRangeField;
        private PropertyInfo pitchField;
        private PropertyInfo playbackModeField;
        private PropertyInfo avoidRepeatingLastField;
        public const BindingFlags showAll = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
        public AudioRandomContainerAccess(AudioResource resource)
        {
            if (resource is AudioClip)
            {
                Logging.LogError("Not Designed For AudioClips");
            }
            else
            {
                this.resource = resource;
                type = resource.GetType();
                elementsField = type.GetProperty("elements", showAll);
                volumeRangeField = type.GetProperty("volumeRandomizationRange", showAll);
                volumeField = type.GetProperty("volume", showAll);
                pitchRangeField = type.GetProperty("pitchRandomizationRange", showAll);
                pitchField = type.GetProperty("pitch", showAll);
                playbackModeField = type.GetProperty("playbackMode", showAll);
                avoidRepeatingLastField = type.GetProperty("avoidRepeatingLast", showAll);
            }
        }
        /// <summary>
        /// Returns the AudioClips the Random Container Contains (does not allow setting array size)
        /// </summary>
        public AudioClip[] clips
        {
            get
            {
                List<AudioClip> result = new();
                var array = (IEnumerable)elementsField.GetValue(resource);
                foreach (var item in array)
                {
                    var clip = (AudioClip)item.GetType().GetProperty("audioClip", showAll).GetValue(item);
                    result.Add(clip);
                }
                return result.ToArray();
            }
            set
            {
                var array = (IEnumerable)elementsField.GetValue(resource);
                var i = 0;
                foreach (var item in array)
                {
                    item.GetType().GetProperty("audioClip", showAll).SetValue(item, value[i]);
                    i++;
                }
            }
        }
        /// <summary>
        /// Returns a SingleRange Property from -80 to 80 Dbs
        /// </summary>
        public SingleRange volumeRange
        {
            get
            {
                var volRange = (Vector2)volumeRangeField.GetValue(resource);
                return new(-80, 80, volRange.x, volRange.y);
            }
            set
            {
                volumeRangeField.SetValue(resource, new Vector2(value.min, value.max));
            }
        }
        /// <summary>
        /// Returns a float Property from -80 to 80 Dbs
        /// </summary>
        public float volume
        {
            get
            {
                var vol = (float)volumeField.GetValue(resource);
                return vol;
            }
            set
            {
                volumeField.SetValue(resource, value);
            }
        }
        /// <summary>
        /// Returns a float Property from -1200 to 1200 Cents
        /// </summary>
        public float pitch
        {
            get
            {
                var vol = (float)pitchField.GetValue(resource);
                return vol;
            }
            set
            {
                pitchField.SetValue(resource, value);
            }
        }
        /// <summary>
        /// Returns a SingleRange Property from -1200 to 1200 Cents
        /// </summary>
        public SingleRange pitchRange
        {
            get
            {
                var pitRange = (Vector2)pitchRangeField.GetValue(resource);
                return new(-1200, 1200, pitRange.x, pitRange.y);
            }
            set
            {
                pitchRangeField.SetValue(resource, new Vector2(value.min, value.max));
            }
        }
        /// <summary>
        /// The PlaybackMode of the Random Container
        /// </summary>
        public PlaybackMode playbackMode
        {
            get
            {
                return (PlaybackMode)(int)playbackModeField.GetValue(resource);
            }
            set
            {
                playbackModeField.SetValue(resource, (int)value);
            }
        }
        /// <summary>
        /// The amount of times it will wait to execute the same clip again
        /// </summary>
        public int avoidRepeatingLast
        {
            get
            {
                return (int)avoidRepeatingLastField.GetValue(resource);
            }
            set
            {
                avoidRepeatingLastField.SetValue(resource, value);
            }
        }
        public enum PlaybackMode
        {
            Sequential,
            Shuffle,
            Random
        }

        public void CreateAudioContainer(AudioContainer container)
        {
            var lis = new List<AudioContainerClip>();
            foreach (var item in clips) lis.Add(new(item, true));
            container.clips = lis.ToArray();
            container.selectOrder = playbackMode == PlaybackMode.Random ? AudioContainer.ClipSelection.Random : AudioContainer.ClipSelection.Forward;
            container.avoidRepeatingLast = avoidRepeatingLast;
            container.usePitchRandom = true;
            container.useVolumeRandom = true;
            var vr = volumeRange;
            var vol = volume;
            container.volumeRange = new(0, 1, vr.min.FromDB() + vol.FromDB() - 1, vr.max.FromDB() + vol.FromDB() - 1);
            var pr = pitchRange;
            var pit = pitch;
            container.pitchRange = new(-3, 3, pr.min.FromCents() + pit.FromCents() - 1, pr.max.FromCents() + pit.FromCents() - 1);
        }
    }
}
