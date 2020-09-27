using Attributes;
using UnityEngine;

namespace Combat
{
    public class HealthPickup : PickupBase
    {
        [Range(0, 100)] [SerializeField] private float healthPercentToRestore = 25;
        [SerializeField] private GameObject healFx;


        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("VAR");
            if (!other.gameObject.CompareTag("Player")) return;
            var health = other.GetComponent<HealthNew>();
            if (health != null) Pickup(health);
        }

        private void Pickup(HealthNew health)
        {
            Instantiate(healFx, health.transform);
            health.Heal(healthPercentToRestore);
            base.Pickup();
        }
    }
}