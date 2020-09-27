using UnityEngine;

namespace Combat
{
    public class WeaponPickup : PickupBase
    {
        [SerializeField] private Weapon weapon;


        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            var fighter = other.GetComponent<FighterNew>();
            if (fighter != null) Pickup(fighter);
        }

        private void Pickup(FighterNew fighter)
        {
            fighter.EquipWeapon(weapon);
            base.Pickup();
        }
    }
}