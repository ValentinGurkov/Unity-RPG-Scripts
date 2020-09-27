using Combat;
using UnityEngine;

namespace Core
{
    public class InputMouseAttackCommand : Command
    {
        private FighterNew _fighter;

        private void Awake()
        {
            _fighter = GetComponent<FighterNew>();
        }

        public void Execute(GameObject target)
        {
            if (_fighter.CanAttack(target)) _fighter.Attack(target);
        }
    }
}