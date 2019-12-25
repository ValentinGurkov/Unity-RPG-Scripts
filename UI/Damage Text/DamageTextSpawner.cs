using RPG.Util;
using UnityEngine;

namespace RPG.UI {
    public class DamageTextSpawner : MonoBehaviour {
        [SerializeField] private string poolTag = "damageText";
        private ObjectPooler pooler;

        private void Start() {
            pooler = ObjectPooler.Instace;
        }

        public void Spawn(float damage) {
            GameObject instance = pooler.SpawnFromPool(poolTag);
            if (instance == null) {
                return;
            }
            instance.transform.position = transform.position;
            DamageText damageText = instance.GetComponent<DamageText>();
            damageText.SetValue(damage);
            damageText.SetPoolTag(poolTag);
        }
    }

}
