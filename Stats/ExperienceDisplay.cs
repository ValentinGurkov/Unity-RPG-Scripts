using System;
using TMPro;
using UnityEngine;

namespace RPG.Stats {
    public class ExperienceDisplay : MonoBehaviour {
        private Experience experience;
        private TextMeshProUGUI text;

        private void Awake() {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
            text = GetComponent<TextMeshProUGUI>();
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
