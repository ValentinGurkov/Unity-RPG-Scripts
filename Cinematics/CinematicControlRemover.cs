using System;
using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics {

    public class CinematicControlRemover : MonoBehaviour {
        public event Action onCinematicStart;
        public event Action onCinematicEnd;
        private PlayableDirector playableDirector;
        private ActionScheduler actionScheduler;
        private GameObject player;
        private PlayerController playerController;

        private void Awake() {
            playableDirector = GetComponent<PlayableDirector>();
            player = GameObject.FindWithTag("Player");
            playerController = player.GetComponent<PlayerController>();
            actionScheduler = player.GetComponent<ActionScheduler>();
        }

        private void OnEnable() {
            playableDirector.played += DisableControl;
            playableDirector.stopped += EnableControl;
        }

        private void OnDisable() {
            playableDirector.played -= DisableControl;
            playableDirector.stopped -= EnableControl;
        }

        public void EnableControl(PlayableDirector pd) {
            if (playableDirector != null) {
                playerController.enabled = true;
            }
            onCinematicEnd();
        }

        public void DisableControl(PlayableDirector pd) {
            actionScheduler.CancelCurrentAction();
            playerController.enabled = false;
            onCinematicStart();
        }
    }
}
