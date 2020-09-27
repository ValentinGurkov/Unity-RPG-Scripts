using System;
using RPG.Attributes;
using UnityEngine;
using Util;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Create New  Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] private AnimatorOverrideController animatorOverride;
        [SerializeField] private Weapon equippedPrefab;
        [SerializeField] private string projectileTag = "arrow";
        [SerializeField] private Projectile projectile;
        [SerializeField] private float weaponRange = 3f;
        [SerializeField] private float weaponDamage = 25f;
        [SerializeField] private float percentageBonus;
        [SerializeField] private Hand hand = Hand.Right;
        private const string WeaponName = "Weapon";
        private const string Destroying = "Destroying";

        private enum Hand
        {
            Left,
            Right
        }

        private ObjectPooler _pooler;
        public float Range => weaponRange;
        public float Damage => weaponDamage;
        public float PercentageBonus => percentageBonus;

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform currentWeapon = rightHand.Find(WeaponName);
            if (currentWeapon == null)
            {
                currentWeapon = leftHand.Find(WeaponName);
            }

            if (currentWeapon == null)
            {
                return;
            }

            currentWeapon.name = Destroying;
            Destroy(currentWeapon.gameObject);
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            return hand == Hand.Right ? rightHand : leftHand;
        }

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);
            Weapon weapon = null;
            if (equippedPrefab != null)
            {
                weapon = Instantiate(equippedPrefab, GetTransform(rightHand, leftHand));
                weapon.gameObject.name = WeaponName;
            }

            var overrideController =
                animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator,
            float calculatedDamage, Action updateUI, ObjectPooler pooler)
        {
            _pooler = pooler;

            GameObject instance = _pooler.SpawnFromPool(projectile.gameObject);
            if (!instance)
            {
                return;
            }

            var projectileInstance = instance.GetComponent<Projectile>();
            projectileInstance.transform.position = GetTransform(rightHand, leftHand).position;
            projectile.transform.rotation = Quaternion.identity;
            projectileInstance.SetTarget(target, instigator, calculatedDamage, updateUI, projectileTag);
        }
    }
}