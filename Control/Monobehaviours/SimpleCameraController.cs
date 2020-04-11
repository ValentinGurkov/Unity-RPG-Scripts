using Core;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Control
{
    public class SimpleCameraController : MonoBehaviour, IMoveInput, ISprintInput, IRotationTrigger, IScrollYInput,
        IRotationInput, ICameraState, ILerpTime
    {
        #region Members

        [SerializeField] private Command movementInput;
        [SerializeField] private Command rotationInput;

        [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
        [SerializeField]
        private float positionLerpTime = 0.2f;

        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
        [SerializeField]
        private float rotationLerpTime = 0.01f;


        [SerializeField] private Vector3 moveDirection;
        [SerializeField] private Vector3 rotationDirection;
        [SerializeField] private bool isSprinting;
        [SerializeField] private bool isRotating;


        private PlayerInputActions m_InputActions;

        public float PositionLerpTime => positionLerpTime;
        public float RotationLerpTime => rotationLerpTime;
        public Vector3 MoveDirection => moveDirection;
        public bool IsSprinting => isSprinting;
        public bool IsRotating => isRotating;
        public float ScrollY { get; private set; }

        public Vector3 RotationDirection
        {
            get => rotationDirection;
            set => rotationDirection = value;
        }

        public CameraState TargetCameraState { get; } = new CameraState();

        public CameraState InterpolatingCameraState { get; } = new CameraState();

        #endregion

        #region Lifecycle hooks

        private void Awake()
        {
            m_InputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            TargetCameraState.SetFromTransform(transform);
            InterpolatingCameraState.SetFromTransform(transform);
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

            m_InputActions.Camera.RotationX.canceled += ctx => rotationDirection.x = 0;
            m_InputActions.Camera.RotationY.canceled += ctx => rotationDirection.z = 0;
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
            rotationDirection.x = obj.ReadValue<float>();
        }

        private void OnRotationY(InputAction.CallbackContext obj)
        {
            if (!isRotating) return;
            rotationDirection.z = obj.ReadValue<float>();
        }

        private void OnScrollY(InputAction.CallbackContext obj)
        {
            ScrollY = obj.ReadValue<float>();
        }

        #endregion
    }
}