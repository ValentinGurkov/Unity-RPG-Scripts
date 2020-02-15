using System;
using RPG.Questing;
using TMPro;
using UnityEngine;

namespace RPG.UI {
    public class QuestToDisplay : MonoBehaviour {
        private TextMeshProUGUI text;
        private string defaultText;
        private string tmp = "";

        private void Awake() {
            text = GetComponent<TextMeshProUGUI>();
            defaultText = text.text;
        }

        public void UpdateQuestDisplay(StageS stage) {
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

        public void UpdateQuestDisplay(Goal goal) {
            if (goal != null) {
                text.text = text.text.Replace(goal.Description, "<s>" + goal.Description + "</s>");
            }
        }

        public void DisplayDefaultText() {
            text.text = defaultText;
        }
    }
}
