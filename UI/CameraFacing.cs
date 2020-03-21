using UnityEngine;

namespace RPG.UI
{
    public class CameraFacing : MonoBehaviour
    {
        private Camera m_MainCamera;

        private void Start()
        {
            m_MainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            transform.forward = m_MainCamera.transform.forward;
        }
    }
}