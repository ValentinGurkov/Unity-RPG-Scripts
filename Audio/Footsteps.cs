using UnityEngine;

namespace RPG.Audio {
    public class Footsteps : MonoBehaviour {
        [SerializeField] private AudioClip[] audioClips;
        [SerializeField] private AudioSource audioSource;
        private const string ACTION_WALK = "walk";

        void Awake() {
            audioSource = GetComponent<AudioSource>();
        }

        private void Step(string action) {
            /* if (action == ACTION_WALK) {
                 GetRandomAudioClip();
                 audioSource.PlayOneShot(audioSource.clip);
             } */
        }

        private void GetRandomAudioClip() {
            AudioClip prevClip = audioSource.clip;

            do {
                audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
            } while (audioSource.clip == prevClip);
        }
    }

}
