using RPG.Saving;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics {

    public class CinematicTrigger : MonoBehaviour, ISaveable {
        private PlayableDirector playableDirector;
        private bool isAlreadyTriggered = false;

        private void Awake() {
            playableDirector = GetComponent<PlayableDirector>();
        }

        private void OnTriggerEnter(Collider other) {
            if (!isAlreadyTriggered && other.gameObject.tag == "Player") {
                playableDirector.Play();
                isAlreadyTriggered = true;
            }
        }

        public object CaptureState() {
            return isAlreadyTriggered;
        }

        public void RestoreState(object state) {
            isAlreadyTriggered = (bool) state;
        }
    }
}
