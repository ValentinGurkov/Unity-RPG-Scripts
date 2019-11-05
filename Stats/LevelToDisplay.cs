using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats {
    public class LevelToDisplay : MonoBehaviour {
        private BaseStats baseStats;
        private TextMeshProUGUI text;

        private void Awake() {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            text = GetComponent<TextMeshProUGUI>();
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
