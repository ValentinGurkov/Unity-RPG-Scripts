namespace RPG.Questing {
    public class KillGoal : Goal, IGoal {
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
        }

        public bool Evaluate(string enemy) {
            if (Stage.Active && enemy.Contains(Enemy)) {
                CurrentAmount++;
                return base.Evaluate();
            }
            return false;
        }
    }
}
