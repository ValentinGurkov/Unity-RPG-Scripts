using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats {
    public class LevelToDisplay : MonoBehaviour {
        private BaseStats baseStats;
        private Text text;

        private void Awake() {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            text = GetComponent<Text>();
        }

        private void Update() {
            var test = baseStats.GetLevel();
            text.text = String.Format("{0:0}", baseStats.GetLevel());
        }
    }

}
