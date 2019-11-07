using RPG.Attributes;
using RPG.Events;
using UnityEngine;

namespace RPG.Questing {
    public class KillGoal : Goal {
        private string Enemy;

        public KillGoal(Stage stage, string enemy, int currentAmount, int requiredAmount, string description = "", bool completed = false) {
            this.Stage = stage;
            this.Enemy = enemy;
            this.CurrentAmount = currentAmount;
            this.RequiredAmount = requiredAmount;
            this.Description = description;
            this.Completed = completed;
        }

        public override void Init() {
            base.Init();
            EventSystem.OnEnemyDeath += EnemyDied;
        }

        private void EnemyDied(Health enemy) {
            Debug.Log($"Enemy has died: {enemy.name}!");
            if (enemy.name == Enemy) {
                this.CurrentAmount++;
                if (Evaluate()) {
                    EventSystem.OnEnemyDeath -= EnemyDied;
                }
            }
        }
    }
}
