using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat {
    public class EnemyHealthDisplay : MonoBehaviour {
        private Fighter fighter;
        private TextMeshProUGUI text;
        private string originalText;

        private void Awake() {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            text = GetComponent<TextMeshProUGUI>();
            originalText = text.text;
        }

        private void Update() {
            if (fighter.Target == null) {
                text.text = originalText;
            } else {
                text.text = String.Format("{0:0}/{1:0}", fighter.Target.HealthPoints, fighter.Target.MaxHealthPoints);
            }
        }
    }
}
