using System;
using RPG.Util;
using TMPro;
using UnityEngine;

namespace RPG.UI {
    public class DamageText : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI damageText = default;
        private ObjectPooler pooler;
        private string poolTag;

        private void Start() {
            pooler = ObjectPooler.Instace;
        }

        public void DestroyText() {
            pooler.AddToPool(poolTag, gameObject);
        }

        public void SetPoolTag(string tag) {
            if (string.IsNullOrEmpty(poolTag)) {
                poolTag = tag;
            }
        }

        public void SetValue(float amount) {
            damageText.text = String.Format("{0:0}", amount);
        }
    }
}
