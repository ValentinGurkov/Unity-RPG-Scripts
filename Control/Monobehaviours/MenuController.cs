using Input;
using Saving;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Control
{
    public class MenuController : MonoBehaviour
    {
        private PlayerInputActions _inputActions;
        private SavingSystem _savingSystem;
        private const string DefaultSaveFile = "save";

        private void Awake()
        {
            _inputActions = new PlayerInputActions();
            _savingSystem = GetComponent<SavingSystem>();
        }

        private void OnEnable()
        {
            if (_savingSystem == null) return;
            _inputActions.Menu.Enable();
            _inputActions.Menu.Load.performed += Load;
            _inputActions.Menu.Save.performed += Save;
            _inputActions.Menu.DeleteSave.performed += Delete;
        }

        private void OnDisable()
        {
            if (_savingSystem == null) return;
            _inputActions.Menu.Enable();
            _inputActions.Menu.Load.performed -= Load;
            _inputActions.Menu.Save.performed -= Save;
            _inputActions.Menu.DeleteSave.performed -= Delete;
        }

        private void Save(InputAction.CallbackContext obj)
        {
            _savingSystem.Save(DefaultSaveFile);
        }

        private void Load(InputAction.CallbackContext obj)
        {
            _savingSystem.Load(DefaultSaveFile);
        }

        private void Delete(InputAction.CallbackContext obj)
        {
            _savingSystem.Delete(DefaultSaveFile);
        }
    }
}