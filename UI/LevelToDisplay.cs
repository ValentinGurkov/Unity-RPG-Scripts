using System;
using RPG.Stats;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class LevelToDisplay : MonoBehaviour
    {
        private BaseStats m_BaseStats;
        private TextMeshProUGUI m_Text;

        private void Awake()
        {
            m_BaseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            m_Text = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            UpdateLevel();
        }

        private void OnEnable()
        {
            m_BaseStats.OnLevelUp += UpdateLevel;
        }

        private void OnDisable()
        {
            m_BaseStats.OnLevelUp -= UpdateLevel;
        }

        private bool UpdateLevel()
        {
            m_Text.text = $"{m_BaseStats.Level:0}";
            return true;
        }
    }
}