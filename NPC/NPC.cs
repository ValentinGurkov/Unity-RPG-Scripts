using RPG.Core;
using RPG.Movement;
using UnityEngine;
using static RPG.Util.Utility;

namespace RPG.NPC {
    [DisallowMultipleComponent]
    public class NPC : MonoBehaviour, IRaycastable {

        public CursorType Cursor => CursorType.NPC;
        public virtual void Interact() { }

        public bool HandleRaycast(GameObject callingObject) {
            if (Input.GetMouseButtonDown(0)) {
                if (IsTargetInRange(callingObject.transform, transform, 2.5f)) {
                    callingObject.GetComponent<CharacterBehaviour>().LookAtTarget(transform);
                    Interact();
                } else {
                    callingObject.GetComponent<Mover>().StartMovement(transform.position, 1f).InteractWithTarget(delegate { Interact(); });
                }
            }
            return true;
        }
    }
}
