using RPG.Events;
using RPG.NPC;
using UnityEngine;

namespace RPG.Questing {
  public class ConversationGoal : Goal {
    public string Target;

    public ConversationGoal(Stage stage, string target, string description) {
      Stage = stage;
      Target = target;
      Description = description;
    }

    public override void Init() {
      base.Init();
      EventSystem.OnTalkedToNPC += TalkedToNPC;
    }

    private void TalkedToNPC(DialogueInitiator npc) {
      Debug.Log($"Talked to: {npc.name}!");
      if (Stage.Active && npc.name.Contains(Target)) {
        Complete();
        EventSystem.OnTalkedToNPC -= TalkedToNPC;
      }
    }
  }
}
