using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes {
    public class HealthDisplay : MonoBehaviour {
        private Health health;
        private Text text;

        private void Awake() {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
            text = GetComponent<Text>();
        }

        private void Update() {
            text.text = String.Format("{0:0}/{1:0}", health.HealthPoints, health.MaxHealthPoints);
        }
    }

}
