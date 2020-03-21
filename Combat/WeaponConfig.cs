using System;
using RPG.Attributes;
using RPG.Util;
using UnityEngine;

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
        private const string WEAPON_NAME = "Weapon";
        private const string DESTORYING = "Destroying";

        private enum Hand
        {
            Left,
            Right
        }

        private ObjectPooler m_Pooler;
        public float Range => weaponRange;
        public float Damage => weaponDamage;
        public float PercentageBonus => percentageBonus;

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform currentWeapon = rightHand.Find(WEAPON_NAME);
            if (currentWeapon == null)
            {
                currentWeapon = leftHand.Find(WEAPON_NAME);
            }

            if (currentWeapon == null)
            {
                return;
            }

            currentWeapon.name = DESTORYING;
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
                weapon.gameObject.name = WEAPON_NAME;
            }

            AnimatorOverrideController overrideController =
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
            float calculatedDamage, Action updateUI)
        {
            if (m_Pooler == null)
            {
                m_Pooler = ObjectPooler.Instace;
            }

            GameObject instance = m_Pooler.SpawnFromPool(projectileTag);
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