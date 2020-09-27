using Core;
using UnityEngine;
using Util;

namespace Combat
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Attack/Weapon", order = 0)]
    public class Weapon : AttackDefinition
    {
        [Header("Weapon")] [SerializeField] private GameObject weaponPrefab;
        [SerializeField] private Enums enums;
        private Transform _handInUse;
        private const string WeaponName = "Weapon";
        private const string Destroying = "Destroying";

        private void DestroyOldWeapon(Transform leftHand, Transform rightHand)
        {
            Transform currentWeapon = null;
            if (_handInUse != null)
            {
                _handInUse.Find(WeaponName);
            }
            else
            {
                currentWeapon = rightHand.Find(WeaponName);
                if (currentWeapon == null)
                {
                    currentWeapon = leftHand.Find(WeaponName);
                }
            }

            if (currentWeapon == null)
            {
                return;
            }


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

        public GameObject Spawn(Transform leftHand, Transform rightHand, Animator animator)
        {
            DestroyOldWeapon(leftHand, rightHand);
            GameObject weapon = null;
            if (weaponPrefab != null)
            {
                _handInUse = GetTransform(leftHand, rightHand);
                weapon = Instantiate(weaponPrefab, _handInUse);
                weapon.name = WeaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (AnimatorOverride != null) animator.runtimeAnimatorController = AnimatorOverride;
            else if (overrideController != null) animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;

            return weapon;
        }
    }
}