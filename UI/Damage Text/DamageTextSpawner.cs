using UnityEngine;

namespace RPG.UI {
    public class DamageTextSpawner : MonoBehaviour {
        [SerializeField] DamageText damageTextPrefab = null;

        public void Spawn(float damage) {
            DamageText instance = Instantiate<DamageText>(damageTextPrefab, transform);
            instance.SetValue(damage);
        }
    }

}
