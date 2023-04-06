using Saving;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        private PlayableDirector m_PlayableDirector;
        private bool m_IsAlreadyTriggered;

        private void Awake()
        {
            m_PlayableDirector = GetComponent<PlayableDirector>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (m_IsAlreadyTriggered || !other.gameObject.CompareTag("Player")) return;
            m_PlayableDirector.Play();
            m_IsAlreadyTriggered = true;
        }

        public object CaptureState()
        {
            return m_IsAlreadyTriggered;
        }

        public void RestoreState(object state)
        {
            m_IsAlreadyTriggered = (bool) state;
        }
    }
}