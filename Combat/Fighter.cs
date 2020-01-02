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

namespace RPG.Combat {

    [RequireComponent(typeof(CharacterBehaviour))]
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider {
        [SerializeField] private Transform leftHandTransform = null;
        [SerializeField] private Transform rightHandTransform = null;
        [SerializeField] private WeaponConfig defaultWeapon = null;
        [SerializeField] private float timeBetweenAttacks = 1f;

        private const string ATTACK_TRIGGER = "attack";
        private const string STOP_ATTACK_TRIGGER = "stopAttack";
        private Health target;
        private Health prevTarget;
        private Mover mover;
        private ActionScheduler actionScheduler;
        private Animator animator;
        private BaseStats baseStats;
        private CharacterBehaviour characterBehaviour;
        private bool stopped;
        private float timeSinceLastAttack = Mathf.Infinity;
        private WeaponConfig currentWeaponConfig;
        private LazyValue<Weapon> currentWeapon;
        private CinematicControlRemover[] cinematicControlRemovers;

        public bool attackDisabled = false;
        public event Action updateTargetUI;
        public Health Target => target;

        private void Awake() {
            mover = GetComponent<Mover>();
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
            characterBehaviour = GetComponent<CharacterBehaviour>();
            animator = GetComponent<Animator>();
            baseStats = GetComponent<BaseStats>();
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            cinematicControlRemovers = FindObjectsOfType<CinematicControlRemover>();
        }

        private void Start() {
            currentWeapon.ForceInit();
        }

        private void OnEnable() {
            for (int i = 0; i < cinematicControlRemovers.Length; i++) {
                cinematicControlRemovers[i].onCinematicStart += ToggleAttack;
                cinematicControlRemovers[i].onCinematicEnd += ToggleAttack;
            }
        }

        private void OnDisable() {
            for (int i = 0; i < cinematicControlRemovers.Length; i++) {
                cinematicControlRemovers[i].onCinematicStart -= ToggleAttack;
                cinematicControlRemovers[i].onCinematicEnd -= ToggleAttack;
            }
        }

        private void Update() {
            timeSinceLastAttack += Time.deltaTime;
            if (target == null || target.IsDead || attackDisabled) {
                return;
            }

            if (CombatTargetChanged(target)) {
                StopAttack();
            }

            if (!GetIsInRange(target.transform)) {
                mover.MoveTo(target.transform.position, 1f);
                stopped = false;
            } else {
                if (!stopped) {
                    mover.Cancel();
                    stopped = true;
                }
                AttackBehaviour();
            }
        }

        private bool CombatTargetChanged(Health newTarget) {
            if (prevTarget == null) {
                prevTarget = newTarget;
                return false;
            }
            if (newTarget != prevTarget) {
                prevTarget = newTarget;
                return true;
            }
            return false;
        }

        private Weapon SetupDefaultWeapon() {
            return defaultWeapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        private void AttackBehaviour() {
            MakeEyeContact();
            if (timeSinceLastAttack > timeBetweenAttacks) {
                // This will trigger the animation Hit() event
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        private void MakeEyeContact() {
            characterBehaviour.LookAtTarget(target.transform);
            target.GetComponent<CharacterBehaviour>().LookAtTarget(gameObject.transform);
        }

        private void TriggerAttack() {
            animator.ResetTrigger(STOP_ATTACK_TRIGGER);
            animator.SetTrigger(ATTACK_TRIGGER);
        }

        private bool GetIsInRange(Transform targetTransform) {
            return IsTargetInRange(transform, targetTransform, currentWeaponConfig.Range);
        }

        // Animation Trigger
        private void Hit() {
            if (target != null) {

                if (currentWeapon.value != null) {
                    currentWeapon.value.OnHit(target);
                }
                float damage = baseStats.GetStat(Stat.Damage);
                if (currentWeaponConfig.HasProjectile()) {
                    currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage, updateTargetUI);
                } else {
                    target.TakeDamage(gameObject, damage);
                }
            }
            updateTargetUI?.Invoke();

        }

        // Animation Trigger
        private void Shoot() {
            Hit();
        }

        private void StopAttack() {
            animator.ResetTrigger(ATTACK_TRIGGER);
            animator.SetTrigger(STOP_ATTACK_TRIGGER);
        }

        public void EquipWeapon(WeaponConfig weapon) {
            currentWeaponConfig = weapon;
            currentWeapon.value = weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public bool CanAttack(GameObject target) {
            if (target == null || (!mover.CanMoveTo(target.transform.position) && !GetIsInRange(target.transform))) {
                return false;
            }

            Health targetToAttack = target.GetComponent<Health>();
            return targetToAttack != null && !targetToAttack.IsDead;
        }

        public void Attack(GameObject combatTarget) {
            actionScheduler.StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel() {
            StopAttack();
            target = null;
            mover.Cancel();
            updateTargetUI?.Invoke();

        }

        public object CaptureState() {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state) {
            string weaponName = (string) state;
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat) {
            if (stat == Stat.Damage) {
                yield return currentWeaponConfig.Damage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat) {
            if (stat == Stat.Damage) {
                yield return currentWeaponConfig.PercentageBous;
            }
        }

        private void ToggleAttack(bool disabled) {
            attackDisabled = disabled;
        }
    }
}
