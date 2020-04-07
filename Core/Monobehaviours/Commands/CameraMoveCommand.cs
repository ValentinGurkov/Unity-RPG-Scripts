using Control;
using System.Collections;
using UnityEngine;

namespace Core
{
    public class CameraMoveCommand : Command
    {
        private IMoveInput m_MoveInput;
        private ISprintInput m_SprintInput;
        private SimpleCameraController m_CameraController;
        private Coroutine m_MoveRoutine;


        private void Awake()
        {
            m_MoveInput = GetComponent<IMoveInput>();
            m_SprintInput = GetComponent<ISprintInput>();
            m_CameraController = GetComponent<SimpleCameraController>();
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

                //TODO boost based on mouse scroll here


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

            m_MoveRoutine = null;
        }
    }
}