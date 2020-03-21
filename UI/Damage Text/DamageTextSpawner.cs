using RPG.Util;
using UnityEngine;

namespace RPG.UI
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] private string poolTag = "damageText";
        private ObjectPooler m_Pooler;

        private void Start()
        {
            m_Pooler = ObjectPooler.Instace;
        }

        public void Spawn(float damage)
        {
            GameObject instance = m_Pooler.SpawnFromPool(poolTag);
            if (instance == null)
            {
                return;
            }

            instance.transform.position = transform.position;
            var damageText = instance.GetComponent<DamageText>();
            damageText.SetValue(damage);
            damageText.SetPoolTag(poolTag);
        }
    }
}