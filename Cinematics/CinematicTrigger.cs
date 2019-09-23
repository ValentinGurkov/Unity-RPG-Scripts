using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics {

    public class CinematicTrigger : MonoBehaviour {
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
    }
}