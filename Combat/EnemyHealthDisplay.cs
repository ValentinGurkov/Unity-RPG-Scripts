using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat {
    public class EnemyHealthDisplay : MonoBehaviour {
        private Fighter fighter;
        private Text text;

        private void Awake() {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            text = GetComponent<Text>();
        }

        private void Update() {
            if (fighter.Target == null) {
                text.text = "N/A";
            } else {
                text.text = String.Format("{0:0}/{1:0}", fighter.Target.HealthPoints, fighter.Target.MaxHealthPoints);
            }
        }
    }
}
