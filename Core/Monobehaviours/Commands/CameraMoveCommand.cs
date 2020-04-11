using System.Collections;
using UnityEngine;

namespace Core
{
    public class CameraMoveCommand : Command
    {
        [Header("Movement Settings")]
        [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
        [SerializeField]
        private float boost = 3.5f;

        private IMoveInput m_MoveInput;
        private ISprintInput m_SprintInput;
        private IScrollYInput m_ScrollYInput;
        private ICameraState m_CameraState;
        private ILerpTime m_LerpTime;
        private Coroutine m_MoveRoutine;


        private void Awake()
        {
            m_MoveInput = GetComponent<IMoveInput>();
            m_SprintInput = GetComponent<ISprintInput>();
            m_ScrollYInput = GetComponent<IScrollYInput>();
            m_CameraState = GetComponent<ICameraState>();
            m_LerpTime = GetComponent<ILerpTime>();
        }

        public override void Execute()
        {
            if (m_MoveRoutine == null) m_MoveRoutine = StartCoroutine(Move());
        }

        private IEnumerator Move()
        {
            while (m_MoveInput.MoveDirection != Vector3.zero)
            {
                // Translation
                Vector3 translation = m_MoveInput.MoveDirection * Time.deltaTime;

                // Speed up movement when shift key held
                if (m_SprintInput.IsSprinting) translation *= 10.0f;

                // Modify movement by a boost factor (defined in Inspector and modified in play mode through the mouse scroll wheel)
                boost += m_ScrollYInput.ScrollY * 0.2f;
                translation *= Mathf.Pow(2.0f, boost);


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

            m_MoveRoutine = null;
        }
    }
}