using RPG.Events;
using RPG.NPC;

namespace RPG.Questing {
    public class ConversationGoal : Goal {
        public string Target;

        public ConversationGoal(Stage stage, string target, string description) {
            Stage = stage;
            Target = target;
            Description = description;
        }

        private void TalkedToNPC(DialogueInitiator npc) {
            if (Stage.Active && npc.name.Contains(Target)) {
                Complete();
                EventSystem.OnTalkedToNPC -= TalkedToNPC;
            }
        }

        public override void Init() {
            base.Init();
            EventSystem.OnTalkedToNPC += TalkedToNPC;
        }
    }
}
