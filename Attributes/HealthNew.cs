using System;
using Events;
using UnityEngine;
using UnityEngine.Events;

namespace Attributes
{
    public class HealthNew : MonoBehaviour
    {
        [SerializeField] private float health;
        [SerializeField] private UnityEvent<float, bool, Color> takeDamageEvent;
        [SerializeField] private GameEvent dieEvent;
        private bool _isDead;
        private Animator _animator;
        private float _maxHealth;

        private static readonly int s_Die = Animator.StringToHash("Die");

        public float Health => health;

        public float MaxHealth => _maxHealth;

        public bool IsDead => _isDead;

        public float Fraction => health / _maxHealth;
        public event Action OnHealthUpdate;


        private void Awake()
        {
            _maxHealth = health;
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            health = 5f;
        }

        public void TakeDamage(float damage, bool isCritical, Color color)
        {
            health = Mathf.Max(health - damage, 0);
            OnHealthUpdate?.Invoke();
            takeDamageEvent?.Invoke(damage, isCritical, color);
            if (!Mathf.Approximately(health, 0)) return;
            dieEvent?.Raise();
            Die();
        }

        public void Heal(float healthPercentToRestore)
        {
            health = Mathf.Min(health + _maxHealth * healthPercentToRestore / 100,
                _maxHealth);
            OnHealthUpdate?.Invoke();
        }


        private void Die()
        {
            if (_isDead) return;
            _isDead = true;
            _animator.SetTrigger(s_Die);
        }
    }
}