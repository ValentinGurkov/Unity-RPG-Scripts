using RPG.Attributes;
using RPG.Combat;
using UnityEngine;

namespace RPG.Events {
    public class EventSystem : MonoBehaviour {

        public delegate void EnemyEventHandler(Health enemy);
        public static event EnemyEventHandler OnEnemyDeath;

        public delegate void ItemPickupHandler(PickupBase pickup);
        public static event ItemPickupHandler OnItemPickedUp;

        public void EnemyDied(Health enemy) {
            if (OnEnemyDeath != null) {
                Debug.Log($"Enemy died: {enemy.name}");
                OnEnemyDeath(enemy);
            }
        }

        public void ItemPickedUp(PickupBase pickup) {
            if (OnItemPickedUp != null) {
                Debug.Log($"Item picked up: {pickup.name}");
                OnItemPickedUp(pickup);
            }
        }
    }
}
