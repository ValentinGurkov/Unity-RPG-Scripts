using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats {
    public class ExperienceDisplay : MonoBehaviour {
        private Experience experience;
        private TextMeshProUGUI text;

        private void Awake() {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
            text = GetComponent<TextMeshProUGUI>();
        }

        private void Update() {
            text.text = String.Format("{0:0}", experience.ExperiencePoints);
        }
    }

}
