using System;
using TMPro;
using UnityEngine;

namespace RPG.Combat {
    public class EnemyHealthDisplay : MonoBehaviour {
        private Fighter fighter;

        private TextMeshProUGUI text;
        private string defaultText;

        private void Awake() {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            text = GetComponent<TextMeshProUGUI>();
            defaultText = text.text;
        }

        private void OnEnable() {
            fighter.updateTargetUI += UpdateTargetHealth;
        }

        private void OnDisable() {
            fighter.updateTargetUI -= UpdateTargetHealth;
        }

        private void UpdateTargetHealth() {
            if (fighter.Target == null) {
                text.text = defaultText;
            } else {
                text.text = String.Format("{0:0}/{1:0}", fighter.Target.HealthPoints, fighter.Target.MaxHealthPoints);
            }
        }
    }
}
