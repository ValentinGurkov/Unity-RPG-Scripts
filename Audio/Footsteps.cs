using UnityEngine;

namespace RPG.Audio
{
    // TODO
    /// <summary>
    /// The implementation for footsteps sounds is still TODO
    /// </summary>
    public class Footsteps : MonoBehaviour
    {
        [SerializeField] private AudioClip walkClip;
        [SerializeField] private AudioClip runClip;
        [SerializeField] private AudioSource audioSource;
        private const string ACTION_WALK = "walk";
        private const string ACTION_RUN = "run";

        private void Step(string action)
        {
            if (!gameObject.CompareTag("Player")) return;
            Debug.Log(action);
            switch (action)
            {
                case ACTION_RUN:
                {
                    if (audioSource.clip != runClip)
                    {
                        audioSource.Stop();
                        audioSource.clip = runClip;
                    }

                    if (!audioSource.isPlaying)
                    {
                        audioSource.PlayOneShot(audioSource.clip);
                    }
                    else
                    {
                        audioSource.Stop();
                    }

                    break;
                }
                case ACTION_WALK:
                {
                    if (audioSource.clip != walkClip)
                    {
                        audioSource.Stop();
                        audioSource.clip = walkClip;
                    }

                    if (!audioSource.isPlaying)
                    {
                        audioSource.PlayOneShot(audioSource.clip);
                    }
                    else
                    {
                        audioSource.Stop();
                    }

                    break;
                }
                default:
                    audioSource.Stop();
                    break;
            }
        }
    }
}