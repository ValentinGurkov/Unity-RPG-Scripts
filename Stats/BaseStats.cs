using Stats;
using UnityEngine;
using Util;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)] [SerializeField] private int startingLevel = 1;
        [SerializeField] private bool shouldUseModifiers;
        [SerializeField] private CharacterClass characterClass = CharacterClass.Player;
        [SerializeField] private Progression progression;
        [SerializeField] private GameObject levelUpParticleEffect;
        private LazyValue<int> _currentLevel;
        private Experience _experience;

        public delegate bool LevelUpHandler();

        public event LevelUpHandler OnLevelUp;
        public CharacterClass CharacterClass => characterClass;
        public int Level => _currentLevel.Value;

        private void Awake()
        {
            _experience = GetComponent<Experience>();
            _currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            _currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (_experience != null) _experience.OnExperienceGained += UpdateLevel;
        }

        private void OnDisable()
        {
            if (_experience != null) _experience.OnExperienceGained -= UpdateLevel;
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel <= _currentLevel.Value) return;
            _currentLevel.Value = newLevel;
            if (OnLevelUp?.Invoke() == true) LevelUpEffect();
        }

        private float GetAdditiveModifiers(Stat stat)
        {
            if (!shouldUseModifiers) return 0;

            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }

            return total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers)
            {
                return 0;
            }

            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }

            return total;
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        private int CalculateLevel()
        {
            if (_experience == null)
            {
                return startingLevel;
            }

            float currentXP = _experience.ExperiencePoints;
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for (int level = 1; level <= penultimateLevel; level++)
            {
                float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if (XPToLevelUp > currentXP)
                {
                    return level;
                }
            }

            return penultimateLevel + 1;
        }

        public float GetStat(Stat stat)
        {
            return (progression.GetStat(stat, characterClass, _currentLevel.Value) + GetAdditiveModifiers(stat)) *
                   (1 + GetPercentageModifier(stat) / 100);
        }
    }
}