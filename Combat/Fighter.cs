using System;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Cinematics;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Stats;
using RPG.Util;
using UnityEngine;
using static RPG.Util.Utility;

namespace RPG.Combat
{
    [RequireComponent(typeof(CharacterBehaviour))]
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] private Transform leftHandTransform;
        [SerializeField] private Transform rightHandTransform;
        [SerializeField] private WeaponConfig defaultWeapon;
        [SerializeField] private float timeBetweenAttacks = 1f;

        private Health m_Target;
        private Health m_PrevTarget;
        private Mover m_Mover;
        private ActionScheduler m_ActionScheduler;
        private Animator m_Animator;
        private BaseStats m_BaseStats;
        private CharacterBehaviour m_CharacterBehaviour;
        private bool m_Stopped;
        private float m_TimeSinceLastAttack = Mathf.Infinity;
        private WeaponConfig m_CurrentWeaponConfig;
        private LazyValue<Weapon> m_CurrentWeapon;
        private CinematicControlRemover[] m_CinematicControlRemovers;

        public bool attackDisabled;
        private static readonly int s_StopAttack = Animator.StringToHash("stopAttack");
        private static readonly int s_Attack = Animator.StringToHash("attack");
        public event Action UpdateTargetUi;
        public Health Target => m_Target;

        private void Awake()
        {
            m_Mover = GetComponent<Mover>();
            m_Animator = GetComponent<Animator>();
            m_ActionScheduler = GetComponent<ActionScheduler>();
            m_CharacterBehaviour = GetComponent<CharacterBehaviour>();
            m_Animator = GetComponent<Animator>();
            m_BaseStats = GetComponent<BaseStats>();
            m_CurrentWeaponConfig = defaultWeapon;
            m_CurrentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            m_CinematicControlRemovers = FindObjectsOfType<CinematicControlRemover>();
        }

        private void Start()
        {
            m_CurrentWeapon.ForceInit();
        }

        private void OnEnable()
        {
            for (int i = 0; i < m_CinematicControlRemovers.Length; i++)
            {
                m_CinematicControlRemovers[i].OnCinematicStart += ToggleAttack;
                m_CinematicControlRemovers[i].OnCinematicEnd += ToggleAttack;
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < m_CinematicControlRemovers.Length; i++)
            {
                m_CinematicControlRemovers[i].OnCinematicStart -= ToggleAttack;
                m_CinematicControlRemovers[i].OnCinematicEnd -= ToggleAttack;
            }
        }

        private void Update()
        {
            m_TimeSinceLastAttack += Time.deltaTime;
            if (m_Target == null || m_Target.IsDead || attackDisabled)
            {
                return;
            }

            if (CombatTargetChanged(m_Target))
            {
                StopAttack();
            }

            if (!GetIsInRange(m_Target.transform))
            {
                m_Mover.MoveTo(m_Target.transform.position, 1f);
                m_Stopped = false;
            }
            else
            {
                if (!m_Stopped)
                {
                    m_Mover.Cancel();
                    m_Stopped = true;
                }

                AttackBehaviour();
            }
        }

        private bool CombatTargetChanged(Health newTarget)
        {
            if (m_PrevTarget == null)
            {
                m_PrevTarget = newTarget;
                return false;
            }

            if (newTarget == m_PrevTarget) return false;
            m_PrevTarget = newTarget;
            return true;
        }

        private Weapon SetupDefaultWeapon()
        {
            return defaultWeapon.Spawn(rightHandTransform, leftHandTransform, m_Animator);
        }

        private void AttackBehaviour()
        {
            MakeEyeContact();
            if (!(m_TimeSinceLastAttack > timeBetweenAttacks)) return;
            // This will trigger the animation Hit() event
            TriggerAttack();
            m_TimeSinceLastAttack = 0;
        }

        private void MakeEyeContact()
        {
            m_CharacterBehaviour.LookAtTarget(m_Target.transform);
            m_Target.GetComponent<CharacterBehaviour>().LookAtTarget(gameObject.transform);
        }

        private void TriggerAttack()
        {
            m_Animator.ResetTrigger(s_StopAttack);
            m_Animator.SetTrigger(s_Attack);
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return IsTargetInRange(transform, targetTransform, m_CurrentWeaponConfig.Range);
        }

        // Animation Trigger
        private void Hit()
        {
            if (m_Target != null)
            {
                if (m_CurrentWeapon.Value != null)
                {
                    m_CurrentWeapon.Value.OnHit(m_Target);
                }

                float damage = m_BaseStats.GetStat(Stat.Damage);
                if (m_CurrentWeaponConfig.HasProjectile())
                {
                    m_CurrentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, m_Target, gameObject,
                        damage, UpdateTargetUi);
                }
                else
                {
                    m_Target.TakeDamage(gameObject, damage);
                }
            }

            UpdateTargetUi?.Invoke();
        }

        // Animation Trigger
        private void Shoot()
        {
            Hit();
        }

        private void StopAttack()
        {
            m_Animator.ResetTrigger(s_Attack);
            m_Animator.SetTrigger(s_StopAttack);
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            m_CurrentWeaponConfig = weapon;
            m_CurrentWeapon.Value = weapon.Spawn(rightHandTransform, leftHandTransform, m_Animator);
        }

        public bool CanAttack(GameObject target)
        {
            if (target == null || (!m_Mover.CanMoveTo(target.transform.position) && !GetIsInRange(target.transform)))
            {
                return false;
            }

            Health targetToAttack = target.GetComponent<Health>();
            return targetToAttack != null && !targetToAttack.IsDead;
        }

        public void Attack(GameObject combatTarget)
        {
            m_ActionScheduler.StartAction(this);
            m_Target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            StopAttack();
            m_Target = null;
            m_Mover.Cancel();
            UpdateTargetUi?.Invoke();
        }

        public object CaptureState()
        {
            return m_CurrentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            var weaponName = (string) state;
            var weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return m_CurrentWeaponConfig.Damage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return m_CurrentWeaponConfig.PercentageBonus;
            }
        }

        private void ToggleAttack(bool disabled)
        {
            attackDisabled = disabled;
        }
    }
}