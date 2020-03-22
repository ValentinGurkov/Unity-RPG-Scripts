using System;
using Core;
using Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Util;

namespace Control
{
    public class CharacterController : MonoBehaviour, IMouseInput
    {
        #region Members

        [SerializeField] private InputMouseCommand mouseMoveInput;
        [SerializeField] private float raycastRadius = 1f;

        private readonly RaycastHit[] m_Hits = new RaycastHit[5];
        private PlayerInputActions m_InputActions;

        public bool IsHoldingMouseButton { get; private set; }

        #endregion

        #region Lifecycle Hooks

        private void Awake()
        {
            m_InputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            m_InputActions.Player.Enable();
            if (mouseMoveInput != null) m_InputActions.Player.MouseMove.performed += OnMouseMoveInput;
        }

        private void OnDisable()
        {
            if (mouseMoveInput != null) m_InputActions.Player.MouseMove.performed -= OnMouseMoveInput;
            m_InputActions.Player.Disable();
        }

        private void Update()
        {
            if (InteractWithUI()) return;
            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;

            SetCursor(GameManager.CursorTypes[Constants.CursorTypes.None] as CursorType);
        }

        #endregion

        #region Input Callbacks

        private void OnMouseMoveInput(InputAction.CallbackContext obj)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            var value = obj.ReadValue<float>();
            IsHoldingMouseButton = value >= 0.15f;

            mouseMoveInput.Execute();
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
            if (!mouseMoveInput.InteractWithMovement()) return false;
            SetCursor(GameManager.CursorTypes[Constants.CursorTypes.Movement] as CursorType);
            return true;
        }

        private int RaycastAllSorted()
        {
            int size = Physics.SphereCastNonAlloc(mouseMoveInput.GetMouseRay(), raycastRadius, m_Hits);
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