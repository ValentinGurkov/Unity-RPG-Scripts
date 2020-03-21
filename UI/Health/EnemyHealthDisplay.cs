using System;
using RPG.Combat;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        private Fighter m_Fighter;
        private TextMeshProUGUI m_Text;
        private string m_DefaultText;

        private void Awake()
        {
            m_Fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            m_Text = GetComponent<TextMeshProUGUI>();
            m_DefaultText = m_Text.text;
        }

        private void Start()
        {
            UpdateTargetHealth();
        }

        private void OnEnable()
        {
            m_Fighter.UpdateTargetUi += UpdateTargetHealth;
        }

        private void OnDisable()
        {
            m_Fighter.UpdateTargetUi -= UpdateTargetHealth;
        }

        private void UpdateTargetHealth()
        {
            if (m_Fighter.Target == null)
            {
                m_Text.text = m_DefaultText;
            }
            else
            {
                m_Text.text = $"{m_Fighter.Target.HealthPoints:0}/{m_Fighter.Target.MaxHealthPoints:0}";
            }
        }
    }
}