using RPG.Attributes;
using RPG.Control;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat {

    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable {

        public CursorType Cursor => CursorType.Combat;

        public CharacterClass Type => baseStats.CharacterClass;

        private BaseStats baseStats;

        void Awake() {
            baseStats = GetComponent<BaseStats>();
        }

        public bool HandleRaycast(PlayerController callingController) {
            Fighter fighter = callingController.GetComponent<Fighter>();
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
