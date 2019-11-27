using RPG.Attributes;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat {

    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable {

        public CursorType Cursor => CursorType.Combat;

        public bool HandleRaycast(GameObject callingObject) {
            Fighter fighter = callingObject.GetComponent<Fighter>();
            if (!fighter.CanAttack(gameObject)) {
                return false;
            }

            if (Input.GetMouseButton(0)) {
                fighter.Attack(gameObject);
            }

            return true;
        }

    }
}
