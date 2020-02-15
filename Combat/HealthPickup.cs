using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat {
    public class HealthPickup : PickupBase {
        [Range(0, 100)][SerializeField] private float healthPercentToRestore = 25;
        [SerializeField] private GameObject healFX = default;
        [SerializeField] private bool respawnable = true;
        [SerializeField] private float respawnTime = 5f;

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Player")) {
                Pickup(other.GetComponent<Health>(), healFX, healthPercentToRestore, respawnable, respawnTime);
            }
        }

        private void Pickup(Health health, GameObject healFX, float healthPercentToRestore, bool respawnable, float respawnTime) {
            Instantiate(healFX, health.transform);
            health.Heal(healthPercentToRestore);
            base.Pickup(respawnable, respawnTime);
        }

    }
}
