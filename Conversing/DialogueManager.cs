using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPG.Conversing
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Animator animator;
        private readonly Queue<string> m_DialogLines = new Queue<string>();
        private Coroutine m_CurrentDialogue;
        private const string IS_OPEN_TRIGGER = "IsOpen";

        public event Action onDialogueClose;

        private IEnumerator TypeSentance(IReadOnlyList<char> dialogLine)
        {
            dialogueText.text = "";
            for (int i = 0; i < dialogLine.Count; i++)
            {
                dialogueText.text += dialogLine[i];
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }

        public void StartDialogue(Dialogue dialogue)
        {
            Debug.Log($"Starting a conversation with {dialogue.Name}.");

            m_DialogLines.Clear();

            for (int i = 0; i < dialogue.DialogueLines.Length; i++)
            {
                m_DialogLines.Enqueue(dialogue.DialogueLines[i]);
            }

            nameText.text = dialogue.Name;
            animator.SetBool(IS_OPEN_TRIGGER, true);
            ContinueDialogue();
        }

        public void ContinueDialogue()
        {
            if (m_DialogLines.Count == 0)
            {
                CloseDialogue();
                return;
            }

            if (m_CurrentDialogue != null)
            {
                StopCoroutine(m_CurrentDialogue);
            }

            m_CurrentDialogue = StartCoroutine(TypeSentance(m_DialogLines.Dequeue().ToCharArray()));
        }

        // TODO Implement close dialogue on moving away from NPC
        private void CloseDialogue()
        {
            Debug.Log("End of conversation with.");
            animator.SetBool(IS_OPEN_TRIGGER, false);
            m_DialogLines.Clear();
            onDialogueClose?.Invoke();
        }
    }
}