using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using RPG.NPC;
using UnityEngine;

namespace RPG.Events {
    public class EventSystem : MonoBehaviour {

        public delegate void EnemyEventHandler(Health enemy);
        public static event EnemyEventHandler OnEnemyDeath;

        public delegate void ItemPickupHandler(PickupBase pickup);
        public static event ItemPickupHandler OnItemPickedUp;

        public delegate void DialogueInitiatedHandler(DialogueInitiator npc);
        public static event DialogueInitiatedHandler OnTalkedToNPC;

        public void EnemyDied(Health enemy) {
            Debug.Log($"Enemy died: {enemy.name}");
            OnEnemyDeath?.Invoke(enemy);
        }

        public void ItemPickedUp(PickupBase pickup) {
            Debug.Log($"Item picked up: {pickup.name}");
            OnItemPickedUp?.Invoke(pickup);
        }

        public void InitiatedDialogue(DialogueInitiator npc) {
            OnTalkedToNPC?.Invoke(npc);
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}
