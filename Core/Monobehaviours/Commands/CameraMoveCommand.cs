using System.Collections;
using UnityEngine;

namespace Core
{
    // <summary>
    /// Unity HDRP preview scene camera using the new input system
    /// </summary>
    public class CameraMoveCommand : Command
    {
        [Header("Movement Settings")] [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")] [SerializeField]
        private float boost = 3.5f;

        private IMoveInput _moveInput;
        private ISprintInput _sprintInput;
        private IScrollYInput _scrollYInput;
        private ICameraState _cameraState;
        private ILerpTime _lerpTime;
        private Coroutine _moveRoutine;


        private void Awake()
        {
            _moveInput = GetComponent<IMoveInput>();
            _sprintInput = GetComponent<ISprintInput>();
            _scrollYInput = GetComponent<IScrollYInput>();
            _cameraState = GetComponent<ICameraState>();
            _lerpTime = GetComponent<ILerpTime>();
        }

        public override void Execute()
        {
            if (_moveRoutine == null) _moveRoutine = StartCoroutine(Move());
        }

        private IEnumerator Move()
        {
            while (_moveInput.MoveDirection != Vector3.zero)
            {
                // Translation
                Vector3 translation = _moveInput.MoveDirection * Time.deltaTime;

                // Speed up movement when shift key held
                if (_sprintInput.IsSprinting) translation *= 10.0f;

                // Modify movement by a boost factor (defined in Inspector and modified in play mode through the mouse scroll wheel)
                boost += _scrollYInput.ScrollY * 0.2f;
                translation *= Mathf.Pow(2.0f, boost);


                _cameraState.TargetCameraState.Translate(translation);

                // Framerate-independent interpolation
                // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
                float positionLerpPct =
                    1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / _lerpTime.PositionLerpTime * Time.deltaTime);
                float rotationLerpPct =
                    1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / _lerpTime.RotationLerpTime * Time.deltaTime);
                _cameraState.InterpolatingCameraState.LerpTowards(_cameraState.TargetCameraState,
                    positionLerpPct,
                    rotationLerpPct);

                _cameraState.InterpolatingCameraState.UpdateTransform(transform);
                yield return null;
            }

            _moveRoutine = null;
        }
    }
}