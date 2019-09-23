using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using RPG.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes {

    public class Health : MonoBehaviour, ISaveable {

        [SerializeField] private float regenerationPercentage = 70f;
        [SerializeField] private TakeDamageEvent takeDamage;
        [SerializeField] private UnityEvent onDie;

        private const string DIE_TRIGGER = "die";
        private LazyValue<float> healthPoints;
        private Animator animator;
        private ActionScheduler actionScheduler;
        private BaseStats baseStats;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float> {

        }

        private void Awake() {
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
            baseStats = GetComponent<BaseStats>();
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private void Start() {
            healthPoints.ForceInit();
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

        public bool IsDead { get; private set; } = false;

        public float Percentage {
            get {
                return Fraction * 100;
            }
        }

        public float HealthPoints {
            get {
                return healthPoints.value;
            }
        }

        public float MaxHealthPoints {
            get {
                return baseStats.GetStat(Stat.Health);
            }
        }

        public float Fraction {
            get {
                return healthPoints.value / baseStats.GetStat(Stat.Health);
            }
        }

        public void TakeDamage(GameObject instigator, float damage) {
            if (IsDead) {
                return;
            }
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            takeDamage.Invoke(damage);
            if (healthPoints.value == 0) {
                onDie.Invoke();
                Die();
                AwardExperience(instigator);
            }
        }

        private void AwardExperience(GameObject instigator) {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) {
                return;
            }

            experience.GainExperience(baseStats.GetStat(Stat.ExperienceReward));
        }

        private void RegenerateHealth() {
            float regenHealthPoints = baseStats.GetStat(Stat.Health) * (regenerationPercentage / 100);
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
        }

        private void Die() {
            if (IsDead) {
                return;
            }
            animator.SetTrigger(DIE_TRIGGER);
            actionScheduler.CancelCurrentAction();
            IsDead = true;
        }

        public void Heal(float healthPercentToRestore) {
            healthPoints.value = Mathf.Min(healthPoints.value + (MaxHealthPoints * healthPercentToRestore / 100), MaxHealthPoints);
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
    }
}
