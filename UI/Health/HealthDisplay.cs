using System;
using RPG.Attributes;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class HealthDisplay : MonoBehaviour
    {
        private Health m_Health;
        private TextMeshProUGUI m_Text;

        private void Awake()
        {
            m_Health = GameObject.FindWithTag("Player").GetComponent<Health>();
            m_Text = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            UpdateHealth();
        }

        private void OnEnable()
        {
            m_Health.OnHealthUpdate += UpdateHealth;
        }

        private void OnDisable()
        {
            m_Health.OnHealthUpdate -= UpdateHealth;
        }

        private void UpdateHealth()
        {
            m_Text.text = $"{m_Health.HealthPoints:0}/{m_Health.MaxHealthPoints:0}";
        }
    }
}