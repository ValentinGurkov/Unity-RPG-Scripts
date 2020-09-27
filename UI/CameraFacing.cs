using UnityEngine;

namespace UI
{
    public class CameraFacing : MonoBehaviour
    {
        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            transform.forward = _mainCamera.transform.forward;
        }
    }
}