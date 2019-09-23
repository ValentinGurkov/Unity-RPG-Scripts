using UnityEngine;

namespace RPG.Control {
    public class CameraFacing : MonoBehaviour {

        private void LateUpdate() {
            transform.forward = Camera.main.transform.forward;
        }
    }

}
