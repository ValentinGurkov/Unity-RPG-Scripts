using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI {
    public class DialogueSystem : MonoBehaviour {
        private GameObject dialoguePanel;
        private Button continueButton;
        private Text dialogueText;
        private Text nameText;
        private List<string> dialogLines = new List<string>();
        private string npcName;
        private int currentDialogLine = 0;

        private void Start() {
            dialoguePanel = GameObject.FindGameObjectWithTag("Dialogue");
            continueButton = dialoguePanel.transform.Find("Continue").GetComponent<Button>();
            dialogueText = dialoguePanel.transform.Find("Dialogue Text").GetComponent<Text>();
            nameText = dialoguePanel.transform.Find("Name").GetChild(0).GetComponent<Text>();

            continueButton.onClick.AddListener(delegate { ContinueDialogue(); });

            dialoguePanel.SetActive(false);
        }

        public void AddNewDialogue(string[] lines, string npcName) {
            dialogLines = new List<string>(lines.Length);
            dialogLines.AddRange(lines);

            this.npcName = npcName;
            Debug.Log("dialog lines: " + dialogLines.Count);
            CreateDialogue();
        }

        public void CreateDialogue() {
            dialogueText.text = dialogLines[currentDialogLine];
            nameText.text = npcName;
            dialoguePanel.SetActive(true);
        }

        public void ContinueDialogue() {
            if (currentDialogLine < dialogLines.Count - 1) {
                currentDialogLine++;
                dialogueText.text = dialogLines[currentDialogLine];
            } else {
                CloseDialogue();
            }

        }

        //TODO Implement close dialogue on moving away from NPC
        public void CloseDialogue() {
            currentDialogLine = 0;
            dialoguePanel.SetActive(false);
        }
    }

}
