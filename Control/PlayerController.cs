using System;
using RPG.Attributes;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace RPG.Control {

    public class PlayerController : MonoBehaviour {

        [System.Serializable]
        struct CursorMapping {
            public CursorType type;
            public Texture2D texture;
            private Vector2 hotspot;

            public Vector2 HotSpot {
                get;
            }
        }

        [SerializeField] private CursorMapping[] mappings = null;
        [SerializeField] private float navMeshMaxProjectionDistance = 1f;
        [SerializeField] private float raycastRadius = 1f;

        private Mover mover;
        private Health playerHealth;

        private void Awake() {
            mover = GetComponent<Mover>();
            playerHealth = GetComponent<Health>();
        }

        private void Update() {
            if (InteractWithUI()) {
                return;
            }
            if (playerHealth.IsDead) {
                SetCursor(CursorType.None);
                return;
            }

            if (InteractWithComponent()) {
                return;
            }

            if (InteractWithMovement()) {
                return;
            }

            SetCursor(CursorType.None);
        }

        private bool InteractWithUI() {
            if (EventSystem.current.IsPointerOverGameObject()) {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

        private bool InteractWithComponent() {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits) {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables) {
                    if (raycastable.HandleRaycast(this)) {
                        SetCursor(raycastable.Cursor);
                        return true;
                    }
                }
            }
            return false;
        }

        private RaycastHit[] RaycastAllSorted() {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[hits.Length];
            for (int i = 0; i < distances.Length; i++) {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }

        private bool InteractWithMovement() {
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);

            if (hasHit) {
                if (!mover.CanMoveTo(target)) {
                    return false;
                }
                if (Input.GetMouseButton(0)) {
                    mover.StartMovement(target, 1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target) {
            target = new Vector3();
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) {
                return false;
            }
            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, navMeshMaxProjectionDistance, NavMesh.AllAreas);
            if (!hasCastToNavMesh) {
                return false;
            }
            target = navMeshHit.position;

            return true;
        }

        private CursorMapping GetCursorMapping(CursorType type) {
            for (int i = 0; i < mappings.Length; i++) {
                if (mappings[i].type == type) {
                    return mappings[i];
                }
            }
            return mappings[0];
        }

        private Ray GetMouseRay() {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        public void SetCursor(CursorType type) {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.HotSpot, CursorMode.Auto);
        }
    }
}
