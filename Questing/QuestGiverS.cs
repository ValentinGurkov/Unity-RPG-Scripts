using System;
using RPG.Conversing;
using RPG.Core;
using RPG.NPC;
using RPG.Saving;
using UnityEngine;

namespace RPG.Questing {
    public class QuestGiverS : DialogueInitiator, IRaycastable, ISaveable {
        [SerializeField] private QuestS quest;
        [SerializeField] private Dialogue questPendingDialogue;
        [SerializeField] private Dialogue questCompletedDialogue;
        [SerializeField] private Dialogue afterQuestDialogue;

        private bool assignedQuest = false;
        private bool hasBeenHelped = false;
        private QuestManagerS questManager;

        public override CursorType Cursor => CursorType.Quest;

        private void Awake() {
            questManager = GameObject.FindWithTag("QuestManager").GetComponent<QuestManagerS>();
        }

        private void AssignQuest() {
            if (!assignedQuest) {
                Debug.Log("Quest assigned");
                assignedQuest = true;
                questManager.AddQuest(this, quest);
                base.DialogueManager.onDialogueClose -= AssignQuest;
            }
        }

        private void CheckQuest() {
            if (quest != null) {
                Debug.Log("Checking quest");
                if (quest.Completed) {
                    hasBeenHelped = true;
                    Debug.Log("Quest completed");
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

        public void SetQuest(QuestS quest) {
            this.quest = quest;
        }

        public void MarkQuestCompleted() { }

        public object CaptureState() {
            return new Tuple<bool, bool>(assignedQuest, hasBeenHelped);
        }

        public void RestoreState(object state) {
            var t = (Tuple<bool, bool>) state;
            assignedQuest = t.Item1;
            hasBeenHelped = t.Item2;
        }
    }

}
