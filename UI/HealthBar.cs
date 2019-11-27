using RPG.Attributes;
using UnityEngine;

namespace RPG.UI {
    public class HealthBar : MonoBehaviour {

        [SerializeField] private Health healthComponent = null;
        [SerializeField] private RectTransform foreground = null;
        [SerializeField] private Canvas rootCanvas = null;

        void Update() {
            if (Mathf.Approximately(healthComponent.Fraction, 0) || Mathf.Approximately(healthComponent.Fraction, 1)) {
                rootCanvas.enabled = false;
            } else {
                rootCanvas.enabled = true;
                foreground.localScale = new Vector3(healthComponent.Fraction, 1, 1);
            }
        }
    }

}
