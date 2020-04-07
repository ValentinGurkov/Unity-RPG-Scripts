using System;
using Core;
using Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Util;

namespace Control
{
    public class CharacterController : MonoBehaviour, IMouseInput, IMoveInput, IRotationInput, IInteractInput
    {
        #region Members

        [Header("Input Commands")] [SerializeField]
        private InputMouseCommand mouseInput;

        [SerializeField] private Command movementInput;
        [SerializeField] private Command analogRotationInput;
        [SerializeField] private Command mouseRotationInput;
        [SerializeField] private float raycastRadius = 1f;

        private readonly RaycastHit[] m_Hits = new RaycastHit[5];
        private PlayerInputActions m_InputActions;

        [SerializeField] private bool isHoldingMouseButton;
        [SerializeField] private bool isPressingInteract;
        [SerializeField] private Vector3 moveDirection;
        [SerializeField] private Vector3 rotationDirection;


        public bool IsHoldingMouseButton => isHoldingMouseButton;

        public bool IsPressingInteract => isPressingInteract;

        public Vector3 MoveDirection => moveDirection;

        public Vector3 RotationDirection
        {
            get => rotationDirection;
            set => rotationDirection = value;
        }

        #endregion

        #region Lifecycle Hooks

        private void Awake()
        {
            m_InputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            m_InputActions.Player.Enable();
            if (mouseInput != null) m_InputActions.Player.MouseMove.performed += OnMouseInput;
            if (movementInput != null) m_InputActions.Player.Movement.performed += OnMoveInput;
            if (analogRotationInput != null) m_InputActions.Player.AnalogAim.performed += OnAnalogAimInput;
        }

        private void OnDisable()
        {
            if (mouseInput != null) m_InputActions.Player.MouseMove.performed -= OnMouseInput;
            if (movementInput != null) m_InputActions.Player.Movement.performed -= OnMoveInput;
            if (analogRotationInput != null) m_InputActions.Player.AnalogAim.performed -= OnAnalogAimInput;
            m_InputActions.Player.Disable();
        }

        private void Update()
        {
            if (!Cursor.visible || mouseInput == null) return;
            if (InteractWithUI()) return;
            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;

            SetCursor(GameManager.CursorTypes[Constants.CursorTypes.None] as CursorType);
        }

        #endregion

        #region Input Callbacks

        private void OnMouseInput(InputAction.CallbackContext obj)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            var value = obj.ReadValue<float>();
            isHoldingMouseButton = value >= 0.15f;

            mouseInput.Execute();
        }

        private void OnMoveInput(InputAction.CallbackContext obj)
        {
            var value = obj.ReadValue<Vector2>();
            moveDirection = new Vector3(value.x, 0, value.y);
            movementInput.Execute();
        }

        private void OnAnalogAimInput(InputAction.CallbackContext obj)
        {
            var value = obj.ReadValue<Vector2>();
            rotationDirection = new Vector3(value.x, 0, value.y);
            analogRotationInput.Execute();
        }

        #endregion

        #region Mouse Cursor Updates

        private static void SetCursor(CursorType type)
        {
            Cursor.SetCursor(type.Texture, type.Hotspot, CursorMode.Auto);
        }

        private static bool InteractWithUI()
        {
            if (!EventSystem.current.IsPointerOverGameObject()) return false;
            SetCursor(GameManager.CursorTypes[Constants.CursorTypes.UI] as CursorType);
            return true;
        }

        private bool InteractWithMovement()
        {
            if (!mouseInput.InteractWithMovement()) return false;
            SetCursor(GameManager.CursorTypes[Constants.CursorTypes.Movement] as CursorType);
            return true;
        }

        private int RaycastAllSorted()
        {
            int size = Physics.SphereCastNonAlloc(mouseInput.GetMouseRay(), raycastRadius, m_Hits);
            var distances = new float[size];
            for (var i = 0; i < size; i++)
            {
                distances[i] = m_Hits[i].distance;
            }

            Array.Sort(distances, m_Hits);
            return size;
        }

        private bool InteractWithComponent()
        {
            int size = RaycastAllSorted();

            for (var i = 0; i < size; i++)
            {
                var raycastable = m_Hits[i].transform.GetComponent<IRaycastable>();
                if (raycastable == null) return false;
                if (!raycastable.HandleRaycast(gameObject)) continue;
                SetCursor(raycastable.Cursor);
                return true;
            }

            return false;
        }

        #endregion
    }
}