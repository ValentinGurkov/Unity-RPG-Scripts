using System.Collections;
using Core;
using UnityEngine;
using UnityEngine.Events;
using Util;

namespace Combat
{
    public class PickupBase : Interactable
    {
        [SerializeField] private bool respawnable = true;
        [SerializeField] private float respawnTime = 5f;
        [SerializeField] private OnPickupEvent onPickUp;

        private WaitForSeconds _waitTime;

        [System.Serializable]
        public class OnPickupEvent : UnityEvent<string> { }

        public override CursorType Cursor => gameManager.Enums.CursorTypes[Constants.CursorTypes.Pickup];
        public override string Type => Constants.CursorTypes.Pickup;

        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _waitTime = new WaitForSeconds(respawnTime);
        }

        private IEnumerator HideForSeconds()
        {
            ShowPickup(false);
            yield return _waitTime;
            ShowPickup(true);
        }

        private void ShowPickup(bool shouldShow)
        {
            _collider.enabled = shouldShow;
            for (var i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(shouldShow);
            }
        }

        protected void Pickup()
        {
            if (respawnable) StartCoroutine(HideForSeconds());
            else Destroy(gameObject);

            onPickUp?.Invoke(gameObject.name);
        }
    }
}