using System;
using RPG.Stats;
using TMPro;
using UnityEngine;

namespace RPG.UI {
    public class ExperienceDisplay : MonoBehaviour {
        private Experience experience;
        private TextMeshProUGUI text;

        private void Awake() {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
            text = GetComponent<TextMeshProUGUI>();
        }

        private void Start() {
            UpdateExperience();
        }

        private void OnEnable() {
            experience.onExperienceGained += UpdateExperience;
        }

        private void OnDisable() {
            experience.onExperienceGained -= UpdateExperience;
        }

        private void UpdateExperience() {
            text.text = String.Format("{0:0}", experience.ExperiencePoints);
        }
    }

}
