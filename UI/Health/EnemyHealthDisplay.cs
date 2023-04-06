using Combat;
using TMPro;
using UnityEngine;

namespace UI.Health
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        private FighterNew _fighter;
        private TextMeshProUGUI _text;
        private string _defaultText;

        private void Awake()
        {
            _fighter = GameObject.FindWithTag("Player").GetComponent<FighterNew>();
            _text = GetComponent<TextMeshProUGUI>();
            _defaultText = _text.text;
        }

        private void Start()
        {
            UpdateTargetHealth();
        }

        private void OnEnable()
        {
            _fighter.OnTargetStatusChanged += UpdateTargetHealth;
        }

        private void OnDisable()
        {
            _fighter.OnTargetStatusChanged -= UpdateTargetHealth;
        }

        private void UpdateTargetHealth()
        {
            _text.text = _fighter.Target == null ? _defaultText : $"{_fighter.Target.Health:0}/{_fighter.Target.MaxHealth:0}";
        }
    }
}