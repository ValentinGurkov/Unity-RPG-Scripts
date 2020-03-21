using System;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using RPG.Util;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float regenerationPercentage = 70f;
        [SerializeField] private TakeDamageEvent takeDamage;
        [SerializeField] private bool startDead;
        [SerializeField] private OnDieEvent onDie;

        private LazyValue<float> m_HealthPoints;
        private Animator m_Animator;
        private ActionScheduler m_ActionScheduler;
        private BaseStats m_BaseStats;
        private static readonly int s_Die = Animator.StringToHash("die");

        public event Action OnHealthUpdate;

        [Serializable]
        public class TakeDamageEvent : UnityEvent<float> { }

        [Serializable]
        public class OnDieEvent : UnityEvent<String> { }

        public bool IsDead { get; private set; }
        public float Percentage => Fraction * 100;
        public float HealthPoints => m_HealthPoints.Value;
        public float MaxHealthPoints => m_BaseStats.GetStat(Stat.Health);
        public float Fraction => m_HealthPoints.Value / m_BaseStats.GetStat(Stat.Health);

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_ActionScheduler = GetComponent<ActionScheduler>();
            m_BaseStats = GetComponent<BaseStats>();
            m_HealthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private void Start()
        {
            m_HealthPoints.ForceInit();
            if (startDead)
            {
                Die();
            }
        }

        private void OnEnable()
        {
            m_BaseStats.OnLevelUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            m_BaseStats.OnLevelUp -= RegenerateHealth;
        }

        private float GetInitialHealth()
        {
            return m_BaseStats.GetStat(Stat.Health);
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            if (IsDead)
            {
                return;
            }

            m_HealthPoints.Value = Mathf.Max(m_HealthPoints.Value - damage, 0);
            takeDamage?.Invoke(damage);
            OnHealthUpdate?.Invoke();
            if (m_HealthPoints.Value >= Mathf.Epsilon) return;
            onDie?.Invoke(gameObject.name);
            Die();
            AwardExperience(instigator);
        }

        public void Heal(float healthPercentToRestore)
        {
            m_HealthPoints.Value = Mathf.Min(m_HealthPoints.Value + (MaxHealthPoints * healthPercentToRestore / 100),
                MaxHealthPoints);
            OnHealthUpdate?.Invoke();
        }

        public object CaptureState()
        {
            return m_HealthPoints.Value;
        }

        public void RestoreState(object state)
        {
            m_HealthPoints.Value = (float) state;
            if (m_HealthPoints.Value <= 0)
            {
                Die();
            }
        }

        private void AwardExperience(GameObject instigator)
        {
            var experience = instigator.GetComponent<Experience>();
            if (experience == null)
            {
                return;
            }

            experience.GainExperience(m_BaseStats.GetStat(Stat.ExperienceReward));
        }

        private bool RegenerateHealth()
        {
            if (IsDead)
            {
                return false;
            }

            float regenHealthPoints = m_BaseStats.GetStat(Stat.Health) * (regenerationPercentage / 100);
            m_HealthPoints.Value = Mathf.Max(m_HealthPoints.Value, regenHealthPoints);
            OnHealthUpdate?.Invoke();
            return true;
        }

        private void Die()
        {
            if (IsDead)
            {
                return;
            }

            m_Animator.SetTrigger(s_Die);
            m_ActionScheduler.CancelCurrentAction();
            IsDead = true;
        }
    }
}