using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat {

    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Create New  Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject {
        [SerializeField] private AnimatorOverrideController animatorOverride = null;
        [SerializeField] private Weapon equippedPrefab = null;
        [SerializeField] private Projectile projectile = null;
        [SerializeField] private float weaponRange = 3f;
        [SerializeField] private float weaponDamage = 25f;
        [SerializeField] private float percentageBonus = 0;
        [SerializeField] private HAND hand = HAND.RIGHT;
        private const string WEAPON_NAME = "Weapon";
        private const string DESTORYING = "Destroying";
        private enum HAND { LEFT, RIGHT }

        public Transform GetTransform(Transform rightHand, Transform leftHand) {
            Transform handTransform;
            return handTransform = hand == HAND.RIGHT ? rightHand : leftHand;
        }

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator) {
            DestroyOldWeapon(rightHand, leftHand);
            Weapon weapon = null;
            if (equippedPrefab != null) {
                weapon = Instantiate(equippedPrefab, GetTransform(rightHand, leftHand));
                weapon.gameObject.name = WEAPON_NAME;
            }
            AnimatorOverrideController overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null) {
                animator.runtimeAnimatorController = animatorOverride;
            } else if (overrideController != null) {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand) {
            Transform currentWeapon = rightHand.Find(WEAPON_NAME);
            if (currentWeapon == null) {
                currentWeapon = leftHand.Find(WEAPON_NAME);
            }
            if (currentWeapon == null) {
                return;
            }
            currentWeapon.name = DESTORYING;
            Destroy(currentWeapon.gameObject);
        }

        public bool HasProjectile() {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage) {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

        public float Range => weaponRange;

        public float Damage => weaponDamage;

        public float PercentageBous => percentageBonus;
    }
}
