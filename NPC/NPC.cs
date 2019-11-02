using RPG.Control;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using static RPG.Util.Utility;

namespace RPG.NPC {
    [DisallowMultipleComponent]
    public class NPC : MonoBehaviour, IRaycastable {

        public CursorType Cursor => CursorType.NPC;
        public virtual void Interact() { }

        public bool HandleRaycast(PlayerController callingController) {
            if (Input.GetMouseButtonDown(0)) {
                if (IsTargetInRange(callingController.transform, transform, 2.5f)) {
                    callingController.GetComponent<CharacterBehaviour>().LookAtTarget(transform);
                    Interact();
                } else {
                    callingController.GetComponent<Mover>().StartMovement(transform.position, 1f).InteractWithTarget(delegate { Interact(); });
                }
            }
            return true;
        }
    }
}
