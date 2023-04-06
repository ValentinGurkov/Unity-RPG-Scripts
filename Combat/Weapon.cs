using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private UnityEvent onHit;
        [SerializeField] private GameObject hitEffect;

        public void OnHit(Health target)
        {
            onHit?.Invoke();

            if (hitEffect == null) return;
            GameObject impactObj = Instantiate(hitEffect);
            impactObj.transform.position = target.GetComponent<Collider>().ClosestPoint(transform.position);
            impactObj.transform.rotation = transform.rotation;
        }
    }
}