using UnityEngine;

namespace RPG.Audio
{
    /// <summary>
    /// Allows playing random audio FX from a collection.
    /// </summary>
    public class AudioRandomizer : MonoBehaviour
    {
        [SerializeField] private AudioClip[] audioClips;

        private AudioSource m_AudioSource;

        private void Awake()
        {
            m_AudioSource = GetComponent<AudioSource>();
            m_AudioSource.clip = audioClips[0];
        }

        public void PlayRandomClip()
        {
            if (audioClips.Length > 1)
            {
                AudioClip prevClip = m_AudioSource.clip;

                do
                {
                    m_AudioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
                } while (m_AudioSource.clip == prevClip);
            }

            m_AudioSource.pitch = Random.Range(0.9f, 1.1f);
            m_AudioSource.Play();
        }
    }
}