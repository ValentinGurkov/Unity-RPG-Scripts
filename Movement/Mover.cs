using System;
using Core;
using RPG.Attributes;
using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;
using Util;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] private float maxSpeed = 6f;
        [SerializeField] private float maxNavPathLength = 40f;
        [SerializeField] private float interactStoppingDistance = 3f;
        private float m_OriginalStoppingDistance;
        private Action m_CallbackOnReachingDestination;
        private NavMeshAgent m_NavMeshAgent;
        private Animator m_Animator;
        private ActionScheduler m_ActionScheduler;
        private Health m_Health;
        private static readonly int s_ForwardSpeed = Animator.StringToHash("forwardSpeed");

        private void Awake()
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponent<Animator>();
            m_ActionScheduler = GetComponent<ActionScheduler>();
            m_Health = GetComponent<Health>();

            m_OriginalStoppingDistance = m_NavMeshAgent.stoppingDistance;
        }

        private void Update()
        {
            m_NavMeshAgent.enabled = !m_Health.IsDead;
            UpdateAnimator();

            if (m_CallbackOnReachingDestination == null || m_NavMeshAgent.pathPending) return;
            m_NavMeshAgent.stoppingDistance = interactStoppingDistance;
            if (!(m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance)) return;
            if (m_NavMeshAgent.hasPath && !Mathf.Approximately(m_NavMeshAgent.velocity.sqrMagnitude, 0)) return;
            Cancel();
            m_CallbackOnReachingDestination();
            m_CallbackOnReachingDestination = null;
            m_NavMeshAgent.stoppingDistance = m_OriginalStoppingDistance;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;

            if (path.corners.Length < 2)
            {
                return 0;
            }

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return total;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = m_NavMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = Math.Abs(localVelocity.z);
            m_Animator.SetFloat(s_ForwardSpeed, speed);
        }

        public bool CanMoveTo(Vector3 destination)
        {
            var path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath || path.status != NavMeshPathStatus.PathComplete || GetPathLength(path) > maxNavPathLength)
            {
                return false;
            }

            ;
            return true;
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            m_NavMeshAgent.destination = destination;
            m_NavMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            m_NavMeshAgent.isStopped = false;
        }

        public void InteractWithTarget(Action callback)
        {
            m_CallbackOnReachingDestination = callback;
        }

        public Mover StartMovement(Vector3 destination, float speedFraction)
        {
            m_ActionScheduler.StartAction(this);
            MoveTo(destination, speedFraction);

            return this;
        }

        public void Cancel()
        {
            m_NavMeshAgent.isStopped = true;
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            var position = (SerializableVector3) state;
            m_NavMeshAgent.enabled = false;
            transform.position = position.ToVector();
            m_NavMeshAgent.enabled = true;
            m_ActionScheduler.CancelCurrentAction();
        }
    }
}