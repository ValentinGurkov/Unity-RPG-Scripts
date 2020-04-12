using System.Collections;
using Movement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Core
{
    public class InputMouseCommand : Command
    {
        [SerializeField] private LayerMask clickableLayer;
        [SerializeField] private float navMeshMaxProjectionDistance = 1f;
        [SerializeField] private Transform indicator;

        private readonly WaitForSeconds m_IndicatorHideDelay = new WaitForSeconds(0.2f);
        private IMouseInput m_MouseInput;
        private Coroutine m_MoveCoroutine;
        private Mouse m_Mouse;
        private Camera m_Camera;
        private CharacterMover m_Mover;

        private void Awake()
        {
            m_MouseInput = GetComponent<IMouseInput>();
            m_Mover = GetComponent<CharacterMover>();
        }

        private void Start()
        {
            m_Mouse = Mouse.current;
            m_Camera = Camera.main;
        }

        public override void Execute()
        {
            if (m_MoveCoroutine == null) m_MoveCoroutine = StartCoroutine(Move());
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

            while (m_MouseInput.IsHoldingMouseButton)
            {
                ray = GetMouseRay();
                if (Physics.Raycast(ray, out hit, 50, clickableLayer.value))
                {
                    m_Mover.Move(hit.point);
                }

                yield return null;
            }

            m_MoveCoroutine = null;
        }

        private IEnumerator HideIndicator()
        {
            yield return m_IndicatorHideDelay;
            indicator.gameObject.SetActive(false);
        }

        public Ray GetMouseRay()
        {
            return m_Camera.ScreenPointToRay(m_MouseInput.MousePosition);
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
            return hasHit && m_Mover.CanMoveTo(target);
        }
    }
}