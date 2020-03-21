using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    public class HealthPickup : PickupBase
    {
        [Range(0, 100)] [SerializeField] private float healthPercentToRestore = 25;
        [SerializeField] private GameObject healFx;
        [SerializeField] private bool respawnable = true;
        [SerializeField] private float respawnTime = 5f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Pickup(other.GetComponent<Health>(), healFx, healthPercentToRestore, respawnable, respawnTime);
            }
        }

        private void Pickup(Health health, GameObject healFX, float percentToRestore, bool respawn,
            float time)
        {
            Instantiate(healFX, health.transform);
            health.Heal(percentToRestore);
            base.Pickup(respawn, time);
        }
    }
}