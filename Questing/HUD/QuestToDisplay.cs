using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Questing {
    public class QuestToDisplay : MonoBehaviour {
        private Text text;
        private string originalText;
        private QuestManager questManager;

        private void Awake() {
            text = GetComponent<Text>();
            originalText = text.text;
        }

        // Need to find a neat way to handle the refernce to QuestManager as it is instaced dynamically
        private void Start() {
            questManager = GameObject.FindWithTag("QuestManager").GetComponent<QuestManager>();
            questManager.onQuestAdded += UpdateQuestDisplay;
        }

        private void OnDisable() {
            if (questManager != null) {
                questManager.onQuestAdded -= UpdateQuestDisplay;
            }
        }

        private void UpdateQuestDisplay(Quest quest) {
            text.text = "";
            text.text += quest.Goals[0].Description;
            for (int i = 1; i < quest.Goals.Count; i++) {
                text.text += String.Concat("\n", quest.Goals[i].Description);
            }
        }

    }

}
