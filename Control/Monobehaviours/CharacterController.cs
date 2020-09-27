using System;
using Core;
using Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Util;

namespace Control
{
    public class CharacterController : MonoBehaviour, IMouseInput, IMoveInput, IRotationInput, IInteractInput,
        IDashInput
    {
        #region Members

        [SerializeField] private GameManager gameManager;


        [Header("Mouse Input Commands")] [SerializeField]
        private InputMouseMoveCommand mouseMoveInput;

        [SerializeField] private InputMouseAttackCommand mouseAttackInput;

        [Header("Other Input Commands")] [SerializeField]
        private Command movementInput;

        [SerializeField] private Command analogRotationInput;
        [SerializeField] private Command dashInput;
        [SerializeField] private float raycastRadius = 1f;

        private readonly RaycastHit[] _hits = new RaycastHit[5];
        private PlayerInputActions _inputActions;
        private string _currentCursor;
        private GameObject _hoverTarget;

        [SerializeField] private bool isHoldingMouseButton;
        [SerializeField] private bool isPressingInteract;
        [SerializeField] private bool isDashing;
        [SerializeField] private Vector2 mousePosition;
        [SerializeField] private Vector3 moveDirection;
        [SerializeField] private Vector3 rotationDirection;


        public bool IsHoldingMouseButton => isHoldingMouseButton;

        public bool IsPressingInteract => isPressingInteract;

        public bool IsDashing => isDashing;

        public Vector3 MoveDirection => moveDirection;

        public Vector2 MousePosition => mousePosition;

        public Vector3 RotationDirection
        {
            get => rotationDirection;
            set => rotationDirection = value;
        }

        #endregion

        #region Lifecycle Hooks

        private void Awake()
        {
            _inputActions = new PlayerInputActions();
            _currentCursor = Constants.CursorTypes.None;
        }

        private void OnEnable()
        {
            _inputActions.Player.Enable();
            if (mouseMoveInput != null)
            {
                _inputActions.Player.MouseClick.performed += OnMouseInput;
                _inputActions.Player.MousePosition.performed += OnMousePosition;
            }

            if (movementInput != null) _inputActions.Player.Movement.performed += OnMoveInput;
            if (analogRotationInput != null) _inputActions.Player.AnalogAim.performed += OnAnalogAimInput;
            if (dashInput != null)
            {
                _inputActions.Player.Dash.performed += OnSkillInput;
                _inputActions.Player.Dash.canceled += OnSkillEnd;
            }
        }

        private void OnDisable()
        {
            if (mouseMoveInput != null)
            {
                _inputActions.Player.MouseClick.performed -= OnMouseInput;
                _inputActions.Player.MousePosition.performed -= OnMousePosition;
            }

            if (movementInput != null) _inputActions.Player.Movement.performed -= OnMoveInput;
            if (analogRotationInput != null) _inputActions.Player.AnalogAim.performed -= OnAnalogAimInput;
            if (dashInput != null)
            {
                _inputActions.Player.Dash.performed -= OnSkillInput;
                _inputActions.Player.Dash.canceled -= OnSkillEnd;
            }

            _inputActions.Player.Disable();
        }

        #endregion

        #region Input Callbacks

        private void OnMouseInput(InputAction.CallbackContext obj)
        {
            if (EventSystem.current && EventSystem.current.IsPointerOverGameObject()) return;
            var value = obj.ReadValue<float>();
            isHoldingMouseButton = value >= 0.15f;

            switch (_currentCursor)
            {
                case Constants.CursorTypes.Movement:
                    mouseMoveInput.Execute();
                    break;
                case Constants.CursorTypes.Combat:
                    mouseAttackInput.Execute(_hoverTarget);
                    break;
                default:
                    mouseMoveInput.Execute();
                    break;
            }
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

        private void OnMousePosition(InputAction.CallbackContext obj)
        {
            mousePosition = obj.ReadValue<Vector2>();
            if (!Cursor.visible || mouseMoveInput == null) return;
            if (InteractWithUI()) return;
            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;

            SetCursor(gameManager.Enums.CursorTypes[Constants.CursorTypes.None], Constants.CursorTypes.None);
        }

        private void OnSkillInput(InputAction.CallbackContext obj)
        {
            var value = obj.ReadValue<float>();
            isDashing = value >= 0.15f;

            dashInput.Execute();
        }

        private void OnSkillEnd(InputAction.CallbackContext context)
        {
            isDashing = false;
            dashInput.Complete();
        }

        #endregion

        #region Mouse Cursor Updates

        private void SetCursor(CursorType cursor, string type)
        {
            Cursor.SetCursor(cursor.Texture, cursor.Hotspot, CursorMode.Auto);
            _currentCursor = type;
        }

        private bool InteractWithUI()
        {
            if (!EventSystem.current || !EventSystem.current.IsPointerOverGameObject()) return false;
            SetCursor(gameManager.Enums.CursorTypes[Constants.CursorTypes.UI], Constants.CursorTypes.UI);
            return true;
        }

        private bool InteractWithMovement()
        {
            if (!mouseMoveInput.InteractWithMovement()) return false;
            SetCursor(gameManager.Enums.CursorTypes[Constants.CursorTypes.Movement], Constants.CursorTypes.Movement);
            return true;
        }

        private int RaycastAllSorted()
        {
            int size = Physics.SphereCastNonAlloc(mouseMoveInput.GetMouseRay(), raycastRadius, _hits);
            var distances = new float[size];
            for (var i = 0; i < size; i++)
            {
                distances[i] = _hits[i].distance;
            }

            Array.Sort(distances, _hits);
            return size;
        }

        private bool InteractWithComponent()
        {
            int size = RaycastAllSorted();

            for (var i = 0; i < size; i++)
            {
                var raycastable = _hits[i].transform.GetComponent<IRaycastable>();
                if (raycastable == null) return false;
                if (!raycastable.HandleRaycast(gameObject)) continue;
                SetCursor(raycastable.Cursor, raycastable.Type);
                _hoverTarget = _hits[i].transform.gameObject;
                return true;
            }

            return false;
        }

        #endregion
    }
}