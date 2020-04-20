using System.Collections;
using Movement;
using UnityEngine;

namespace Core
{
    [RequireComponent(typeof(CharacterMoverNavMesh))]
    public class InputMoveNavMeshCommand : Command
    {
        [SerializeField] private float directionModifier = 1f;

        private IMoveInput m_MoveInput;
        private CharacterMoverNavMesh m_Mover;
        private Transform m_Transform;
        private Camera m_Camera;
        private Coroutine m_MoveRoutine;

        private void Awake()
        {
            m_MoveInput = GetComponent<IMoveInput>();
            m_Mover = GetComponent<CharacterMoverNavMesh>();
            m_Transform = transform;
            m_Camera = Camera.main;
        }

        public override void Execute()
        {
            if (m_MoveRoutine == null) m_MoveRoutine = StartCoroutine(Move());
        }

        private IEnumerator Move()
        {
            while (m_MoveInput.MoveDirection != Vector3.zero)
            {
                Vector3 screenPos = m_Camera.WorldToScreenPoint(
                    m_Transform.position + m_MoveInput.MoveDirection * directionModifier);
                Ray ray = m_Camera.ScreenPointToRay(screenPos);
                if (Physics.Raycast(ray, out RaycastHit hit, 50f))
                {
                    m_Mover.Move(hit.point);
                }

                yield return null;
            }

            m_MoveRoutine = null;
        }
    }
}