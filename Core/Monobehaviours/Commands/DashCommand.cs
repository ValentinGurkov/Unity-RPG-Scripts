using Movement;
using UnityEngine;

namespace Core
{
    public class DashCommand : Command
    {
        private IMouseInput _mouseInput;
        private CharacterMoverNavMesh _mover;
        private Camera _camera;

        private void Awake()
        {
            _mouseInput = GetComponent<IMouseInput>();
            _mover = GetComponent<CharacterMoverNavMesh>();
            _camera = Camera.main;
        }

        public override void Execute()
        {
            Ray ray = GetMouseRay();
            if (Physics.Raycast(ray, out RaycastHit lookDirectionHit, 50f, LayerMask.GetMask("Ground")))
            {
                _mover.Dash(lookDirectionHit.point);
            }
        }

        private Ray GetMouseRay()
        {
            return _camera.ScreenPointToRay(_mouseInput.MousePosition);
        }
    }
}