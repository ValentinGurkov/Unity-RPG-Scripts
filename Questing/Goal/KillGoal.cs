using RPG.Attributes;
using RPG.Events;
using UnityEngine;

namespace RPG.Questing {
    public class KillGoal : Goal {
        private string Enemy;

        public KillGoal(Stage stage, string enemy, int currentAmount, int requiredAmount, string description = "", bool completed = false) {
            Stage = stage;
            Enemy = enemy;
            CurrentAmount = currentAmount;
            RequiredAmount = requiredAmount;
            Description = description;
            Completed = completed;
        }

        public override void Init() {
            base.Init();
            EventSystem.OnEnemyDeath += EnemyDied;
        }

        private void EnemyDied(Health enemy) {
            if (Stage.Active && enemy.name.Contains(Enemy)) {
                CurrentAmount++;
                if (Evaluate()) {
                    EventSystem.OnEnemyDeath -= EnemyDied;
                }
            }
        }
    }
}
