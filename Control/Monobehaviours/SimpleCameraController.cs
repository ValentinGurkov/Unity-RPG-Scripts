using System;
using System.Security.Cryptography.X509Certificates;
using Core;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Control
{
    public class SimpleCameraController : MonoBehaviour, IMoveInput, ISprintInput, IRotationTrigger
    {
        #region Members

        [SerializeField] private Command movementInput;
        [SerializeField] private Command rotationInput;


        [Header("Movement Settings")]
        [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
        [SerializeField]
        private float boost = 3.5f;

        [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
        [SerializeField]
        private float positionLerpTime = 0.2f;

        [Header("Rotation Settings")]
        [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
        [SerializeField]
        private AnimationCurve mouseSensitivityCurve =
            new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
        [SerializeField]
        private float rotationLerpTime = 0.01f;

        [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")] [SerializeField]
        private bool invertY;

        [SerializeField] private Vector3 moveDirection;
        [SerializeField] private bool isSprinting;
        [SerializeField] private bool isRotating;


        private PlayerInputActions m_InputActions;

        public float PositionLerpTime => positionLerpTime;
        public float RotationLerpTime => rotationLerpTime;
        public Vector3 MoveDirection => moveDirection;
        public bool IsSprinting => isSprinting;
        public bool IsRotating => isRotating;
        public bool InvertY => invertY;
        public AnimationCurve MouseSensitivityCurve => mouseSensitivityCurve;

        public float MouseY;
        public float MouseX;
        public float ScrollY;
        public Vector2 MouseDelta;

        //remove this test
        public readonly CameraState m_TargetCameraState = new CameraState();
        public readonly CameraState m_InterpolatingCameraState = new CameraState();

        #endregion

        #region Lifecycle hooks

        private void Awake()
        {
            m_InputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            m_TargetCameraState.SetFromTransform(transform);
            m_InterpolatingCameraState.SetFromTransform(transform);
            m_InputActions.Camera.Enable();
            if (movementInput != null)
            {
                m_InputActions.Camera.Movement.performed += OnMoveInput;
                m_InputActions.Camera.Sprint.performed += OnSprintInput;
            }

            if (rotationInput != null) m_InputActions.Camera.Rotate.performed += OnRotateInput;

            m_InputActions.Camera.RotationX.performed += OnRotationX;
            m_InputActions.Camera.RotationY.performed += OnRotationY;
            m_InputActions.Camera.ScrollY.performed += OnScrollY;

            m_InputActions.Camera.RotationX.canceled += ctx => MouseX = 0;
            m_InputActions.Camera.RotationY.canceled += ctx => MouseY = 0;
        }

        private void OnDisable()
        {
            if (movementInput != null)
            {
                m_InputActions.Camera.Movement.performed -= OnMoveInput;
                m_InputActions.Camera.Sprint.performed += OnSprintInput;
            }

            if (rotationInput != null) m_InputActions.Camera.Rotate.performed -= OnRotateInput;
            m_InputActions.Camera.RotationX.performed -= OnRotationX;
            m_InputActions.Camera.RotationY.performed -= OnRotationY;
            m_InputActions.Camera.Disable();
        }


        private void Update()
        {
            return;

            if (isRotating)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Vector2 mouseMovement = new Vector2(MouseX, MouseY * (InvertY ? 1 : -1)) * Time.deltaTime;
                float mouseSensitivityFactor =
                    MouseSensitivityCurve.Evaluate(mouseMovement.magnitude);
                m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
                m_TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }

            Vector3 translation = moveDirection * Time.deltaTime;

            if (isSprinting)
            {
                translation *= 10.0f;
            }

            // Modify movement by a boost factor (defined in Inspector and modified in play mode through the mouse scroll wheel)
            boost += ScrollY * 0.2f;
            translation *= Mathf.Pow(2.0f, boost);

            m_TargetCameraState.Translate(translation);

            // Framerate-independent interpolation
            // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
            float positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
            float rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
            m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, positionLerpPct, rotationLerpPct);

            m_InterpolatingCameraState.UpdateTransform(transform);
        }

        #endregion

        #region Input Callbacks

        private void OnMoveInput(InputAction.CallbackContext obj)
        {
            var value = obj.ReadValue<Vector2>();
            moveDirection = new Vector3(value.x, 0, value.y);
            movementInput.Execute();
        }

        private void OnSprintInput(InputAction.CallbackContext obj)
        {
            var value = obj.ReadValue<float>();
            isSprinting = value >= 0.15f;
        }

        private void OnRotateInput(InputAction.CallbackContext obj)
        {
            var value = obj.ReadValue<float>();
            isRotating = value >= 0.15f;
            rotationInput.Execute();
        }

        private void OnRotationX(InputAction.CallbackContext obj)
        {
            if (!isRotating) return;
            MouseX = obj.ReadValue<float>();
        }

        private void OnRotationY(InputAction.CallbackContext obj)
        {
            if (!isRotating) return;
            MouseY = obj.ReadValue<float>();
        }

        private void OnScrollY(InputAction.CallbackContext obj)
        {
            ScrollY = obj.ReadValue<float>();
        }

        #endregion
    }
}