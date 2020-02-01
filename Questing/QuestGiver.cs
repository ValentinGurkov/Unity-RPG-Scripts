using System;
using RPG.Conversing;
using RPG.Core;
using RPG.NPC;
using RPG.Saving;
using UnityEngine;

namespace RPG.Questing {
    public class QuestGiver : DialogueInitiator, IRaycastable, ISaveable {
        [SerializeField] private string questType;
        [SerializeField] private Dialogue questPendingDialogue;
        [SerializeField] private Dialogue questCompletedDialogue;
        [SerializeField] private Dialogue afterQuestDialogue;

        private bool assignedQuest = false;
        private bool hasBeenHelped = false;
        private QuestManager questManager;
        private Quest quest;

        public new CursorType Cursor => CursorType.Quest;

        private void Start() {
            questManager = GameObject.FindWithTag("QuestManager").GetComponent<QuestManager>();
        }

        private void AssignQuest() {
            if (!assignedQuest) {
                Debug.Log("Quest assigned");
                assignedQuest = true;
                quest = questManager.AddQuest(gameObject.name, questType);
                base.DialogueManager.onDialogueClose -= AssignQuest;
            }
        }

        private void CheckQuest() {
            if (quest != null) {
                Debug.Log("Checking quest");
                onDialogueInitiated?.Invoke(gameObject.name);
                if (quest.Completed) {
                    Debug.Log("Quest completed");
                    quest.CompleteQuest();
                    hasBeenHelped = true;
                    assignedQuest = true;
                    StartDialogue(questCompletedDialogue);
                } else {
                    Debug.Log("Quest not yet completed");
                    StartDialogue(questPendingDialogue);
                }
            }
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

        public object CaptureState() {
            return new Tuple<bool, bool>(assignedQuest, hasBeenHelped);
        }

        public void RestoreState(object state) {
            var t = (Tuple<bool, bool>) state;
            assignedQuest = t.Item1;
            hasBeenHelped = t.Item2;
        }

        public void SetQuest(Quest quest) {
            this.quest = quest;
        }
    }

}
