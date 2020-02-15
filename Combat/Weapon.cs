using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat {
    public class Weapon : MonoBehaviour {
        [SerializeField] private UnityEvent onHit = default;
        [SerializeField] private GameObject hitEffect = default;

        public void OnHit(Health target) {
            onHit?.Invoke();

            if (hitEffect != null) {
                GameObject impactObj = Instantiate(hitEffect);
                impactObj.transform.position = target.GetComponent<Collider>().ClosestPoint(transform.position);
                impactObj.transform.rotation = transform.rotation;
            }
        }
    }
}
