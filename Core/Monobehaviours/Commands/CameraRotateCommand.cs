using System.Collections;
using Control;
using UnityEngine;

namespace Core
{
    public class CameraRotateCommand : Command
    {
        private IRotationTrigger m_RotationTrigger;
        private SimpleCameraController m_CameraController;
        private Coroutine m_RotateRoutine;

        private void Awake()
        {
            m_RotationTrigger = GetComponent<IRotationTrigger>();
            m_CameraController = GetComponent<SimpleCameraController>();
        }

        public override void Execute()
        {
            if (m_RotateRoutine == null) m_RotateRoutine = StartCoroutine(Rotate());
        }

        private IEnumerator Rotate()
        {
            Cursor.lockState = CursorLockMode.Locked;
            while (m_RotationTrigger.IsRotating)
            {
                Vector2 mouseMovement = new Vector2(m_CameraController.MouseX,
                                            m_CameraController.MouseY * (m_CameraController.InvertY ? 1 : -1)) *
                                        (Time.deltaTime * 5);
                float mouseSensitivityFactor =
                    m_CameraController.MouseSensitivityCurve.Evaluate(mouseMovement.magnitude);
                m_CameraController.m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
                m_CameraController.m_TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;

                Vector3 translation = new Vector3() * Time.deltaTime;
                m_CameraController.m_TargetCameraState.Translate(translation);

                // Framerate-independent interpolation
                // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
                float positionLerpPct =
                    1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / m_CameraController.PositionLerpTime * Time.deltaTime);
                float rotationLerpPct =
                    1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / m_CameraController.RotationLerpTime * Time.deltaTime);
                m_CameraController.m_InterpolatingCameraState.LerpTowards(m_CameraController.m_TargetCameraState,
                    positionLerpPct,
                    rotationLerpPct);

                m_CameraController.m_InterpolatingCameraState.UpdateTransform(transform);

                yield return null;
            }

            Cursor.lockState = CursorLockMode.None;
            m_RotateRoutine = null;
        }
    }
}