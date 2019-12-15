using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Util;
using UnityEngine;
using static RPG.Util.Utility;

namespace RPG.Control {

    public class AIController : MonoBehaviour {
        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] private float suspicionTime = 5f;
        [SerializeField] private float aggroCooldown = 5f;
        [SerializeField] private float shoutDistance = 5f;
        [SerializeField] private float dwellTime = 3f;
        [SerializeField] private float wayPointTolerance = 1f;
        [Range(0, 1)][SerializeField] private float patrolSpeedFraction = 0.2f;
        [SerializeField] private PatrolPath patrolPath;

        private int currentWaypointIndex = 0;
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private float timeSinceLastWaypoint = Mathf.Infinity;
        private float timeSinceAggrevated = Mathf.Infinity;
        private bool hasNotAggrevated = true;
        private GameObject player;
        private Health playerHealth;
        private Health AIHealth;
        private Fighter fighter;
        private Mover mover;
        private ActionScheduler actionScheduler;
        private LazyValue<Vector3> guardPosition;

        public PatrolPath PatrolPath => patrolPath;

        private void Awake() {
            player = GameObject.FindWithTag("Player");
            playerHealth = player.GetComponent<Health>();
            AIHealth = GetComponent<Health>();
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            actionScheduler = GetComponent<ActionScheduler>();
            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private void Start() {
            guardPosition.ForceInit();
        }

        private void Update() {
            if (AIHealth.IsDead) {
                return;
            }
            if (IsAggrevated()) {
                timeSinceLastSawPlayer = 0;
                AttackBehaviour();
            } else if (timeSinceLastSawPlayer < suspicionTime) {
                SuspicionBehaviour();
            } else {
                hasNotAggrevated = true;
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }

        private void UpdateTimers() {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceLastWaypoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        private Vector3 GetGuardPosition() {
            return transform.position;
        }

        private void AttackBehaviour() {
            fighter.Attack(player);
            AggrevateNearbyEnemies();
            if (hasNotAggrevated) {
                hasNotAggrevated = false;
                Aggrevate();
            }
        }

        private void AggrevateNearbyEnemies() {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (RaycastHit hit in hits) {
                AIController ai = hit.collider.GetComponent<AIController>();

                if (ai == null || ai == this) {
                    continue;
                }
                ai.Aggrevate();
            }
        }

        private bool IsAggrevated() {
            return !playerHealth.IsDead && IsTargetInRange(transform, player.transform, chaseDistance) || timeSinceAggrevated < aggroCooldown;
        }

        private void SuspicionBehaviour() {
            actionScheduler.CancelCurrentAction();
        }

        private void PatrolBehaviour() {
            Vector3 nextPosition = guardPosition.value;

            if (patrolPath != null) {
                if (IsAtWayPoint()) {
                    if (dwellTime < timeSinceLastWaypoint) {
                        timeSinceLastWaypoint = 0;
                        CycleWaypoint();
                    }
                }

                nextPosition = GetCurrentWaypoint().position;
            }

            mover.StartMovement(nextPosition, patrolSpeedFraction);
        }

        private bool IsAtWayPoint() {
            return IsTargetInRange(transform, GetCurrentWaypoint(), wayPointTolerance);
        }

        private void CycleWaypoint() {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private Transform GetCurrentWaypoint() {
            return patrolPath.GetWayPoint(currentWaypointIndex);
        }

        public void Aggrevate() {
            timeSinceAggrevated = 0;
        }
    }
}
