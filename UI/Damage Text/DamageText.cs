using Pooling;
using TMPro;
using UnityEngine;
using Util;

namespace UI.Damage_Text
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private float critFontMultiplier = 0.1f;

        private ObjectPooler _pooler;
        private float _fontSize;

        private void Awake()
        {
            _fontSize = damageText.fontSize;
        }

        // Called by animation event
        public void DestroyText()
        {
            _pooler.AddToPool(gameObject);
        }

        public void Initialize(float amount, bool isCritical, Color color, ObjectPooler pooler)
        {
            _pooler = pooler;
            damageText.text = $"{amount:0}";
            damageText.color = color;
            damageText.fontStyle = isCritical ? FontStyles.Bold | FontStyles.Italic : FontStyles.Normal;
            damageText.fontSize = isCritical ? _fontSize + _fontSize * critFontMultiplier : _fontSize;
        }
    }
}