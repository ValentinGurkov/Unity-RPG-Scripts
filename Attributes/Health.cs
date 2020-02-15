﻿using System;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using RPG.Util;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes {

    public class Health : MonoBehaviour, ISaveable {

        [SerializeField] private float regenerationPercentage = 70f;
        [SerializeField] private TakeDamageEvent takeDamage = default;
        [SerializeField] private bool startDead = default;
        [SerializeField] private OnDieEvent onDie = default;

        private const string DIE_TRIGGER = "die";
        private LazyValue<float> healthPoints;
        private Animator animator;
        private ActionScheduler actionScheduler;
        private BaseStats baseStats;

        public event Action onHealthUpdate;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float> { }

        [System.Serializable]
        public class OnDieEvent : UnityEvent<String> { }

        public bool IsDead { get; private set; } = false;
        public float Percentage => Fraction * 100;
        public float HealthPoints => healthPoints.value;
        public float MaxHealthPoints => baseStats.GetStat(Stat.Health);
        public float Fraction => healthPoints.value / baseStats.GetStat(Stat.Health);

        private void Awake() {
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
            baseStats = GetComponent<BaseStats>();
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private void Start() {
            healthPoints.ForceInit();
            if (startDead) {
                Die();
            }
        }

        private void OnEnable() {
            baseStats.onLevelUp += RegenerateHealth;
        }

        private void OnDisable() {
            baseStats.onLevelUp -= RegenerateHealth;
        }

        private float GetInitialHealth() {
            return baseStats.GetStat(Stat.Health);
        }

        public void TakeDamage(GameObject instigator, float damage) {
            if (IsDead) {
                return;
            }
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            takeDamage?.Invoke(damage);
            onHealthUpdate?.Invoke();
            if (healthPoints.value == 0) {
                onDie?.Invoke(gameObject.name);
                Die();
                AwardExperience(instigator);
            }
        }

        public void Heal(float healthPercentToRestore) {
            healthPoints.value = Mathf.Min(healthPoints.value + (MaxHealthPoints * healthPercentToRestore / 100), MaxHealthPoints);
            onHealthUpdate?.Invoke();
        }

        public object CaptureState() {
            return healthPoints.value;
        }

        public void RestoreState(object state) {
            healthPoints.value = (float) state;
            if (healthPoints.value <= 0) {
                Die();
            }
        }

        private void AwardExperience(GameObject instigator) {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) {
                return;
            }

            experience.GainExperience(baseStats.GetStat(Stat.ExperienceReward));
        }

        private bool RegenerateHealth() {
            if (IsDead) {
                return false;
            }
            float regenHealthPoints = baseStats.GetStat(Stat.Health) * (regenerationPercentage / 100);
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
            onHealthUpdate?.Invoke();
            return true;
        }

        private void Die() {
            if (IsDead) {
                return;
            }
            animator.SetTrigger(DIE_TRIGGER);
            actionScheduler.CancelCurrentAction();
            IsDead = true;
        }
    }
}
