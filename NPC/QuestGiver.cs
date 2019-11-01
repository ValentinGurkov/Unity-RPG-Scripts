using RPG.Control;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using static RPG.Util.Utility;

namespace RPG.NPC {
    public class QuestGiver : DialogueInitiator, IRaycastable {
        public CursorType Cursor => CursorType.Quest;

        //TODO issue quest & save state (implement ISaveable)
        private bool quiestIssued = false;

        public override void Interact() {
            TriggerDialogue();
        }

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
