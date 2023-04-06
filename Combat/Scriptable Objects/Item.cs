using RPG.Combat;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "Item", menuName = "Item/New Item", order = 0)]
    public class Item : ScriptableObject
    {
        [SerializeField] private string itemName;
        [SerializeField] private AnimatorOverrideController animatorOverride;
        [SerializeField] private WeaponConfig equippedPrefab;
        [SerializeField] private float itemWeight;
        [SerializeField] private ItemDefinition itemType;
        [SerializeField] private ItemSlot itemSlot;
        [SerializeField] private int itemAmount;
        [SerializeField] private int spawnChanceWeight;
        [SerializeField] private Sprite itemIcon;
        [SerializeField] private bool isEquipped;
        [SerializeField] private bool isInteractable;
        [SerializeField] private bool isStorable;
        [SerializeField] private bool isIndestructible;
        [SerializeField] private bool isQuestItme;
        [SerializeField] private bool isStackable;
        [SerializeField] private bool destroyOnUse;
        [SerializeField] private bool isUnique;

        public float ItemWeight => itemWeight;
        public ItemSlot ItemSlots => itemSlot;
        public ItemDefinition ItemType => itemType;
        public int ItemAmount => itemAmount;
        public int SpawnChanceWeight => spawnChanceWeight;
        public Sprite ItemIcon => itemIcon;
        public bool IsStorable => isStorable;
        public bool IsIndestructible => isIndestructible;
        public bool IsStackable => isStackable;
    }
}