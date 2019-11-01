using RPG.Combat;
using UnityEngine;

public class WeaponPickup : PickupBase {
    [SerializeField] private WeaponConfig weapon = null;
    [SerializeField] private float respawnTime = 5f;
    [SerializeField] private bool respawnable = true;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            Pickup(other.GetComponent<Fighter>(), weapon, respawnable, respawnTime);
        }
    }
}
