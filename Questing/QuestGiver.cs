using System;
using RPG.Conversing;
using RPG.Core;
using RPG.NPC;
using RPG.Saving;
using UnityEngine;

namespace RPG.Questing
{
    public class QuestGiver : DialogueInitiator, IRaycastable, ISaveable
    {
        [SerializeField] private Quest quest;
        [SerializeField] private Dialogue questPendingDialogue;
        [SerializeField] private Dialogue questCompletedDialogue;
        [SerializeField] private Dialogue afterQuestDialogue;

        private bool m_AssignedQuest;
        private bool m_HasBeenHelped;
        private QuestManager m_QuestManager;

        public override CursorType Cursor => CursorType.Quest;

        private void Awake()
        {
            m_QuestManager = GameObject.FindWithTag("QuestManager").GetComponent<QuestManager>();
        }

        private void AssignQuest()
        {
            if (m_AssignedQuest) return;
            m_AssignedQuest = true;
            m_QuestManager.AddQuest(this, quest);
            DialogueManager.onDialogueClose -= AssignQuest;
        }

        private void CheckQuest()
        {
            if (quest == null) return;
            onDialogueInitiated?.Invoke(gameObject.name);
            StartDialogue(m_HasBeenHelped ? questCompletedDialogue : questPendingDialogue);
        }

        protected override void Interact()
        {
            if (!m_AssignedQuest && !m_HasBeenHelped)
            {
                base.Interact();
                DialogueManager.onDialogueClose += AssignQuest;
            }
            else if (m_AssignedQuest && !m_HasBeenHelped)
            {
                CheckQuest();
            }
            else
            {
                onDialogueInitiated?.Invoke(gameObject.name);
                StartDialogue(afterQuestDialogue);
            }
        }

        public void MarkQuestCompleted()
        {
            m_HasBeenHelped = true;
        }

        public object CaptureState()
        {
            return new Tuple<bool, bool>(m_AssignedQuest, m_HasBeenHelped);
        }

        public void RestoreState(object state)
        {
            (bool assignedQuest, bool hasBeenHelped) = (Tuple<bool, bool>) state;
            m_AssignedQuest = assignedQuest;
            m_HasBeenHelped = hasBeenHelped;
        }
    }
}