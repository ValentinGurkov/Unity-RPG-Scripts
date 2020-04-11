using System.Collections;
using UnityEngine;

namespace Core
{
    public class CameraRotateCommand : Command
    {
        [Header("Rotation Settings")]
        [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
        [SerializeField]
        private AnimationCurve mouseSensitivityCurve =
            new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

        [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")] [SerializeField]
        private bool invertY;

        private IRotationTrigger m_RotationTrigger;
        private IRotationInput m_RotationInput;
        private ICameraState m_CameraState;
        private ILerpTime m_LerpTime;
        private Coroutine m_RotateRoutine;

        private void Awake()
        {
            m_RotationTrigger = GetComponent<IRotationTrigger>();
            m_RotationInput = GetComponent<IRotationInput>();
            m_CameraState = GetComponent<ICameraState>();
            m_LerpTime = GetComponent<ILerpTime>();
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
                Vector2 mouseMovement =
                    new Vector2(m_RotationInput.RotationDirection.x,
                        m_RotationInput.RotationDirection.z * (invertY ? 1 : -1)) *
                    (Time.deltaTime * 5);
                float mouseSensitivityFactor =
                    mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);
                m_CameraState.TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
                m_CameraState.TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;

                Vector3 translation = new Vector3() * Time.deltaTime;
                m_CameraState.TargetCameraState.Translate(translation);

                // Framerate-independent interpolation
                // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
                float positionLerpPct =
                    1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / m_LerpTime.PositionLerpTime * Time.deltaTime);
                float rotationLerpPct =
                    1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / m_LerpTime.RotationLerpTime * Time.deltaTime);
                m_CameraState.InterpolatingCameraState.LerpTowards(m_CameraState.TargetCameraState,
                    positionLerpPct,
                    rotationLerpPct);

                m_CameraState.InterpolatingCameraState.UpdateTransform(transform);

                yield return null;
            }

            Cursor.lockState = CursorLockMode.None;
            m_RotateRoutine = null;
        }
    }
}