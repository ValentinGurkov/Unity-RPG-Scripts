using RPG.Attributes;
using UnityEngine;

namespace RPG.UI {
    public class HealthBar : MonoBehaviour {

        [SerializeField] private Health health = null;
        [SerializeField] private RectTransform foreground = null;
        [SerializeField] private Canvas rootCanvas = null;

        private void Update() {
            if (Mathf.Approximately(health.Fraction, 0) || Mathf.Approximately(health.Fraction, 1)) {
                rootCanvas.enabled = false;
            } else {
                rootCanvas.enabled = true;
                foreground.localScale = new Vector3(health.Fraction, 1, 1);
            }
        }
    }
}
