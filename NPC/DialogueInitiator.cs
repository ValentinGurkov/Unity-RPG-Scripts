﻿using RPG.Conversing;
using RPG.Core;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.NPC {
    public class DialogueInitiator : NPCBase {
        [SerializeField] private Dialogue dialogue;
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] public OnDialogueInitiatedEvent onDialogeInitiated;
        private bool isInteracting = false;

        [System.Serializable]
        public class OnDialogueInitiatedEvent : UnityEvent<DialogueInitiator> { }

        public new CursorType Cursor => CursorType.Converse;
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
            onDialogeInitiated?.Invoke(this);
            isInteracting = true;
        }
    }
}
