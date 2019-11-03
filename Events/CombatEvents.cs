using RPG.Attributes;
using UnityEngine;

namespace RPG.Events {
    public class CombatEvents : MonoBehaviour {

        public delegate void EnemyEventHandler(Health enemy);
        public static event EnemyEventHandler OnEnemyDeath;

        public void EnemyDied(Health enemy) {
            if (OnEnemyDeath != null) {
                Debug.Log($"Enemy died: {enemy.gameObject.name}");
                OnEnemyDeath(enemy);
            }
        }
    }
}
