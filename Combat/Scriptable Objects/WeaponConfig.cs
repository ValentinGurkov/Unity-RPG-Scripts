using Attributes;
using Core;
using Pooling;
using UnityEngine;
using Util;

namespace Combat
{
    [CreateAssetMenu(fileName = "New WeaponConfig", menuName = "Attack/WeaponConfig", order = 0)]
    public class WeaponConfig : AttackDefinition
    {
        [Header("WeaponConfig")] [SerializeField]
        private Weapon weaponPrefab;

        [SerializeField] private Enums enums;
        private Transform _handInUse;
        private ObjectPooler _pooler;
        private Weapon _weapon;
        private const string WeaponName = "Weapon";
        private const string Destroying = "Destroying";

        public Transform HandInUse => _handInUse;

        public ObjectPooler Pooler => _pooler;

        private static void DestroyOldWeapon(Transform leftHand, Transform rightHand)
        {
            Transform currentWeapon = rightHand.Find(WeaponName);
            if (currentWeapon == null)
            {
                currentWeapon = leftHand.Find(WeaponName);
            }

            if (currentWeapon == null) return;


            currentWeapon.name = Destroying;
            Destroy(currentWeapon.gameObject);
        }

        private Transform GetTransform(Transform leftHand, Transform rightHand)
        {
            Transform handToUse = rightHand;
            if (ItemSlot == enums.ItemSlots[Constants.ItemSlots.LeftHand]) handToUse = leftHand;
            else if (ItemSlot == enums.ItemSlots[Constants.ItemSlots.TwoHanded])
                return handToUse; //TODO handle two-handed weapons (what would be different?);
            return handToUse;
        }

        public void Spawn(Transform leftHand, Transform rightHand, Animator animator, ObjectPooler pooler)
        {
            //destroy old weaponConfig is not working
            DestroyOldWeapon(leftHand, rightHand);
            if (weaponPrefab != null)
            {
                _handInUse = GetTransform(leftHand, rightHand);
                _weapon = Instantiate(weaponPrefab.gameObject, _handInUse).GetComponent<Weapon>();
                _weapon.name = WeaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (AnimatorOverride != null) animator.runtimeAnimatorController = AnimatorOverride;
            else if (overrideController != null) animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;

            _pooler = pooler;
        }

        public override void Attack(GameObject instigator, HealthNew target, float baseDamage)
        {
            //TODO we have hit effect in a scriptable object attribute and on a projectile monobehavoiur. optimize that
            if (_weapon != null) _weapon.OnHit(target, _pooler);
            base.Attack(instigator, target, baseDamage);
        }
    }
}