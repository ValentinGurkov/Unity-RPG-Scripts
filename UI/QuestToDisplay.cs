using System;
using RPG.Questing;
using TMPro;
using UnityEngine;

namespace RPG.UI {
    public class QuestToDisplay : MonoBehaviour {
        private TextMeshProUGUI text;
        private string defaultText;
        private QuestManager questManager;
        private string tmp = "";

        private void Awake() {
            text = GetComponent<TextMeshProUGUI>();
            defaultText = text.text;
        }

        // Need to find a neat way to handle the refernce to QuestManager as it is instaced dynamically
        private void Start() {
            questManager = GameObject.FindWithTag("QuestManager").GetComponent<QuestManager>();
            questManager.onQuestAdded += UpdateQuestDisplay;
            questManager.onQuestComplete += DisplayDefaultText;
            UpdateQuestDisplay(questManager.Subscribe());
        }

        private void OnEnable() {
            if (questManager != null) {
                questManager.onQuestAdded += UpdateQuestDisplay;
                questManager.onQuestComplete += DisplayDefaultText;
            }
        }

        private void OnDisable() {
            if (questManager != null) {
                questManager.onQuestAdded -= UpdateQuestDisplay;
                questManager.onQuestComplete -= DisplayDefaultText;
            }
        }

        private void UpdateQuestDisplay(Stage stage) {
            if (stage != null) {
                if (stage.Goals[0].Completed) {
                    tmp += "<s>" + stage.Goals[0].Description + "</s>";
                } else {
                    tmp += stage.Goals[0].Description;
                }
                for (int j = 1; j < stage.Goals.Count; j++) {
                    if (stage.Goals[j].Completed) {
                        tmp += "<s>" + String.Concat("\n", stage.Goals[j].Description) + "</s>";
                    } else {
                        tmp += String.Concat("\n", stage.Goals[j].Description);
                    }
                }
                text.text = tmp;
                tmp = "";
            }
        }

        private void DisplayDefaultText() {
            questManager.onQuestAdded -= UpdateQuestDisplay;
            questManager.onQuestComplete -= DisplayDefaultText;
            text.text = defaultText;
        }

    }

}
