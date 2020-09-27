using UnityEngine;

namespace Audio
{
    /// <summary>
    /// Allows playing random audio FX from a collection.
    /// </summary>
    public class AudioRandomizer : MonoBehaviour
    {
        [SerializeField] private AudioClip[] audioClips;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = audioClips[0];
        }

        // Called by a Unity event
        public void PlayRandomClip()
        {
            if (audioClips.Length > 1)
            {
                AudioClip prevClip = _audioSource.clip;

                do
                {
                    _audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
                } while (_audioSource.clip == prevClip);
            }

            _audioSource.pitch = Random.Range(0.9f, 1.1f);
            _audioSource.Play();
        }
    }
}