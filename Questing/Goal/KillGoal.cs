using RPG.Attributes;
using RPG.Events;
using UnityEngine;

namespace RPG.Questing {
    public class KillGoal : Goal {
        private string Enemy;

        public KillGoal(Quest quest, string enemy, string description, bool completed, int currentAmount, int requiredAmount) {
            this.Quest = quest;
            this.Enemy = enemy;
            this.Description = description;
            this.Completed = completed;
            this.CurrentAmount = currentAmount;
            this.RequiredAmount = requiredAmount;
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
