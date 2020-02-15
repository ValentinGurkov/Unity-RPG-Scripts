using System;
using RPG.Conversing;
using RPG.Core;
using RPG.NPC;
using RPG.Saving;
using UnityEngine;

namespace RPG.Questing {
    public class QuestGiver : DialogueInitiator, IRaycastable, ISaveable {
        [SerializeField] private Quest quest = default;
        [SerializeField] private Dialogue questPendingDialogue = default;
        [SerializeField] private Dialogue questCompletedDialogue = default;
        [SerializeField] private Dialogue afterQuestDialogue = default;

        private bool assignedQuest = false;
        private bool hasBeenHelped = false;
        private QuestManager questManager;

        public override CursorType Cursor => CursorType.Quest;

        private void Awake() {
            questManager = GameObject.FindWithTag("QuestManager").GetComponent<QuestManager>();
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
                onDialogueInitiated?.Invoke(gameObject.name);
                if (hasBeenHelped) {
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
                onDialogueInitiated?.Invoke(gameObject.name);
                StartDialogue(afterQuestDialogue);
            }
        }

        public void MarkQuestCompleted() {
            hasBeenHelped = true;
        }

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
