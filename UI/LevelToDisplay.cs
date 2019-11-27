using System;
using RPG.Stats;
using TMPro;
using UnityEngine;

namespace RPG.UI {
    public class LevelToDisplay : MonoBehaviour {
        private BaseStats baseStats;
        private TextMeshProUGUI text;

        private void Awake() {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            text = GetComponent<TextMeshProUGUI>();
        }

        private void Start() {
            UpdateLevel();
        }

        private void OnEnable() {
            baseStats.onLevelUp += UpdateLevel;
        }

        private void OnDisable() {
            baseStats.onLevelUp -= UpdateLevel;
        }

        private bool UpdateLevel() {
            text.text = String.Format("{0:0}", baseStats.GetLevel());
            return true;
        }
    }

}
