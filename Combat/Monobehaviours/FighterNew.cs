using System;
using System.Collections;
using System.Collections.Generic;
using Attributes;
using Core;
using Movement;
using Pooling;
using Saving;
using Stats;
using UnityEngine;
using Util;
using static Util.Utility;

namespace Combat
{
    [RequireComponent(typeof(CharacterBehaviour), typeof(HealthNew), typeof(ActionScheduler))]
    public class FighterNew : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] private Enums enums;
        [SerializeField] private float timeBetweenAttacks = 1f;
        [SerializeField] private float attackRange = 3f;
        [SerializeField] private WeaponConfig weapon;
        [SerializeField] private Transform leftHand;
        [SerializeField] private Transform rightHand;
        [SerializeField] private ObjectPooler objectPooler;


        private IMouseInput _mouseInput;
        private CharacterMoverNavMesh _mover;
        private Animator _animator;
        private HealthNew _target;
        private CharacterBehaviour _targetCharacterBehaviour;
        private CharacterBehaviour _characterBehaviour;
        private ActionScheduler _actionScheduler;
        private BaseStats _baseStats;
        private Coroutine _attackRoutine;
        private float _timeSinceLastAttack;
        private bool _attackFinished;
        private bool _isPlayer;
        private static readonly int s_Attack = Animator.StringToHash("Attack");
        private static readonly int s_StopAttack = Animator.StringToHash("StopAttack");

        public HealthNew Target => _target;
        public event Action OnTargetStatusChanged;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _mover = GetComponent<CharacterMoverNavMesh>();
            _mouseInput = GetComponent<IMouseInput>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _characterBehaviour = GetComponent<CharacterBehaviour>();
            _baseStats = GetComponent<BaseStats>();

            //TODO what to do if there is no scriptable object weapon attached?
            if (weapon != null) SpawnWeapon();

            _isPlayer = gameObject.CompareTag("Player");
        }

        private void Start()
        {
            objectPooler = FindObjectOfType<ObjectPooler>();
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
        }

        public bool CanAttack(GameObject target)
        {
            if (target == null || !_mover.CanMoveTo(target.transform.position) && !GetIsInRange(target.transform))
            {
                return false;
            }

            var targetToAttack = target.GetComponent<HealthNew>();
            return targetToAttack != null && !targetToAttack.IsDead;
        }


        public void Attack(GameObject target)
        {
            _target = target.GetComponent<HealthNew>();
            _targetCharacterBehaviour = _target.GetComponent<CharacterBehaviour>();
            if (_target == null || _target.IsDead) return;
            if (_attackRoutine != null) StopCoroutine(_attackRoutine);
            _attackRoutine = StartCoroutine(PursueAndAttackTarget());
        }

        public void Cancel()
        {
            _target = null;
            _targetCharacterBehaviour = null;
            StopAttack();
            _mover.Cancel();
            OnTargetStatusChanged?.Invoke();
        }

        //Called by on animation exit event
        public void OnAttackFinished()
        {
            _attackFinished = true;
        }

        public void EquipWeapon(WeaponConfig weaponConfig)
        {
            weapon = weaponConfig;
            SpawnWeapon();
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return IsTargetInRange(transform, targetTransform, weapon.Range);
        }


        private IEnumerator PursueAndAttackTarget()
        {
            while (_target != null && !IsTargetInRange(transform, _target.transform, attackRange))
            {
                _mover.StartMovement(_target.transform.position, 1f);
                yield return null;
            }

            if (_target == null || _target.IsDead) yield break;
            _actionScheduler.StartAction(this);
            AttackBehaviour();
            while (_isPlayer && _mouseInput.IsHoldingMouseButton)
            {
                if (_attackFinished) AttackBehaviour();

                yield return null;
            }

            _attackRoutine = null;
        }

        private void MakeEyeContact()
        {
            _characterBehaviour.LookAtTarget(_target.transform);
            //_targetCharacterBehaviour.LookAtTarget(gameObject.transform);
        }

        private void AttackBehaviour()
        {
            if (_target == null || _target.IsDead) return;
            MakeEyeContact();
            if (_timeSinceLastAttack < timeBetweenAttacks) return;
            // This will trigger the animation Hit() event
            _timeSinceLastAttack = 0;
            TriggerAttack();
        }


        private void TriggerAttack()
        {
            _animator.ResetTrigger(s_StopAttack);
            _animator.SetTrigger(s_Attack);
        }

        private void StopAttack()
        {
            _animator.ResetTrigger(s_Attack);
            _animator.SetTrigger(s_StopAttack);
        }

        // Animation Trigger
        private void Hit()
        {
            if (_target == null) return;
            /*TODO this can be rethought and instead of the fighter calculating base damage from level + weapon stats,
             it can only pass the base damage from level. This would make the modifier interface obsolete though.
             What if there are other things influencing damage (buffs, etc.)? Then then the interface makes sense.*/
            float damage = _baseStats.GetStat(enums.Stats[Constants.Stats.Damage]);
            weapon.Attack(gameObject, _target, damage);
            if (!_target.IsDead) return;
            _target = null;
            _targetCharacterBehaviour = null;
        }

        // Animation Trigger
        private void Shoot()
        {
            Hit();
        }

        private void SpawnWeapon()
        {
            weapon.Spawn(leftHand, rightHand, _animator, objectPooler);
            attackRange = weapon.Range;
            timeBetweenAttacks = weapon.Cooldown;
            weapon.OnAttackFinished += OnTargetStatusChanged;
        }

        public object CaptureState()
        {
            return weapon.name;
        }

        public void RestoreState(object state)
        {
            var weaponName = (string) state;
            var weaponConfig = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weaponConfig);
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == enums.Stats[Constants.Stats.Damage])
            {
                yield return weapon.Damage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == enums.Stats[Constants.Stats.Damage])
            {
                yield return weapon.PercentageBonus;
            }
        }
    }
}