using UnityEngine;
using UnityEngine.AI;

namespace Movement
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CharacterMover : MonoBehaviour
    {
        [SerializeField] private float maxNavPathLength = 40f;
        private NavMeshAgent m_NavMeshAgent;

        public Vector3 Velocity => m_NavMeshAgent.velocity;

        private void Awake()
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
        }

        public void Move(Vector3 destination, float speed)
        {
            m_NavMeshAgent.destination = destination;
            m_NavMeshAgent.speed = speed;
            m_NavMeshAgent.isStopped = false;
        }

        public bool CanMoveTo(Vector3 destination)
        {
            var path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
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
    }
}