using System.Collections;
using UnityEngine;

namespace Core
{
    // <summary>
    /// Unity HDRP preview scene camera using the new input system
    /// </summary>
    public class CameraRotateCommand : Command
    {
        [Header("Rotation Settings")]
        [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
        [SerializeField]
        private AnimationCurve mouseSensitivityCurve =
            new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

        [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")] [SerializeField]
        private bool invertY;

        private IRotationTrigger _rotationTrigger;
        private IRotationInput _rotationInput;
        private ICameraState _cameraState;
        private ILerpTime _lerpTime;
        private Coroutine _rotateRoutine;

        private void Awake()
        {
            _rotationTrigger = GetComponent<IRotationTrigger>();
            _rotationInput = GetComponent<IRotationInput>();
            _cameraState = GetComponent<ICameraState>();
            _lerpTime = GetComponent<ILerpTime>();
        }

        public override void Execute()
        {
            if (_rotateRoutine == null) _rotateRoutine = StartCoroutine(Rotate());
        }

        private IEnumerator Rotate()
        {
            Cursor.lockState = CursorLockMode.Locked;
            while (_rotationTrigger.IsRotating)
            {
                Vector2 mouseMovement =
                    new Vector2(_rotationInput.RotationDirection.x,
                        _rotationInput.RotationDirection.z * (invertY ? 1 : -1)) *
                    (Time.deltaTime * 5);
                float mouseSensitivityFactor =
                    mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);
                _cameraState.TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
                _cameraState.TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;

                Vector3 translation = new Vector3() * Time.deltaTime;
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

            Cursor.lockState = CursorLockMode.None;
            _rotateRoutine = null;
        }
    }
}