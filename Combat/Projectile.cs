using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat {

    public class Projectile : MonoBehaviour {
        [SerializeField] private float speed = 1f;
        [SerializeField] private bool isHoming = false;
        [SerializeField] private GameObject hitEffect = null;
        [SerializeField] private float projectileTTL = 10f;
        [SerializeField] private float afterHitTTL = 0.2f;
        [SerializeField] private GameObject[] destroyAfterHit;
        [SerializeField] private UnityEvent onHit;
        private Health target = null;
        private GameObject instigator = null;
        private float damage = 0;

        void Start() {
            // use character behaviour look at method?
            transform.LookAt(GetAimLocation());
        }

        private void Update() {
            if (target == null) {
                return;
            }
            if (isHoming && !target.IsDead) {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        void OnTriggerEnter(Collider other) {
            Health targetHealth = other.GetComponent<Health>();
            if (targetHealth == null) { // to disallow friendly fire -  || targetHealth != target
                Destroy(gameObject);
                return;
            }

            if (targetHealth.IsDead) {
                Destroy(gameObject, projectileTTL);
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
                Destroy(toDestroy);
            }

            onHit.Invoke();

            Destroy(gameObject, afterHitTTL);
        }

        private Vector3 GetAimLocation() {
            CapsuleCollider targetCollider = target.GetComponent<CapsuleCollider>();
            if (targetCollider == null) {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * targetCollider.height / 2;
        }

        public void SetTarget(Health target, GameObject instigator, float damage) {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;
        }
    }
}
