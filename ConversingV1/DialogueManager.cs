using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Logger = Util.Logger;

namespace ConversingV1
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Animator animator;
        private readonly Queue<string> _dialogLines = new Queue<string>();
        private Coroutine _currentDialogue;
        private static readonly int s_IsOpen = Animator.StringToHash("IsOpen");

        public event Action OnDialogueClose;

        private IEnumerator TypeSentence(IReadOnlyList<char> dialogLine)
        {
            dialogueText.text = "";
            for (var i = 0; i < dialogLine.Count; i++)
            {
                dialogueText.text += dialogLine[i];
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }

        public void StartDialogue(Dialogue dialogue)
        {
            Logger.Log($"Starting a conversation with {dialogue.Name}.");

            _dialogLines.Clear();

            for (var i = 0; i < dialogue.DialogueLines.Length; i++)
            {
                _dialogLines.Enqueue(dialogue.DialogueLines[i]);
            }

            nameText.text = dialogue.Name;
            animator.SetBool(s_IsOpen, true);
            ContinueDialogue();
        }

        private void ContinueDialogue()
        {
            if (_dialogLines.Count == 0)
            {
                CloseDialogue();
                return;
            }

            if (_currentDialogue != null)
            {
                StopCoroutine(_currentDialogue);
            }

            _currentDialogue = StartCoroutine(TypeSentence(_dialogLines.Dequeue().ToCharArray()));
        }

        // TODO Implement close dialogue on moving away from NPC
        private void CloseDialogue()
        {
            Logger.Log("End of conversation with.");
            animator.SetBool(s_IsOpen, false);
            _dialogLines.Clear();
            OnDialogueClose?.Invoke();
        }
    }
}