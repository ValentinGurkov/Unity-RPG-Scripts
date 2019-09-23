using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat {
    public class Weapon : MonoBehaviour {
        [SerializeField] private UnityEvent onHit;

        public void OnHit() {
            onHit.Invoke();
        }
    }
}
