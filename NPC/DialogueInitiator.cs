using RPG.Conversing;
using RPG.Core;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.NPC
{
    public class DialogueInitiator : NPCBase
    {
        [SerializeField] private Dialogue dialogue = default;
        [SerializeField] private DialogueManager dialogueManager = default;
        private bool m_IsInteracting;
        public DialogueInitiatedEvent onDialogueInitiated;

        [System.Serializable]
        public class DialogueInitiatedEvent : UnityEvent<string> { }

        public override CursorType Cursor => CursorType.Converse;
        protected DialogueManager DialogueManager => dialogueManager;

        private void OnEnable()
        {
            dialogueManager.onDialogueClose += EndInteraction;
        }

        private void OnDisable()
        {
            dialogueManager.onDialogueClose -= EndInteraction;
        }

        private void EndInteraction()
        {
            m_IsInteracting = false;
        }

        protected override void Interact()
        {
            if (m_IsInteracting) return;
            onDialogueInitiated?.Invoke(gameObject.name);
            StartDialogue(dialogue);
        }

        protected void StartDialogue(Dialogue dialogue)
        {
            dialogueManager.StartDialogue(dialogue);
            m_IsInteracting = true;
        }
    }
}