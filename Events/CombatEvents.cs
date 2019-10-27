using RPG.Combat;
using UnityEngine;

namespace RPG.Events {
    public class CombatEvents : MonoBehaviour {

        public delegate void EnemyEventHandler(CombatTarget enemy);
        public static event EnemyEventHandler OnEnemyDeath;

        public static void EnemyDied(CombatTarget enemy) {
            if (OnEnemyDeath != null) {
                OnEnemyDeath(enemy);
            }
        }
    }
}
