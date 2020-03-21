using RPG.Attributes;
using UnityEngine;

namespace RPG.UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private RectTransform foreground;
        [SerializeField] private Canvas rootCanvas;

        private void Update()
        {
            if (Mathf.Approximately(health.Fraction, 0) || Mathf.Approximately(health.Fraction, 1))
            {
                rootCanvas.enabled = false;
            }
            else
            {
                rootCanvas.enabled = true;
                foreground.localScale = new Vector3(health.Fraction, 1, 1);
            }
        }
    }
}