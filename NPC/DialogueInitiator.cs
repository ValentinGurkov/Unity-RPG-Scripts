using RPG.Control;
using RPG.Conversing;
using UnityEngine;

namespace RPG.NPC {
    public class DialogueInitiator : NPC {
        [SerializeField] private Dialogue dialogue;
        [SerializeField] private DialogueManager dialogueManager;

        public new CursorType Cursor => CursorType.Converse;
        public DialogueManager DialogueManager => dialogueManager;

        private bool isInteracting = false;

        private void OnEnable() {
            dialogueManager.onDialogueClose += EndInteraction;
        }

        private void OnDisable() {
            dialogueManager.onDialogueClose -= EndInteraction;
        }

        public override void Interact() {
            if (!isInteracting) {
                StartDialogue(dialogue);
            }
        }

        public void StartDialogue(Dialogue dialogue) {
            dialogueManager.StartDialogue(dialogue);
            isInteracting = true;
        }

        void EndInteraction() {
            isInteracting = false;
        }

    }
}
