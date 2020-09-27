using Attributes;
using TMPro;
using UnityEngine;

namespace UI.Health
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private HealthNew health;
        [SerializeField] private TextMeshProUGUI text;

        private void Awake()
        {
            // in case prefab in the scene is not connected to the player in the scene
            if (health == null) health = GameObject.FindWithTag("Player").GetComponent<HealthNew>();
        }

        private void Start()
        {
            UpdateHealth();
        }

        private void OnEnable()
        {
            health.OnHealthUpdate += UpdateHealth;
        }

        private void OnDisable()
        {
            health.OnHealthUpdate -= UpdateHealth;
        }

        private void UpdateHealth()
        {
            text.text = $"{health.Health:0}/{health.MaxHealth:0}";
        }
    }
}