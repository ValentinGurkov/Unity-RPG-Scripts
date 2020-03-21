using System;
using RPG.Questing;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class QuestToDisplay : MonoBehaviour
    {
        private TextMeshProUGUI m_Text;
        private string m_DefaultText;
        private string m_Tmp = "";

        private void Awake()
        {
            m_Text = GetComponent<TextMeshProUGUI>();
            m_DefaultText = m_Text.text;
        }

        public void UpdateQuestDisplay(StageS stage)
        {
            if (stage == null) return;
            if (stage.Goals[0].Completed)
            {
                m_Tmp += "<s>" + stage.Goals[0].Description + "</s>";
            }
            else
            {
                m_Tmp += stage.Goals[0].Description;
            }

            for (int j = 1; j < stage.Goals.Count; j++)
            {
                if (stage.Goals[j].Completed)
                {
                    m_Tmp += "<s>" + String.Concat("\n", stage.Goals[j].Description) + "</s>";
                }
                else
                {
                    m_Tmp += String.Concat("\n", stage.Goals[j].Description);
                }
            }

            m_Text.text = m_Tmp;
            m_Tmp = "";
        }

        public void UpdateQuestDisplay(Goal goal)
        {
            if (goal != null)
            {
                m_Text.text = m_Text.text.Replace(goal.Description, "<s>" + goal.Description + "</s>");
            }
        }

        public void DisplayDefaultText()
        {
            m_Text.text = m_DefaultText;
        }
    }
}