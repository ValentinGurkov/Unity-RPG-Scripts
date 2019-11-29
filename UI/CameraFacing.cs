using UnityEngine;

namespace RPG.UI {
    public class CameraFacing : MonoBehaviour {

        private void LateUpdate() {
            transform.forward = Camera.main.transform.forward;
        }
    }

}
