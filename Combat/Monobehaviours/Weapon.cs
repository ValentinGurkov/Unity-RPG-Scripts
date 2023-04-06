using Attributes;
using Pooling;
using UnityEngine;

namespace Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private GameObject hitEffect;
        private Transform _transform;
        private GameObject _impact;

        private void Awake()
        {
            _transform = transform;
        }

        public void OnHit(HealthNew target, ObjectPooler pooler)
        {
            if (hitEffect == null) return;
            Vector3 position = _transform.position;
            Vector3 closestPoint = target.GetComponent<Collider>().ClosestPoint(position);
            _impact = pooler.SpawnFromPool(hitEffect, closestPoint, _transform.rotation);
            pooler.AddToPoolWithDelay(_impact.GetComponent<ParticleSystem>());
        }
    }
}