using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPG.Conversing {
    public class DialogueManager : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Animator animator;
        private const string IS_OPEN_TRIGGER = "IsOpen";
        private Queue<string> dialogLines = new Queue<string>();
        private Coroutine currentDialogue = null;

        public event Action onDialogueClose;

        private IEnumerator TypeSentance(char[] dialogLine) {
            dialogueText.text = "";
            for (int i = 0; i < dialogLine.Length; i++) {
                dialogueText.text += dialogLine[i];
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }

        public void StartDialogue(Dialogue dialogue) {
            Debug.Log($"Starting a conversation with {dialogue.Name}.");

            dialogLines.Clear();

            for (int i = 0; i < dialogue.DialogueLines.Length; i++) {
                dialogLines.Enqueue(dialogue.DialogueLines[i]);
            }
            nameText.text = dialogue.Name;
            animator.SetBool(IS_OPEN_TRIGGER, true);
            ContinueDialogue();
        }

        public void ContinueDialogue() {
            if (dialogLines.Count == 0) {
                CloseDialogue();
                return;
            }
            if (currentDialogue != null) {
                StopCoroutine(currentDialogue);
            }
            currentDialogue = StartCoroutine(TypeSentance(dialogLines.Dequeue().ToCharArray()));

        }

        // TODO Implement close dialogue on moving away from NPC
        public void CloseDialogue() {
            Debug.Log("End of conversation with.");
            animator.SetBool(IS_OPEN_TRIGGER, false);
            dialogLines.Clear();
            onDialogueClose?.Invoke();
        }
    }

}
