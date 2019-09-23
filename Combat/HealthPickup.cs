using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat {
    public class HealthPickup : PickupBase {
        [Range(0, 100)][SerializeField] private float healthPercentToRestore = 25;
        [SerializeField] private GameObject healFX = null;
        [SerializeField] private bool respawnable = true;
        [SerializeField] private float respawnTime = 5f;

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.tag == "Player") {
                Pickup(other.GetComponent<Health>(), healFX, healthPercentToRestore, respawnable, respawnTime);
            }
        }

    }
}
