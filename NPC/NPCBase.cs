using RPG.Core;
using RPG.Movement;
using UnityEngine;
using static RPG.Util.Utility;

namespace RPG.NPC
{
    [DisallowMultipleComponent]
    public class NPCBase : MonoBehaviour, IRaycastable
    {
        public virtual CursorType Cursor => CursorType.NPC;
        protected virtual void Interact() { }

        public bool HandleRaycast(GameObject callingObject)
        {
            if (!Input.GetMouseButtonDown(0)) return true;
            if (IsTargetInRange(callingObject.transform, transform, 2.5f))
            {
                // TODO make this work with CharacterBehaviour's smooth LookAt. Maybe use a Coroutine?
                transform.LookAt(callingObject.transform);
                callingObject.transform.LookAt(transform);
                Interact();
            }
            else
            {
                callingObject.GetComponent<Mover>().StartMovement(transform.position, 1f)
                    .InteractWithTarget(Interact);
            }

            return true;
        }
    }
}