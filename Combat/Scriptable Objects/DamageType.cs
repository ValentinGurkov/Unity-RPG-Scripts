using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "New Damage Type", menuName = "Enums/Damage Type", order = 0)]
    public class DamageType : ScriptableObject
    {
        [SerializeField] private string id;

        [SerializeField, ColorUsage(true, true)]
        private Color color;

        public Color Color => color;
    }
}