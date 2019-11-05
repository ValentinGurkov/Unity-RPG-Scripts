using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Questing {
    public class QuestToDisplay : MonoBehaviour {
        // private Text text;
        private TextMeshProUGUI text;
        private string originalText;
        private QuestManager questManager;

        private void Awake() {
            text = GetComponent<TextMeshProUGUI>();
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

        //TODO veryfiy this - migrate UI to textmesh pro
        private void UpdateQuestDisplay(Quest quest) {
            text.text = "";
            if (quest.Goals[0].Completed) {
                text.text += "<s>" + quest.Goals[0].Description + "</s>";
            } else {
                text.text += quest.Goals[0].Description;
            }
            for (int i = 1; i < quest.Goals.Count; i++) {
                if (quest.Goals[1].Completed) {
                    text.text += "<s>" + String.Concat("\n", quest.Goals[i].Description) + "</s>";
                } else {
                    text.text += String.Concat("\n", quest.Goals[i].Description);
                }
            }
        }

    }

}
