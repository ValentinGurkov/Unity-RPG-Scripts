using RPG.Util;
using UnityEngine;

namespace RPG.Stats {
    public class BaseStats : MonoBehaviour {
        [Range(1, 99)][SerializeField] private int startingLevel = 1;
        [SerializeField] private bool shouldUseModifiers = false;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private Progression progression = null;
        [SerializeField] private GameObject levelUpParticleEffect = null;
        private LazyValue<int> currentLevel;
        private Experience experience;

        public delegate bool LevelUpHandler();
        public event LevelUpHandler onLevelUp;
        public CharacterClass CharacterClass => characterClass;
        public int Level => currentLevel.value;

        private void Awake() {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start() {
            currentLevel.ForceInit();
        }

        private void OnEnable() {
            if (experience != null) {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable() {
            if (experience != null) {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel() {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value) {
                currentLevel.value = newLevel;
                if (onLevelUp != null && onLevelUp()) {
                    LevelUpEffect();
                }
            }

        }

        private float GetAdditiveModifiers(Stat stat) {
            if (!shouldUseModifiers) {
                return 0;
            }
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>()) {
                foreach (float modifier in provider.GetAdditiveModifiers(stat)) {
                    total += modifier;
                }
            }

            return total;
        }

        private float GetPercentageModifier(Stat stat) {
            if (!shouldUseModifiers) {
                return 0;
            }
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>()) {
                foreach (float modifier in provider.GetPercentageModifiers(stat)) {
                    total += modifier;
                }
            }

            return total;
        }

        private void LevelUpEffect() {
            Instantiate(levelUpParticleEffect, transform);
        }

        private int CalculateLevel() {

            if (experience == null) {
                return startingLevel;
            }
            float currentXP = experience.ExperiencePoints;
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for (int level = 1; level <= penultimateLevel; level++) {
                float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if (XPToLevelUp > currentXP) {
                    return level;
                }
            }
            return penultimateLevel + 1;
        }

        public float GetStat(Stat stat) {
            return (progression.GetStat(stat, characterClass, currentLevel.value) + GetAdditiveModifiers(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }
    }
}
