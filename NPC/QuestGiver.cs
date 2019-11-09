using RPG.Control;
using RPG.Conversing;
using RPG.Questing;
using UnityEngine;

namespace RPG.NPC {
    public class QuestGiver : DialogueInitiator, IRaycastable {
        [SerializeField] private string questType;
        [SerializeField] private Dialogue questPendingDialogue;
        [SerializeField] private Dialogue questCompletedDialogue;
        [SerializeField] private Dialogue afterQuestDialogue;

        //TODO issue quest & save state (implement ISaveable)
        private bool assignedQuest = false;
        private bool hasBeenHelped = false;
        private QuestManager questManager;
        private Quest quest;

        public new CursorType Cursor => CursorType.Quest;

        private void Awake() {
            questManager = GameObject.FindWithTag("QuestManager").GetComponent<QuestManager>();
        }

        public override void Interact() {
            if (!assignedQuest && !hasBeenHelped) {
                base.Interact();
                base.DialogueManager.onDialogueClose += AssignQuest;
            } else if (assignedQuest && !hasBeenHelped) {
                CheckQuest();
            } else {
                StartDialogue(afterQuestDialogue);
            }
        }

        private void AssignQuest() {
            Debug.Log("Quest assigned");
            assignedQuest = true;
            quest = questManager.AddQuest(questType);
        }

        private void CheckQuest() {
            if (quest != null) {
                Debug.Log("Checking quest");
                onDialogeInitiated.Invoke(this);
                if (quest.Completed) {
                    Debug.Log("Quest completed");
                    quest.CompleteQuest();
                    hasBeenHelped = true;
                    assignedQuest = true;
                    base.DialogueManager.onDialogueClose -= AssignQuest;
                    StartDialogue(questCompletedDialogue);
                } else {
                    Debug.Log("Quest not yet completed");
                    StartDialogue(questPendingDialogue);
                }
            }
        }
    }

}
