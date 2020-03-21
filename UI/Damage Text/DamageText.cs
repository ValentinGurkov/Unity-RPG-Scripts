using System;
using RPG.Util;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI damageText;
        private ObjectPooler m_Pooler;
        private string m_PoolTag;

        private void Start()
        {
            m_Pooler = ObjectPooler.Instace;
        }

        public void DestroyText()
        {
            m_Pooler.AddToPool(m_PoolTag, gameObject);
        }

        public void SetPoolTag(string tag)
        {
            if (string.IsNullOrEmpty(m_PoolTag))
            {
                m_PoolTag = tag;
            }
        }

        public void SetValue(float amount)
        {
            damageText.text = $"{amount:0}";
        }
    }
}