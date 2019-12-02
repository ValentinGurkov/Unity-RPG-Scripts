using System;
using RPG.Attributes;
using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement {

    public class Mover : MonoBehaviour, IAction, ISaveable {
        [SerializeField] private float maxSpeed = 6f;
        [SerializeField] private float maxNavPathLength = 40f;
        [SerializeField] private float interactStoppingDistance = 3f;
        private float originalStoppingDistance = 0;
        private Action callbackOnReachingDestination = null;
        private NavMeshAgent navMeshAgent;
        private Animator animator;
        private ActionScheduler actionScheduler;
        private Health health;

        private void Awake() {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
            health = GetComponent<Health>();

            originalStoppingDistance = navMeshAgent.stoppingDistance;
        }

        private void Update() {
            navMeshAgent.enabled = !health.IsDead;
            UpdateAnimator();

            if (callbackOnReachingDestination != null && !navMeshAgent.pathPending) {
                navMeshAgent.stoppingDistance = interactStoppingDistance;
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
                    if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f) {
                        Cancel();
                        callbackOnReachingDestination();
                        callbackOnReachingDestination = null;
                        navMeshAgent.stoppingDistance = originalStoppingDistance;
                    }
                }
            }
        }

        private float GetPathLength(NavMeshPath path) {
            float total = 0;

            if (path.corners.Length < 2) {
                return 0;
            }

            for (int i = 0; i < path.corners.Length - 1; i++) {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return total;
        }

        private void UpdateAnimator() {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = Math.Abs(localVelocity.z);
            animator.SetFloat("forwardSpeed", speed);
        }

        public bool CanMoveTo(Vector3 destination) {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath || path.status != NavMeshPathStatus.PathComplete || GetPathLength(path) > maxNavPathLength) {
                return false;
            };
            return true;
        }

        public void MoveTo(Vector3 destination, float speedFraction) {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        public void InteractWithTarget(Action callback) {
            callbackOnReachingDestination = callback;
        }

        public Mover StartMovement(Vector3 destination, float speedFraction) {
            actionScheduler.StartAction(this);
            MoveTo(destination, speedFraction);

            return this;
        }

        public void Cancel() {
            navMeshAgent.isStopped = true;
        }

        public object CaptureState() {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state) {
            SerializableVector3 position = (SerializableVector3) state;
            navMeshAgent.enabled = false;
            transform.position = position.ToVector();
            navMeshAgent.enabled = true;
            actionScheduler.CancelCurrentAction();
        }
    }
}
