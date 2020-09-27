using Core;
using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using Util;
using static Util.Utility;

namespace RPG.Control
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

        private int m_CurrentWaypointIndex;
        private float m_TimeSinceLastSawPlayer = Mathf.Infinity;
        private float m_TimeSinceLastWaypoint = Mathf.Infinity;
        private float m_TimeSinceAggrevated = Mathf.Infinity;
        private bool m_HasNotAggrevated = true;
        private GameObject m_Player;
        private Health m_PlayerHealth;
        private Health m_AiHealth;
        private Fighter m_Fighter;
        private Mover m_Mover;
        private ActionScheduler m_ActionScheduler;
        private LazyValue<Vector3> m_GuardPosition;

        public PatrolPath PatrolPath => patrolPath;

        private void Awake()
        {
            m_Player = GameObject.FindWithTag("Player");
            m_PlayerHealth = m_Player.GetComponent<Health>();
            m_AiHealth = GetComponent<Health>();
            m_Fighter = GetComponent<Fighter>();
            m_Mover = GetComponent<Mover>();
            m_ActionScheduler = GetComponent<ActionScheduler>();
            m_GuardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private void Start()
        {
            m_GuardPosition.ForceInit();
        }

        private void Update()
        {
            if (m_AiHealth.IsDead)
            {
                return;
            }

            if (IsAggrevated())
            {
                m_TimeSinceLastSawPlayer = 0;
                AttackBehaviour();
            }
            else if (m_TimeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                m_HasNotAggrevated = true;
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }

        private void UpdateTimers()
        {
            m_TimeSinceLastSawPlayer += Time.deltaTime;
            m_TimeSinceLastWaypoint += Time.deltaTime;
            m_TimeSinceAggrevated += Time.deltaTime;
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void AttackBehaviour()
        {
            m_Fighter.Attack(m_Player);
            AggrevateNearbyEnemies();
            if (!m_HasNotAggrevated) return;
            m_HasNotAggrevated = false;
            Aggrevate();
        }

        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                var ai = hit.collider.GetComponent<AIController>();

                if (ai == null || ai == this)
                {
                    continue;
                }

                ai.Aggrevate();
            }
        }

        private bool IsAggrevated()
        {
            return !m_PlayerHealth.IsDead && IsTargetInRange(transform, m_Player.transform, chaseDistance) ||
                   m_TimeSinceAggrevated < aggroCooldown;
        }

        private void SuspicionBehaviour()
        {
            m_ActionScheduler.CancelCurrentAction();
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = m_GuardPosition.Value;

            if (patrolPath != null)
            {
                if (IsAtWayPoint())
                {
                    if (dwellTime < m_TimeSinceLastWaypoint)
                    {
                        m_TimeSinceLastWaypoint = 0;
                        CycleWaypoint();
                    }
                }

                nextPosition = GetCurrentWaypoint().position;
            }

            m_Mover.StartMovement(nextPosition, patrolSpeedFraction);
        }

        private bool IsAtWayPoint()
        {
            return IsTargetInRange(transform, GetCurrentWaypoint(), wayPointTolerance);
        }

        private void CycleWaypoint()
        {
            m_CurrentWaypointIndex = patrolPath.GetNextIndex(m_CurrentWaypointIndex);
        }

        private Transform GetCurrentWaypoint()
        {
            return patrolPath.GetWayPoint(m_CurrentWaypointIndex);
        }

        private void Aggrevate()
        {
            m_TimeSinceAggrevated = 0;
        }
    }
}