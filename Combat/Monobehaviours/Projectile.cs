using System;
using System.Collections;
using System.Collections.Generic;
using Attributes;
using Pooling;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 1f;
        [SerializeField] private bool isHoming;
        [SerializeField] private float projectileTTL = 3f;
        [SerializeField] private float afterHitTTL = 0.2f;
        [SerializeField] private GameObject hitEffect;
        [SerializeField] private GameObject[] destroyAfterHit;
        [SerializeField] private UnityEvent onHit;
        [SerializeField] private UnityEvent onLaunch;
        [SerializeField] private List<string> tagsToGoTrough;

        private HealthNew _target;
        private ObjectPooler _pooler;
        private Transform _transform;
        private float _originalSpeed;
        private WaitForSeconds _projectileWait;
        private WaitForSeconds _afterHitWait;
        private Coroutine _ttlRoutine;
        private GameObject _impact;

        public Action<HealthNew> OnTargetHit;


        private void Awake()
        {
            _originalSpeed = speed;
            _transform = transform;
            _projectileWait = new WaitForSeconds(projectileTTL);
            _afterHitWait = new WaitForSeconds(afterHitTTL);
        }

        private void Update()
        {
            if (_target == null) return;

            if (isHoming && !_target.IsDead)
            {
                _transform.LookAt(GetAimLocation());
            }

            _transform.Translate(Vector3.forward * (speed * Time.deltaTime));
        }

        private void OnTriggerEnter(Collider other)
        {
            // make projectiles go trough cinematic triggers and items on the ground
            if (tagsToGoTrough.Contains(other.tag)) return;

            var targetHealth = other.GetComponent<HealthNew>();

            if (targetHealth == null)
            {
                // to disallow friendly fire -  || targetHealth != target
                ReturnToPool();
                return;
            }

            if (targetHealth.IsDead)
            {
                _ttlRoutine = StartCoroutine(StartTTL(_projectileWait));
                return;
            }

            speed = 0;

            if (hitEffect != null)
            {
                Vector3 position = _transform.position;
                Vector3 closestPoint = !targetHealth.IsDead
                    ? other.ClosestPoint(position)
                    : _target.GetComponent<Collider>().ClosestPoint(position);
                _impact = _pooler.SpawnFromPool(hitEffect, closestPoint, _transform.rotation);
                _pooler.AddToPoolWithDelay(_impact.GetComponent<ParticleSystem>());
            }

            OnTargetHit?.Invoke(targetHealth == _target ? _target : targetHealth);

            foreach (GameObject toDestroy in destroyAfterHit)
            {
                toDestroy.SetActive(false);
            }

            onHit?.Invoke();
            if (_ttlRoutine != null)
            {
                StopCoroutine(_ttlRoutine);
                _ttlRoutine = null;
            }

            ;
            _pooler.AddToPoolWithDelay(gameObject, _afterHitWait);
        }

        private void PointTowardsTarget()
        {
            if (_target == null) return;
            _transform.LookAt(GetAimLocation());
            onLaunch?.Invoke();
        }

        private Vector3 GetAimLocation()
        {
            var targetCollider = _target.GetComponent<CapsuleCollider>();
            if (targetCollider == null) return _target.transform.position;
            return _target.transform.position + Vector3.up * targetCollider.height / 2;
        }

        private void ReturnToPool()
        {
            _pooler.AddToPool(gameObject);
        }

        private IEnumerator StartTTL(WaitForSeconds ttl)
        {
            yield return ttl;
            _ttlRoutine = null;
            ReturnToPool();
        }

        public void SetTarget(HealthNew target, ObjectPooler pooler)
        {
            foreach (GameObject toDestroy in destroyAfterHit)
            {
                toDestroy.SetActive(true);
            }

            _target = target;
            speed = _originalSpeed;
            _pooler = pooler;
            _ttlRoutine = StartCoroutine(StartTTL(_projectileWait));

            PointTowardsTarget();
        }
    }
}