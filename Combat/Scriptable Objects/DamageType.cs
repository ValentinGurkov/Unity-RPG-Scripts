using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "New Damage Type", menuName = "Enum/Damage Type", order = 0)]
    public class DamageType : ScriptableObject
    {
        [SerializeField] private string id;

        [SerializeField, ColorUsage(true, true)]
        private Color color;

        public Color Color => color;
    }
}