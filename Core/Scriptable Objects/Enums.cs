using System.Collections.Generic;
using Combat;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "EnumManager", menuName = "Manager/Enum Manager", order = 1)]
    public class Enums : SerializedScriptableObject
    {
        [SerializeField] private Dictionary<string, CursorType> cursorTypes;
        public Dictionary<string, CursorType> CursorTypes => cursorTypes;

        [SerializeField] private Dictionary<string, ItemDefinition> itemDefinitions;
        public Dictionary<string, ItemDefinition> ItemDefinitions => itemDefinitions;

        [SerializeField] private Dictionary<string, ItemSlot> itemSlots;
        public Dictionary<string, ItemSlot> ItemSlots => itemSlots;


        [SerializeField] private Dictionary<string, DamageType> damageTypes;
        public Dictionary<string, DamageType> DamageTypes => damageTypes;

        [SerializeField] private Dictionary<string, DamageTypeEffect> damageTypeEffects;
        public Dictionary<string, DamageTypeEffect> DamageTypeEffects => damageTypeEffects;
    }
}