using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats {
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject {
        static int Instances = 0;
        static int StatInstances = 0;

        [SerializeField] private CharacterProgression[] characterClasses = new CharacterProgression[Enum.GetNames(typeof(CharacterClass)).Length];
        private Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        [System.Serializable]
        class CharacterProgression {
            [HideInInspector][SerializeField] private string Name = Enum.GetName(typeof(CharacterClass), GetActiveInstances());
            [SerializeField] private ProgressionStat[] stats = new ProgressionStat[Enum.GetNames(typeof(Stat)).Length];
            private CharacterClass characteClass = (CharacterClass) GetActiveInstances();

            public CharacterClass CharacterClass => characteClass;
            public ProgressionStat[] Stats => stats;

            public CharacterProgression() {
                Instances++;
            }

            ~CharacterProgression() {
                Instances--;
            }

            public static int GetActiveInstances() {
                if (Instances >= Enum.GetNames(typeof(CharacterClass)).Length) {
                    Instances = 0;
                };
                return Instances;
            }

        }

        [System.Serializable]
        class ProgressionStat {
            [HideInInspector][SerializeField] private string StatName = Enum.GetName(typeof(Stat), GetActiveInstances());
            [SerializeField] private Stat stat = (Stat) GetActiveInstances();
            [SerializeField] private float[] levels = new float[5];

            public Stat Stat => stat;
            public float[] Levels => levels;

            public ProgressionStat() {
                if (StatInstances < Enum.GetNames(typeof(Stat)).Length) {
                    StatInstances++;
                }
            }

            ~ProgressionStat() {
                StatInstances--;
            }

            public float GetStat(int level) {
                if (level > levels.Length) {
                    return 0;
                }

                return levels[level - 1];
            }

            public static int GetActiveInstances() {
                if (StatInstances >= Enum.GetNames(typeof(Stat)).Length) {
                    StatInstances = 0;
                };
                return StatInstances;
            }
        }

        private void BuildLookup() {
            if (lookupTable != null) {
                return;
            }

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            for (int progressionClass = 0; progressionClass < characterClasses.Length; progressionClass++) {
                var statLookupTable = new Dictionary<Stat, float[]>();
                for (int progressionStat = 0; progressionStat < characterClasses[progressionClass].Stats.Length; progressionStat++) {
                    statLookupTable[characterClasses[progressionClass].Stats[progressionStat].Stat] = characterClasses[progressionClass].Stats[progressionStat].Levels;
                }
                lookupTable[characterClasses[progressionClass].CharacterClass] = statLookupTable;
            }
        }

        public float GetStat(Stat stat, CharacterClass characterClass, int level) {

            BuildLookup();
            float[] levels = lookupTable[characterClass][stat];
            if (levels.Length < level) {
                return 0f;
            }
            return levels[level - 1];
        }

        public int GetLevels(Stat stat, CharacterClass characterClass) {
            BuildLookup();
            float[] levels = lookupTable[characterClass][stat];

            return levels.Length;
        }
    }
}
