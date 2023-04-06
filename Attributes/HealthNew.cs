using System;
using Core;
using Events;
using Saving;
using Stats;
using UnityEngine;
using UnityEngine.Events;
using Util;

namespace Attributes
{
    [RequireComponent(typeof(BaseStats), typeof(ActionScheduler))]
    public class HealthNew : MonoBehaviour, ISaveable
    {
        [SerializeField] private Enums enums;
        [SerializeField] private UnityEvent<float, bool, Color> takeDamageEvent;
        [SerializeField] private GameEvent dieEvent;
        private BaseStats _baseStats;
        private ActionScheduler _actionScheduler;
        private Animator _animator;
        private LazyValue<float> _health;
        private bool _isDead;
        private float _maxHealth;

        private static readonly int s_Die = Animator.StringToHash("Die");

        public float Health => _health.Value;

        public float MaxHealth => _maxHealth;

        public bool IsDead => _isDead;

        public float Fraction => _health.Value / _maxHealth;
        public event Action OnHealthUpdate;


        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _baseStats = GetComponent<BaseStats>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _health = new LazyValue<float>(GetHeath);
            _maxHealth = GetHeath();
        }

        private void Start()
        {
            _health.ForceInit();
        }

        private void Die()
        {
            if (_isDead) return;
            _isDead = true;
            _actionScheduler.CancelCurrentAction();
            _animator.SetTrigger(s_Die);
        }

        private void AwardExperience(GameObject instigator)
        {
            var experience = instigator.GetComponent<Experience>();
            if (experience == null) return;

            experience.GainExperience(_baseStats.GetStat(enums.Stats[Constants.Stats.ExperienceReward]));
        }

        private float GetHeath()
        {
            return _baseStats.GetStat(enums.Stats[Constants.Stats.Health]);
        }

        public void TakeDamage(GameObject instigator, float damage, bool isCritical, Color color)
        {
            _health.Value = Mathf.Max(_health.Value - damage, 0);
            OnHealthUpdate?.Invoke();
            takeDamageEvent?.Invoke(damage, isCritical, color);
            if (!Mathf.Approximately(_health.Value, 0)) return;
            dieEvent?.Raise();
            Die();
            AwardExperience(instigator);
        }

        public void Heal(float healthPercentToRestore)
        {
            _health.Value = Mathf.Min(_health.Value + _maxHealth * healthPercentToRestore / 100,
                _maxHealth);
            OnHealthUpdate?.Invoke();
        }


        public object CaptureState()
        {
            return _health.Value;
        }

        public void RestoreState(object state)
        {
            _health.Value = (float) state;
            if (_health.Value <= 0) Die();
        }
    }
}