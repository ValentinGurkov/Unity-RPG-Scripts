using System.Collections;
using Core;
using Saving;
using UnityEngine;
using UnityEngine.AI;
using Util;

namespace Movement
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CharacterMoverNavMesh : MonoBehaviour, IAction, ISaveable
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
        private NavMeshAgent _navMeshAgent;
        private Coroutine _speedRoutine;
        private Transform _transform;
        private WaitForSeconds _dashWait;
        private float _originalAcceleration;
        private bool _isDashing;

        public Vector3 Velocity => _navMeshAgent.velocity;

        private void Awake()
        {
            _actionScheduler = GetComponent<ActionScheduler>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _transform = transform;
            _originalAcceleration = _navMeshAgent.acceleration;
            _dashWait = new WaitForSeconds(dashDuration);
        }

        public void StartMovement(Vector3 destination, float speedFraction)
        {
            _actionScheduler.StartAction(this);
            Move(destination, speedFraction);
        }

        public void Move(Vector3 destination, float speedFraction)
        {
            Cancel();
            _navMeshAgent.acceleration = _originalAcceleration;
            _navMeshAgent.isStopped = false;
            _navMeshAgent.destination = destination;
            if (_speedRoutine != null) StopCoroutine(_speedRoutine);
            _speedRoutine = StartCoroutine(SetSpeed(speedFraction));
        }

        public void Dash(Vector3 destination)
        {
            Cancel();
            _actionScheduler.StartAction(this);
            if (_speedRoutine != null)
            {
                StopCoroutine(_speedRoutine);
                _speedRoutine = null;
            }

            ;
            if (_isDashing) return;
            _navMeshAgent.isStopped = false;
            StartCoroutine(DashRoutine(destination));
        }

        public void Cancel()
        {
            if (_speedRoutine != null)
            {
                StopCoroutine(_speedRoutine);
                _speedRoutine = null;
            }

            _navMeshAgent.isStopped = true;
            _navMeshAgent.acceleration = float.MaxValue;
        }

        private IEnumerator DashRoutine(Vector3 destination)
        {
            _isDashing = true;
            _navMeshAgent.acceleration = dashAcceleration;
            _navMeshAgent.speed = dashSpeed;
            Vector3 position = _transform.position;
            _navMeshAgent.destination = position + (destination - position).normalized * dashDistance;
            yield return _dashWait;
            _isDashing = false;
        }

        public bool CanMoveTo(Vector3 destination)
        {
            var path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(_transform.position, destination, NavMesh.AllAreas, path);
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


        private IEnumerator SetSpeed(float speedFraction)
        {
            while (_navMeshAgent.remainingDistance >= _navMeshAgent.stoppingDistance)
            {
                yield return _navMeshAgent.speed = Mathf.Lerp(speed, _navMeshAgent.velocity.magnitude, Time.deltaTime) * speedModifier *
                                                   Mathf.Clamp01(speedFraction);
            }

            _speedRoutine = null;
        }

        public object CaptureState()
        {
            return new SerializableVector3(_transform.position);
        }

        public void RestoreState(object state)
        {
            var position = (SerializableVector3) state;
            _navMeshAgent.enabled = false;
            _transform.position = position.ToVector();
            _navMeshAgent.enabled = true;
            _actionScheduler.CancelCurrentAction();
        }
    }
}