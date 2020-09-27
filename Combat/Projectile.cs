using System;
using System.Collections;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;
using Util;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 1f;
        [SerializeField] private bool isHoming;
        [SerializeField] private float projectileTTL = 10f;
        [SerializeField] private float afterHitTTL = 0.2f;
        [SerializeField] private GameObject hitEffect;
        [SerializeField] private GameObject[] destroyAfterHit;
        [SerializeField] private UnityEvent onHit;
        [SerializeField] private UnityEvent onLaunch;
        [SerializeField] private string m_PoolTag;
        private const string TAG_CINEMATIC = "Cinematic";
        private const string TAG_PICKUP = "Pickup";
        private float m_Damage;
        private Health m_Target;
        private GameObject m_Instigator;
        private ObjectPooler m_Pooler;
        private Transform m_Transform;

        /// <summary>
        /// Can update UI when damaging an enemy (e.g. update enemy health on HUD)
        /// </summary>
        private Action m_UpdateUi;

        private float m_OriginalSpeed;

        private void Awake()
        {
            m_OriginalSpeed = speed;
            m_Transform = transform;
        }

        private void Update()
        {
            if (m_Target == null)
            {
                return;
            }

            if (isHoming && !m_Target.IsDead)
            {
                m_Transform.LookAt(GetAimLocation());
            }

            m_Transform.Translate(Vector3.forward * (speed * Time.deltaTime));
            StartCoroutine(ReturnToPoolWithDelay(projectileTTL));
        }

        private void OnTriggerEnter(Collider other)
        {
            // make projectiles go trough cinematic triggers and items on the ground
            if (other.gameObject.CompareTag(TAG_CINEMATIC) || other.gameObject.CompareTag(TAG_PICKUP))
            {
                return;
            }

            foreach (GameObject toDestroy in destroyAfterHit)
            {
                toDestroy.SetActive(true);
            }

            var targetHealth = other.GetComponent<Health>();

            if (targetHealth == null)
            {
                // to disallow friendly fire -  || targetHealth != target
                ReturnToPool();
                return;
            }

            if (targetHealth.IsDead)
            {
                StartCoroutine(ReturnToPoolWithDelay(projectileTTL));
                return;
            }

            speed = 0;

            if (hitEffect != null)
            {
                Vector3 position = m_Transform.position;
                Vector3 closestPoint = !targetHealth.IsDead
                    ? other.ClosestPoint(position)
                    : m_Target.GetComponent<Collider>().ClosestPoint(position);
                GameObject impactObj = Instantiate(hitEffect);
                impactObj.transform.position = closestPoint;
                impactObj.transform.rotation = m_Transform.rotation;
            }

            if (targetHealth == m_Target)
            {
                m_Target.TakeDamage(m_Instigator, m_Damage);
            }
            else
            {
                targetHealth.TakeDamage(m_Instigator, m_Damage);
            }

            foreach (GameObject toDestroy in destroyAfterHit)
            {
                toDestroy.SetActive(false);
            }

            onHit?.Invoke();
            m_UpdateUi?.Invoke();

            StartCoroutine(ReturnToPoolWithDelay(afterHitTTL));
        }

        private void PointTowardsTarget()
        {
            if (m_Target == null) return;
            m_Transform.LookAt(GetAimLocation());
            onLaunch?.Invoke();
        }

        private Vector3 GetAimLocation()
        {
            var targetCollider = m_Target.GetComponent<CapsuleCollider>();
            if (targetCollider == null)
            {
                return m_Target.transform.position;
            }

            return m_Target.transform.position + Vector3.up * targetCollider.height / 2;
        }

        private void ReturnToPool()
        {
            m_Pooler.AddToPool(gameObject);
        }

        private IEnumerator ReturnToPoolWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            ReturnToPool();
        }

        public void SetTarget(Health target, GameObject instigator, float damage, Action updateUI, string poolTag)
        {
            m_Target = target;
            m_Damage = damage;
            m_Instigator = instigator;
            m_UpdateUi = updateUI;
            speed = m_OriginalSpeed;
            if (string.IsNullOrEmpty(m_PoolTag))
            {
                m_PoolTag = poolTag;
            }

            PointTowardsTarget();
        }
    }
}