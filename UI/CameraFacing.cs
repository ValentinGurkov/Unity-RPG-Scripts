using UnityEngine;

namespace RPG.UI {
    public class CameraFacing : MonoBehaviour {
        private Camera mainCamera;

        private void Start() {
            mainCamera = Camera.main;
        }

        private void LateUpdate() {
            transform.forward = mainCamera.transform.forward;
        }
    }
}
