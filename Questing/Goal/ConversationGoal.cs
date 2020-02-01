namespace RPG.Questing {
    public class ConversationGoal : Goal, IGoal {
        public string Target;

        public ConversationGoal(Stage stage, string target, string description) {
            Stage = stage;
            Target = target;
            Description = description;
        }

        public bool Evaluate(string npc) {
            if (Stage.Active && npc.Contains(Target)) {
                Complete();
                return true;
            }
            return false;
        }

        public override void Init() {
            base.Init();
        }
    }
}
