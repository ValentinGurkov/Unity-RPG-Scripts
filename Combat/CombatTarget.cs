using RPG.Attributes;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    /// <summary>
    /// GameObjects with this component will become attackable.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType Cursor => CursorType.Combat;

        public bool HandleRaycast(GameObject callingObject)
        {
            var fighter = callingObject.GetComponent<Fighter>();
            if (!fighter.CanAttack(gameObject))
            {
                return false;
            }

            if (Input.GetMouseButton(0))
            {
                fighter.Attack(gameObject);
            }

            return true;
        }
    }
}