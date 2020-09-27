using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "New Spell", menuName = "Attack/Spell", order = 0)]
    public class Spell : AttackDefinition
    {
        public void Cast() { }
    }
}