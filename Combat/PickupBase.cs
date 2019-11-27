using System.Collections;
using RPG.Attributes;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat {
    public class PickupBase : MonoBehaviour, IRaycastable {
        [SerializeField] public OnPickupEvent onPickup;

        public CursorType Cursor => CursorType.Pickup;

        [System.Serializable]
        public class OnPickupEvent : UnityEvent<PickupBase> { }

        private Collider objectCollider;

        private void Awake() {
            objectCollider = GetComponent<Collider>();
        }

        private IEnumerator HideForSeconds(float time) {
            ShowPickup(false);
            yield return new WaitForSeconds(time);
            ShowPickup(true);
        }

        private void ShowPickup(bool shouldShow) {
            objectCollider.enabled = shouldShow;
            for (int i = 0; i < transform.childCount; i++) {
                transform.GetChild(i).gameObject.SetActive(shouldShow);
            }
        }

        public void Pickup(bool respawnable, float respawnTime) {
            if (respawnable) {
                StartCoroutine(HideForSeconds(respawnTime));
            } else {
                Destroy(gameObject);
            }
            onPickup.Invoke(this);
        }

        public bool HandleRaycast(GameObject callingObject) {
            if (Input.GetMouseButtonDown(0)) {
                callingObject.GetComponent<Mover>().StartMovement(transform.position, 1f);
            }
            return true;
        }
    }

}
