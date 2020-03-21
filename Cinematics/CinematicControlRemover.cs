using System;
using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        public event Action<bool> OnCinematicStart;
        public event Action<bool> OnCinematicEnd;
        private PlayableDirector m_PlayableDirector;
        private ActionScheduler m_ActionScheduler;
        private GameObject m_Player;
        private PlayerController m_PlayerController;

        private void Awake()
        {
            m_PlayableDirector = GetComponent<PlayableDirector>();
            m_Player = GameObject.FindWithTag("Player");
            m_PlayerController = m_Player.GetComponent<PlayerController>();
            m_ActionScheduler = m_Player.GetComponent<ActionScheduler>();
        }

        private void OnEnable()
        {
            m_PlayableDirector.played += DisableControl;
            m_PlayableDirector.stopped += EnableControl;
        }

        private void OnDisable()
        {
            m_PlayableDirector.played -= DisableControl;
            m_PlayableDirector.stopped -= EnableControl;
        }

        private void EnableControl(PlayableDirector pd)
        {
            if (m_PlayableDirector != null)
            {
                m_PlayerController.enabled = true;
            }

            OnCinematicEnd(false);
        }

        private void DisableControl(PlayableDirector pd)
        {
            m_ActionScheduler.CancelCurrentAction();
            m_PlayerController.enabled = false;
            OnCinematicStart(true);
        }
    }
}