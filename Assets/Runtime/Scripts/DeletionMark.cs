using UnityEngine;

namespace Fletchpike
{
    [ExecuteInEditMode]
    public class DeletionMark : MonoBehaviour
    {
        [SerializeField] private DeletionMarkType _markType;
        public AudioSource audioSource { get; private set; }
        public float waitTime { get; set; }
        public bool executeInEditMode { get; set; }
        public DeletionMarkType markType
        {
            get
            {
                return _markType;
            }
            set
            {
                _markType = value;
            }
        }
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }
        private void Update()
        {
            if (!Application.isPlaying && !executeInEditMode) return;
            if (waitTime > 0)
            {
                waitTime -= Time.deltaTime;
                return;
            }
            if (markType == DeletionMarkType.AudioSource)
            {
                if (audioSource != null)
                { 
                    if (!audioSource.isPlaying)
                    {
                        Destroy(gameObject);
                    }
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
    public enum DeletionMarkType
    { 
        Undefined,
        AudioSource
    }
}
