using System.Collections;
using Core;
using UnityEngine;
using UnityEngine.AI;

namespace Movement
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CharacterMoverNavMesh : MonoBehaviour, IAction
    {
        [SerializeField] private float speed = 6f;
        [SerializeField] private float speedModifier = 1f;
        [SerializeField] private float maxNavPathLength = 40f;

        [Header("Dash Settings")] [SerializeField]
        private float dashSpeed = 25f;

        [SerializeField] private float dashAcceleration = 1000f;
        [SerializeField] private float dashDistance = 10f;
        [SerializeField] private float dashDuration = 0.2f;
        private ActionScheduler _actionScheduler;
        private NavMeshAgent m_NavMeshAgent;
        private Coroutine m_SpeedRoutine;
        private Transform m_Transform;
        private WaitForSeconds m_DashWait;
        private float m_OriginalAcceleration;
        private bool m_IsDashing;

        public Vector3 Velocity => m_NavMeshAgent.velocity;

        private void Awake()
        {
            _actionScheduler = GetComponent<ActionScheduler>();
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_Transform = transform;
            m_OriginalAcceleration = m_NavMeshAgent.acceleration;
            m_DashWait = new WaitForSeconds(dashDuration);
        }

        public void StartMovement(Vector3 destination)
        {
            _actionScheduler.StartAction(this);
            Move(destination);
        }

        public void Move(Vector3 destination)
        {
            Cancel();
            m_NavMeshAgent.acceleration = m_OriginalAcceleration;
            m_NavMeshAgent.isStopped = false;
            m_NavMeshAgent.destination = destination;
            if (m_SpeedRoutine != null) StopCoroutine(m_SpeedRoutine);
            m_SpeedRoutine = StartCoroutine(SetSpeed());
        }

        public void Dash(Vector3 destination)
        {
            Cancel();
            _actionScheduler.StartAction(this);
            if (m_SpeedRoutine != null) StopCoroutine(m_SpeedRoutine);
            if (m_IsDashing) return;
            m_NavMeshAgent.isStopped = false;
            StartCoroutine(DashRoutine(destination));
        }

        public void Cancel()
        {
            m_NavMeshAgent.isStopped = true;
        }

        private IEnumerator DashRoutine(Vector3 destination)
        {
            m_IsDashing = true;
            m_NavMeshAgent.acceleration = dashAcceleration;
            m_NavMeshAgent.speed = dashSpeed;
            Vector3 position = m_Transform.position;
            m_NavMeshAgent.destination = position + (destination - position).normalized * dashDistance;
            yield return m_DashWait;
            m_IsDashing = false;
        }

        public bool CanMoveTo(Vector3 destination)
        {
            var path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(m_Transform.position, destination, NavMesh.AllAreas, path);
            return hasPath && path.status == NavMeshPathStatus.PathComplete && GetPathLength(path) <= maxNavPathLength;
        }

        private static float GetPathLength(NavMeshPath path)
        {
            float total = 0;

            if (path.corners.Length < 2) return 0;

            for (var i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return total;
        }


        private IEnumerator SetSpeed()
        {
            while (m_NavMeshAgent.remainingDistance >= m_NavMeshAgent.stoppingDistance)
            {
                yield return m_NavMeshAgent.speed =
                    Mathf.Lerp(speed, m_NavMeshAgent.velocity.magnitude, Time.deltaTime) * speedModifier;
            }

            m_SpeedRoutine = null;
        }
    }
}