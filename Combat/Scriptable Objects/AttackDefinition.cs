using Attributes;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "New Attack Definition", menuName = "Attack/Attack Definition", order = 1)]
    public class AttackDefinition : ScriptableObject
    {
        [SerializeField] private AnimatorOverrideController animatorOverride;
        [SerializeField] private float cooldown;
        [SerializeField] private float range;
        [SerializeField] private float minDamage;
        [SerializeField] private float maxDamage;
        [SerializeField] private float criticalMultiplier;
        [SerializeField] private float criticalChance;
        [SerializeField] private DamageType damageType;
        [SerializeField] private DamageTypeEffect damageTypeEffect;
        [SerializeField] private ItemSlot itemSlot;
        [SerializeField] private GameObject hitEffect;

        [Header("Only for mixed type of damage")] [SerializeField]
        private float damageOverTimePercent;

        [SerializeField] private float instantDamagePercent;

        public AnimatorOverrideController AnimatorOverride => animatorOverride;

        public float Cooldown => cooldown;
        public float Range => range;
        public float MinDamage => minDamage;
        public float MaxDamage => maxDamage;
        public float CriticalMultiplier => criticalMultiplier;
        public float CriticalChance => criticalChance;
        public ItemSlot ItemSlot => itemSlot;

        //see best class to use this weapon
        public bool Attack(HealthNew target, Transform weaponTransform)
        {
            float damage = Random.Range(MinDamage, MaxDamage);

            bool isCritical = Random.value < CriticalChance;

            if (isCritical) damage *= CriticalMultiplier;

            target.TakeDamage(damage, isCritical, damageType.Color);

            if (hitEffect == null) return target.IsDead;
            GameObject impactObj = Instantiate(hitEffect);
            impactObj.transform.position = target.GetComponent<Collider>().ClosestPoint(weaponTransform.position);
            impactObj.transform.rotation = weaponTransform.rotation;

            return target.IsDead;
        }
    }
}