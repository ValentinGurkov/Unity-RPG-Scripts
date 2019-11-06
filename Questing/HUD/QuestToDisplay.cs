using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace RPG.Questing {
    public class QuestToDisplay : MonoBehaviour {
        private TextMeshProUGUI text;
        private string originalText;
        private QuestManager questManager;
        private StringBuilder sb = new StringBuilder();

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

        private void UpdateQuestDisplay(Quest quest) {
            sb.Clear();
            if (quest.Goals[0].Completed) {
                sb.Append("<s>");
                sb.Append(quest.Goals[0].Description);
                sb.Append("</s>");
            } else {
                sb.Append(quest.Goals[0].Description);
            }
            for (int i = 1; i < quest.Goals.Count; i++) {
                if (quest.Goals[1].Completed) {
                    sb.Append("<s>");
                    sb.Append(String.Concat("\n", quest.Goals[i].Description));
                    sb.Append("</s>");
                } else {
                    sb.Append(String.Concat("\n", quest.Goals[i].Description));
                }
            }
            text.text = sb.ToString();
        }

    }

}
