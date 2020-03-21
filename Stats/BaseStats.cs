using RPG.Util;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)] [SerializeField] private int startingLevel = 1;
        [SerializeField] private bool shouldUseModifiers;
        [SerializeField] private CharacterClass characterClass = CharacterClass.Player;
        [SerializeField] private Progression progression;
        [SerializeField] private GameObject levelUpParticleEffect;
        private LazyValue<int> m_CurrentLevel;
        private Experience m_Experience;

        public delegate bool LevelUpHandler();

        public event LevelUpHandler OnLevelUp;
        public CharacterClass CharacterClass => characterClass;
        public int Level => m_CurrentLevel.Value;

        private void Awake()
        {
            m_Experience = GetComponent<Experience>();
            m_CurrentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            m_CurrentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (m_Experience != null)
            {
                m_Experience.OnExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (m_Experience != null)
            {
                m_Experience.OnExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel <= m_CurrentLevel.Value) return;
            m_CurrentLevel.Value = newLevel;
            if (OnLevelUp?.Invoke() == true)
            {
                LevelUpEffect();
            }
        }

        private float GetAdditiveModifiers(Stat stat)
        {
            if (!shouldUseModifiers)
            {
                return 0;
            }

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
            if (m_Experience == null)
            {
                return startingLevel;
            }

            float currentXP = m_Experience.ExperiencePoints;
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
            return (progression.GetStat(stat, characterClass, m_CurrentLevel.Value) + GetAdditiveModifiers(stat)) *
                   (1 + GetPercentageModifier(stat) / 100);
        }
    }
}