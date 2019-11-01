using RPG.Conversing;
using UnityEngine;

namespace RPG.NPC {
    public class DialogueInitiator : NPC {
        [SerializeField] private Dialogue dialogue;

        private DialogueManager dialogueManager;
        private bool isInteracting = false;
        private void Start() {
            dialogueManager = FindObjectOfType<DialogueManager>();
            dialogueManager.onDialogueClose += EndInteraction;
        }

        public void TriggerDialogue() {
            if (!isInteracting) {
                dialogueManager.StartDialogue(dialogue);
                isInteracting = true;
            }
        }

        private void EndInteraction() {
            isInteracting = false;
        }
    }
}
