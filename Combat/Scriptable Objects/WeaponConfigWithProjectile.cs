using Attributes;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "New WeaponConfig", menuName = "Attack/WeaponConfig With Projectile", order = 0)]
    public class WeaponConfigWithProjectile : WeaponConfig
    {
        [Header("Projectile")] [SerializeField]
        private Projectile projectile;

        private Projectile _projectileInstance;
        private float _baseDamage;
        private GameObject _instigator;

        private void LaunchProjectile(HealthNew target)
        {
            GameObject instance = Pooler.SpawnFromPool(projectile.gameObject, HandInUse.position);
            if (!instance) return;
            instance.transform.position = HandInUse.position;

            _projectileInstance = instance.GetComponent<Projectile>();
            _projectileInstance.OnTargetHit += OnTargetHit;
            _projectileInstance.SetTarget(target, Pooler);
        }

        private void OnTargetHit(HealthNew target)
        {
            base.Attack(_instigator, target, _baseDamage);
            _projectileInstance.OnTargetHit -= OnTargetHit;
        }

        public override void Attack(GameObject instigator, HealthNew target, float baseDamage)
        {
            if (projectile == null) return;
            LaunchProjectile(target);
            _instigator = instigator;
            _baseDamage = baseDamage;
        }
    }
}