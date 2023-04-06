using Core;
using UnityEngine;
using Util;

namespace Stats
{
    [RequireComponent(typeof(Experience))]
    public class BaseStats : MonoBehaviour
    {
        [SerializeField] private Enums enums;
        [Range(1, 99)] [SerializeField] private int startingLevel = 1;
        [SerializeField] private bool shouldUseModifiers;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private ProgressionNew progression;
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

        private int CalculateLevel()
        {
            if (_experience == null) return startingLevel;

            float currentXP = _experience.ExperiencePoints;
            int penultimateLevel = progression.GetLevels(enums.Stats[Constants.Stats.ExperienceToLevelUp], characterClass);
            for (var level = 1; level <= penultimateLevel; level++)
            {
                float xpToLevelUp = progression.GetStat(enums.Stats[Constants.Stats.ExperienceToLevelUp], characterClass, level);
                if (xpToLevelUp > currentXP) return level;
            }

            return penultimateLevel + 1;
        }

        private void LevelUpEffect()
        {
            //TODO make this work via object pooler
            if (levelUpParticleEffect != null) Instantiate(levelUpParticleEffect, transform);
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
            if (!shouldUseModifiers) return 0;

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

        public float GetStat(Stat stat)
        {
            return (progression.GetStat(stat, characterClass, _currentLevel.Value) + GetAdditiveModifiers(stat)) *
                   (1 + GetPercentageModifier(stat) / 100);
        }
    }
}