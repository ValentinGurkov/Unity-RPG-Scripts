using RPG.Combat;
using RPG.Events;
using RPG.Stats;

namespace RPG.Quests {
    public class KillGoal : Goal {
        public CharacterClass EnemyType { get; set; }

        public KillGoal(Quest quest, CharacterClass enemyType, string description, bool completed, int currentAmount, int requiredAmount) {
            this.Quest = quest;
            this.EnemyType = enemyType;
            this.Description = description;
            this.Completed = completed;
            this.CurrentAmount = currentAmount;
            this.RequiredAmount = requiredAmount;
        }

        public override void Init() {
            base.Init();
            CombatEvents.OnEnemyDeath += EnemyDied;
        }

        void EnemyDied(CombatTarget enemy) {
            if (enemy.Type == this.EnemyType) {
                this.CurrentAmount++;
                Evaluate();
            }
        }
    }

}
