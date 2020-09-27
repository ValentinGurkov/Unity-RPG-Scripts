using System.Collections;
using Movement;
using UnityEngine;
using UnityEngine.AI;
using Logger = Util.Logger;

namespace Core
{
    [RequireComponent(typeof(CharacterMoverNavMesh))]
    public class InputMouseMoveCommand : Command
    {
        [SerializeField] private LayerMask clickableLayer;
        [SerializeField] private float navMeshMaxProjectionDistance = 1f;
        [SerializeField] private Transform indicator;

        private readonly WaitForSeconds _indicatorHideDelay = new WaitForSeconds(0.2f);
        private IMouseInput _mouseInput;
        private IDashInput _dashInput;
        private Coroutine _moveCoroutine;
        private Camera _camera;
        private CharacterMoverNavMesh _mover;

        private void Awake()
        {
            _dashInput = GetComponent<IDashInput>();
            _mouseInput = GetComponent<IMouseInput>();
            _mover = GetComponent<CharacterMoverNavMesh>();
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        public override void Execute()
        {
            if (_moveCoroutine == null) _moveCoroutine = StartCoroutine(Move());
        }

        private IEnumerator Move()
        {
            Ray ray = GetMouseRay();
            if (Physics.Raycast(ray, out RaycastHit hit, 50, clickableLayer.value))
            {
                indicator.position = hit.point;
                indicator.gameObject.SetActive(true);
                StartCoroutine(HideIndicator());
            }

            while (_mouseInput.IsHoldingMouseButton && !_dashInput.IsDashing)
            {
                ray = GetMouseRay();
                Logger.Log("check move");
                if (Physics.Raycast(ray, out hit, 50, clickableLayer.value))
                {
                    Logger.Log("moving");
                    _mover.StartMovement(hit.point);
                }

                yield return null;
            }

            _moveCoroutine = null;
        }

        private IEnumerator HideIndicator()
        {
            yield return _indicatorHideDelay;
            indicator.gameObject.SetActive(false);
        }

        public Ray GetMouseRay()
        {
            return _camera.ScreenPointToRay(_mouseInput.MousePosition);
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            bool hasHit = Physics.Raycast(GetMouseRay(), out RaycastHit hit);

            if (!hasHit) return false;

            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out NavMeshHit navMeshHit,
                navMeshMaxProjectionDistance,
                NavMesh.AllAreas);

            if (!hasCastToNavMesh) return false;

            target = navMeshHit.position;

            return true;
        }

        public bool InteractWithMovement()
        {
            bool hasHit = RaycastNavMesh(out Vector3 target);
            return hasHit && _mover.CanMoveTo(target);
        }
    }
}