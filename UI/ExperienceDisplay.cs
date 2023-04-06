using System;
using RPG.Stats;
using Stats;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class ExperienceDisplay : MonoBehaviour
    {
        private Experience m_Experience;
        private TextMeshProUGUI m_Text;

        private void Awake()
        {
            m_Experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
            m_Text = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            UpdateExperience();
        }

        private void OnEnable()
        {
            m_Experience.OnExperienceGained += UpdateExperience;
        }

        private void OnDisable()
        {
            m_Experience.OnExperienceGained -= UpdateExperience;
        }

        private void UpdateExperience()
        {
            m_Text.text = $"{m_Experience.ExperiencePoints:0}";
        }
    }
}