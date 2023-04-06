using System;
using System.Collections.Generic;
using Core;
using UnityEditor;
using UnityEngine;

namespace Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Systems/Progression", order = 0)]
    public class ProgressionNew : ScriptableObject
    {
        [SerializeField] private Enums enums;
        [SerializeField] private CharacterProgression[] characterClasses;
        private Dictionary<CharacterClass, Dictionary<Stat, float[]>> _lookupTable;


        private void OnEnable()
        {
            if (characterClasses == null && enums != null) characterClasses = new CharacterProgression[enums.CharacterClasses.Count];
        }


        [Serializable]
        private class CharacterProgression
        {
            [SerializeField] private CharacterClass characterClass;
            [SerializeField] private ProgressionStat[] stats;

            public CharacterClass CharacterClass => characterClass;
            public ProgressionStat[] Stats => stats;
        }

        [Serializable]
        private class ProgressionStat
        {
            [SerializeField] private Stat stat;
            [SerializeField] private float[] levels;

            public float[] Levels => levels;
            public Stat Stat => stat;
        }

        private void BuildLookup()
        {
            if (_lookupTable != null) return;

            _lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            for (int progressionClass = 0; progressionClass < characterClasses.Length; progressionClass++)
            {
                var statLookupTable = new Dictionary<Stat, float[]>();
                for (int progressionStat = 0;
                    progressionStat < characterClasses[progressionClass].Stats.Length;
                    progressionStat++)
                {
                    statLookupTable[characterClasses[progressionClass].Stats[progressionStat].Stat] =
                        characterClasses[progressionClass].Stats[progressionStat].Levels;
                }

                _lookupTable[characterClasses[progressionClass].CharacterClass] = statLookupTable;
            }
        }

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookup();
            float[] levels = _lookupTable[characterClass][stat];
            if (levels.Length < level)
            {
                return 0;
            }

            return levels[level - 1];
        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();
            float[] levels = _lookupTable[characterClass][stat];

            return levels.Length;
        }
    }
}