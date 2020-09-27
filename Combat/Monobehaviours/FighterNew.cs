using System;
using System.Collections;
using Attributes;
using Core;
using Movement;
using UnityEngine;
using static Util.Utility;

namespace Combat
{
    [RequireComponent(typeof(CharacterBehaviour))]
    public class FighterNew : MonoBehaviour, IAction
    {
        [SerializeField] private float timeBetweenAttacks = 1f;
        [SerializeField] private float attackRange = 3f;
        [SerializeField] private Weapon defaultWeapon;
        [SerializeField] private Transform leftHand;
        [SerializeField] private Transform rightHand;

        private IMouseInput _mouseInput;
        private CharacterMoverNavMesh _mover;
        private Animator _animator;
        private HealthNew _target;
        private ActionScheduler _actionScheduler;
        private CharacterBehaviour _characterBehaviour;
        private Coroutine _attackRoutine;
        private GameObject _equippedWeapon;
        private float _timeSinceLastAttack;
        private bool _attackFinished;
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

            //TODO what to do if there is no scriptable object defaultWeapon attached?
            if (defaultWeapon != null) SpawnWeapon();
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
            if (_target == null || _target.IsDead) return;
            if (_attackRoutine != null) StopCoroutine(_attackRoutine);
            _actionScheduler.StartAction(this);
            _attackRoutine = StartCoroutine(PursueAndAttackTarget());
        }

        public void Cancel()
        {
            _target = null;
            StopAttack();
            _mover.Cancel();
            OnTargetStatusChanged?.Invoke();
        }

        //Called by on animation exit event
        public void OnAttackFinished()
        {
            _attackFinished = true;
        }

        public void EquipWeapon(Weapon weapon)
        {
            defaultWeapon = weapon;
            SpawnWeapon();
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return IsTargetInRange(transform, targetTransform, defaultWeapon.Range);
        }


        private IEnumerator PursueAndAttackTarget()
        {
            while (_target != null && !IsTargetInRange(transform, _target.transform, attackRange))
            {
                _mover.Move(_target.transform.position);
                yield return null;
            }

            if (_target == null) yield break;
            AttackBehaviour();
            while (_mouseInput.IsHoldingMouseButton)
            {
                if (_attackFinished)
                {
                    AttackBehaviour();
                }

                yield return null;
            }

            _attackRoutine = null;
        }

        private void MakeEyeContact()
        {
            _characterBehaviour.LookAtTarget(_target.transform);
            _target.GetComponent<CharacterBehaviour>().LookAtTarget(gameObject.transform);
        }

        private void AttackBehaviour()
        {
            if (_target == null) return;
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
            bool isTargetDead = defaultWeapon.Attack(_target, _equippedWeapon.transform);
            if (isTargetDead) _target = null;
            OnTargetStatusChanged?.Invoke();
        }

        // Animation Trigger
        private void Shoot()
        {
            Hit();
        }

        private void SpawnWeapon()
        {
            _equippedWeapon = defaultWeapon.Spawn(leftHand, rightHand, _animator);
            attackRange = defaultWeapon.Range;
            timeBetweenAttacks = defaultWeapon.Cooldown;
        }
    }
}