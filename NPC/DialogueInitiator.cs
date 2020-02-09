using RPG.Conversing;
using RPG.Core;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.NPC {
    public class DialogueInitiator : NPCBase {
        [SerializeField] private Dialogue dialogue;
        [SerializeField] private DialogueManager dialogueManager;
        private bool isInteracting = false;
        public DialogueInitiatedEvent onDialogueInitiated;

        [System.Serializable]
        public class DialogueInitiatedEvent : UnityEvent<string> { }

        public override CursorType Cursor => CursorType.Converse;
        public DialogueManager DialogueManager => dialogueManager;

        private void OnEnable() {
            dialogueManager.onDialogueClose += EndInteraction;
        }

        private void OnDisable() {
            dialogueManager.onDialogueClose -= EndInteraction;
        }

        private void EndInteraction() {
            isInteracting = false;
        }

        public override void Interact() {
            if (!isInteracting) {
                StartDialogue(dialogue);
            }
        }

        public void StartDialogue(Dialogue dialogue) {
            dialogueManager.StartDialogue(dialogue);
            onDialogueInitiated?.Invoke(gameObject.name);
            isInteracting = true;
        }
    }
}
