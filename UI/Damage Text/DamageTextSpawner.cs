using UnityEngine;
using Util;

namespace UI.Damage_Text
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] private DamageText damageTextPrefab;
        private ObjectPooler _pooler;

        private void Start()
        {
            _pooler = FindObjectOfType<ObjectPooler>();
        }

        public void Spawn(float damage, bool isCritical, Color color)
        {
            GameObject instance = _pooler.SpawnFromPool(damageTextPrefab.gameObject);
            if (instance == null) return;
            instance.SetActive(false);
            instance.transform.position = transform.position;
            instance.SetActive(true);
            var damageText = instance.GetComponent<DamageText>();
            damageText.Initialize(damage, isCritical, color, _pooler);
        }
    }
}