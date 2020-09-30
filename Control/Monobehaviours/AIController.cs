using Attributes;
using Combat;
using Core;
using Movement;
using UnityEngine;
using Util;
using static Util.Utility;

namespace Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] private float suspicionTime = 5f;
        [SerializeField] private float aggroCooldown = 5f;
        [SerializeField] private float shoutDistance = 5f;
        [SerializeField] private float dwellTime = 3f;
        [SerializeField] private float wayPointTolerance = 1f;
        [Range(0, 1)] [SerializeField] private float patrolSpeedFraction = 0.2f;
        [SerializeField] private PatrolPath patrolPath;

        private int _currentWaypointIndex;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeSinceLastWaypoint = Mathf.Infinity;
        private float _timeSinceAggravated = Mathf.Infinity;
        private bool _hasNotAggravated = true;
        private GameObject _player;
        private HealthNew _playerHealth;
        private HealthNew _aiHealth;
        private FighterNew _fighter;
        private CharacterMoverNavMesh _mover;
        private ActionScheduler _actionScheduler;
        private LazyValue<Vector3> _guardPosition;

        public PatrolPath PatrolPath => patrolPath;

        private void Awake()
        {
            _player = GameObject.FindWithTag("Player");
            _playerHealth = _player.GetComponent<HealthNew>();
            _aiHealth = GetComponent<HealthNew>();
            _fighter = GetComponent<FighterNew>();
            _mover = GetComponent<CharacterMoverNavMesh>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private void Start()
        {
            _guardPosition.ForceInit();
        }

        private void Update()
        {
            if (_aiHealth.IsDead)
            {
                return;
            }

            if (IsAggravated())
            {
                _timeSinceLastSawPlayer = 0;
                AttackBehaviour();
            }
            else if (_timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                _hasNotAggravated = true;
                PatrolBehaviour();
            }

            UpdateTimers();
        }
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
#endif
        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceLastWaypoint += Time.deltaTime;
            _timeSinceAggravated += Time.deltaTime;
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void AttackBehaviour()
        {
            _fighter.Attack(_player);
            AggravateNearbyEnemies();
            if (!_hasNotAggravated) return;
            _hasNotAggravated = false;
            Aggravate();
        }

        private void AggravateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                var ai = hit.collider.GetComponent<AIController>();

                if (ai == null || ai == this)
                {
                    continue;
                }

                ai.Aggravate();
            }
        }

        private bool IsAggravated()
        {
            return !_playerHealth.IsDead && IsTargetInRange(transform, _player.transform, chaseDistance) ||
                   _timeSinceAggravated < aggroCooldown;
        }

        private void SuspicionBehaviour()
        {
            _actionScheduler.CancelCurrentAction();
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = _guardPosition.Value;

            if (patrolPath != null)
            {
                if (IsAtWayPoint())
                {
                    if (dwellTime < _timeSinceLastWaypoint)
                    {
                        _timeSinceLastWaypoint = 0;
                        CycleWaypoint();
                    }
                }

                nextPosition = GetCurrentWaypoint().position;
            }

            _mover.StartMovement(nextPosition, patrolSpeedFraction);
        }

        private bool IsAtWayPoint()
        {
            return IsTargetInRange(transform, GetCurrentWaypoint(), wayPointTolerance);
        }

        private void CycleWaypoint()
        {
            _currentWaypointIndex = patrolPath.GetNextIndex(_currentWaypointIndex);
        }

        private Transform GetCurrentWaypoint()
        {
            return patrolPath.GetWayPoint(_currentWaypointIndex);
        }

        private void Aggravate()
        {
            _timeSinceAggravated = 0;
        }
    }
}