using System.Collections;
using Movement;
using UnityEngine;

namespace Core
{
    // <summary>
    /// Moving on NavMesh using arrow keys
    /// </summary>
    [RequireComponent(typeof(CharacterMoverNavMesh))]
    public class InputMoveNavMeshCommand : Command
    {
        [SerializeField] private float directionModifier = 1f;

        private IMoveInput _moveInput;
        private CharacterMoverNavMesh _mover;
        private Transform _transform;
        private Camera _camera;
        private Coroutine _moveRoutine;

        private void Awake()
        {
            _moveInput = GetComponent<IMoveInput>();
            _mover = GetComponent<CharacterMoverNavMesh>();
            _transform = transform;
            _camera = Camera.main;
        }

        public override void Execute()
        {
            if (_moveRoutine == null) _moveRoutine = StartCoroutine(Move());
        }

        private IEnumerator Move()
        {
            while (_moveInput.MoveDirection != Vector3.zero)
            {
                Vector3 screenPos = _camera.WorldToScreenPoint(
                    _transform.position + _moveInput.MoveDirection * directionModifier);
                Ray ray = _camera.ScreenPointToRay(screenPos);
                if (Physics.Raycast(ray, out RaycastHit hit, 50f))
                {
                    _mover.Move(hit.point);
                }

                yield return null;
            }

            _moveRoutine = null;
        }
    }
}