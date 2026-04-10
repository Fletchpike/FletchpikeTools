using UnityEngine;

namespace Fletchpike
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioContainerSource : MonoBehaviour
    {
        [SerializeField] private AudioContainer _container;
        public new AudioSource audio { get; private set; }
        public AudioContainer container
        {
            get => _container;
            set => _container = value;
        }
        private void Awake()
        {
            audio = GetComponent<AudioSource>();
        }
        public void Play()
        { 
            if (audio == null) audio = GetComponent<AudioSource>();
            audio.PlayOneShot(container);
        }
    }
}
