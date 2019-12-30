using System;
using System.Collections;
using RPG.Attributes;
using RPG.Util;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat {

    public class Projectile : MonoBehaviour {
        [SerializeField] private float speed = 1f;
        [SerializeField] private bool isHoming = false;
        [SerializeField] private float projectileTTL = 10f;
        [SerializeField] private float afterHitTTL = 0.2f;
        [SerializeField] private GameObject hitEffect = null;
        [SerializeField] private GameObject[] destroyAfterHit;
        [SerializeField] private UnityEvent onHit;
        [SerializeField] private UnityEvent onLaunch;
        private const string TAG_CINEMATIC = "Cinematic";
        private const string TAG_PICKUP = "Pickup";
        private float damage = 0;
        private Health target = null;
        private GameObject instigator = null;
        private ObjectPooler pooler;
        private string poolTag;
        /// <summary>
        /// Can update UI when damaging an enemy (e.g. update enemy health on HUD)
        /// </summary>
        private Action updateUI = null;
        private float originalSpeed;

        private void Awake() {
            pooler = ObjectPooler.Instace;
            originalSpeed = speed;
        }

        private void Update() {
            if (target == null) {
                return;
            }
            if (isHoming && !target.IsDead) {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            StartCoroutine(ReturnToPoolWithDelay(projectileTTL));
        }

        private void OnTriggerEnter(Collider other) {
            // make projectiles go trough cinematic triggers and items on the ground
            if (other.gameObject.CompareTag(TAG_CINEMATIC) || other.gameObject.CompareTag(TAG_PICKUP)) {
                return;
            }

            foreach (GameObject toDestroy in destroyAfterHit) {
                toDestroy.SetActive(true);
            }

            Health targetHealth = other.GetComponent<Health>();

            if (targetHealth == null) { // to disallow friendly fire -  || targetHealth != target
                ReturnToPool();
                return;
            }

            if (targetHealth.IsDead) {
                StartCoroutine(ReturnToPoolWithDelay(projectileTTL));
                return;
            }

            speed = 0;

            if (hitEffect != null) {
                Vector3 closestPoint = !targetHealth.IsDead ? other.ClosestPoint(transform.position) : target.GetComponent<Collider>().ClosestPoint(transform.position);
                GameObject impactObj = Instantiate(hitEffect);
                impactObj.transform.position = closestPoint;
                impactObj.transform.rotation = transform.rotation;
            }

            if (targetHealth == target) {
                target.TakeDamage(instigator, damage);
            } else {
                targetHealth.TakeDamage(instigator, damage);
            }

            foreach (GameObject toDestroy in destroyAfterHit) {
                toDestroy.SetActive(false);
            }

            onHit.Invoke();

            if (updateUI != null) {
                updateUI();
            }

            StartCoroutine(ReturnToPoolWithDelay(afterHitTTL));
        }

        private void PointTowardsTarget() {
            if (target != null) {
                transform.LookAt(GetAimLocation());
                onLaunch.Invoke();
            }
        }

        private Vector3 GetAimLocation() {
            CapsuleCollider targetCollider = target.GetComponent<CapsuleCollider>();
            if (targetCollider == null) {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * targetCollider.height / 2;
        }

        private void ReturnToPool() {
            pooler.AddToPool(poolTag, gameObject);
        }

        private IEnumerator ReturnToPoolWithDelay(float delay) {
            yield return new WaitForSeconds(delay);
            ReturnToPool();
        }

        // TODO finish this
        /// <summary>
        /// Sets the target for the projectile.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="instigator"></param>
        /// <param name="damage"></param>
        /// <param name="updateUI"></param>
        public void SetTarget(Health target, GameObject instigator, float damage, Action updateUI, string tag) {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;
            this.updateUI = updateUI;
            speed = originalSpeed;
            if (string.IsNullOrEmpty(poolTag)) {
                poolTag = tag;
            }
            PointTowardsTarget();
        }
    }
}
