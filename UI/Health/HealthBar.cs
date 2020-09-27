using Attributes;
using UnityEngine;

namespace UI.Health
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private HealthNew health;
        [SerializeField] private RectTransform foreground;
        [SerializeField] private Canvas rootCanvas;

        private void Start()
        {
            UpdateHealthDisplay();
        }

        private void OnEnable()
        {
            health.OnHealthUpdate += UpdateHealthDisplay;
        }

        private void OnDisable()
        {
            health.OnHealthUpdate -= UpdateHealthDisplay;
        }

        private void UpdateHealthDisplay()
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