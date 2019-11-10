using System;
using TMPro;
using UnityEngine;

namespace RPG.Attributes {
    public class HealthDisplay : MonoBehaviour {
        private Health health;
        private TextMeshProUGUI text;

        private void Awake() {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
            text = GetComponent<TextMeshProUGUI>();
        }

        private void Start() {
            UpdateHealth();
        }

        private void OnEnable() {
            health.onHealthUpdate += UpdateHealth;
        }

        private void OnDisable() {
            health.onHealthUpdate -= UpdateHealth;
        }

        private void UpdateHealth() {
            text.text = String.Format("{0:0}/{1:0}", health.HealthPoints, health.MaxHealthPoints);
        }
    }

}
