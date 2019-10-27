using RPG.Control;
using RPG.Core;
using RPG.Dialogue;
using RPG.Movement;
using UnityEngine;
using static RPG.Utility.Utility;

namespace RPG.NPC {
    public class QuestGiver : NPC, IRaycastable {
        [SerializeField] private string[] dialogue;

        public CursorType Cursor => CursorType.Quest;
        private DialogueSystem dialogueSystem;

        //TODO issue quest & save state (implement ISaveable)
        private bool quiestIssued = false;

        void Start() {
            dialogueSystem = FindObjectOfType<DialogueSystem>();
        }

        public override void Interact() {
            dialogueSystem.AddNewDialogue(dialogue, this.name);
        }

        public bool HandleRaycast(PlayerController callingController) {
            if (Input.GetMouseButtonDown(0)) {
                if (IsTargetInRange(callingController.transform, transform, 1f)) {
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
