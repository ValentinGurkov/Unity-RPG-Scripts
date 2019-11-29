using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI {
    public class DamageText : MonoBehaviour {
        [SerializeField] private Text damageText = null;

        public void DestroyText() {
            Destroy(gameObject);
        }

        public void SetValue(float amount) {
            damageText.text = String.Format("{0:0}", amount);
        }
    }

}
