using System;
using Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

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
        [SerializeField] private float percentageBonus;
        [SerializeField] private float criticalMultiplier;
        [SerializeField] private float criticalChance;
        [SerializeField] private DamageType damageType;
        [SerializeField] private DamageTypeEffect damageTypeEffect;
        [SerializeField] private ItemSlot itemSlot;


        [Header("Only for mixed type of damage")] [SerializeField]
        private float damageOverTimePercent;

        [SerializeField] private float instantDamagePercent;

        public event Action OnAttackFinished;

        public AnimatorOverrideController AnimatorOverride => animatorOverride;

        public float Cooldown => cooldown;
        public float Range => range;
        public float MinDamage => minDamage;
        public float MaxDamage => maxDamage;
        public float Damage => Random.Range(MinDamage, MaxDamage);

        public float PercentageBonus => percentageBonus;
        public float CriticalMultiplier => criticalMultiplier;
        public float CriticalChance => criticalChance;
        public ItemSlot ItemSlot => itemSlot;

        private (float, bool) GetFinalDamage(float baseDamage)
        {
            bool isCritical = Random.value < CriticalChance;

            if (isCritical) baseDamage *= CriticalMultiplier;

            return (baseDamage, isCritical);
        }

        //TODO should round damage?
        public virtual void Attack(GameObject instigator, HealthNew target, float baseDamage)
        {
            (float damage, bool isCritical) = GetFinalDamage(baseDamage);
            target.TakeDamage(instigator, damage, isCritical, damageType.Color);
            OnAttackFinished?.Invoke();
        }
    }
}