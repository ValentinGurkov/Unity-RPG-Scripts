using System;
using TMPro;
using UnityEngine;

namespace RPG.Questing {
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
        }

        private void OnDisable() {
            if (questManager != null) {
                questManager.onQuestAdded -= UpdateQuestDisplay;
                questManager.onQuestComplete -= DisplayDefaultText;
            }
        }

        private void UpdateQuestDisplay(Stage stage) {
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

        private void DisplayDefaultText() {
            text.text = defaultText;
        }

        //TODO maybe remake the event to work with the current active quest stage instead of looping the quest stages? Check quest manager if it can control this
        // private void UpdateQuestDisplay(Quest quest) {
        //     for (int i = 0; i < quest.Stages.Count; i++) {
        //         if (quest.Stages[i].Active) {
        //             if (quest.Stages[i].Goals[0].Completed) {
        //                 tmp += "<s>" + quest.Stages[i].Goals[0].Description + "</s>";
        //             } else {
        //                 tmp += quest.Stages[i].Goals[0].Description;
        //             }
        //             for (int j = 1; j < quest.Stages[i].Goals.Count; j++) {
        //                 if (quest.Stages[i].Goals[j].Completed) {
        //                     tmp += "<s>" + String.Concat("\n", quest.Stages[i].Goals[j].Description) + "</s>";
        //                 } else {
        //                     tmp += String.Concat("\n", quest.Stages[i].Goals[j].Description);
        //                 }
        //             }
        //         }
        //     }
        //     text.text = tmp;
        //     tmp = "";
        // }

    }

}
