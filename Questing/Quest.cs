using System;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Stats;
using UnityEngine;

namespace RPG.Questing {
    //[CreateAssetMenu(fileName = "Quest", menuName = "Quest/Create New  Quest", order = 0)]
    public class Quest : MonoBehaviour {
        public List<Stage> Stages { get; set; } = new List<Stage>();
        public string QuestName { get; set; }
        public string Description { get; set; }
        public int ExperienceReward { get; set; }
        public WeaponConfig ItemReward { get; set; }
        public bool Completed { get; set; }
        public event Action onComplete;

        private GameObject player;
        private Experience experience;

        private void Start() {
            player = GameObject.FindWithTag("Player");
            experience = player.GetComponent<Experience>();
        }

        public void CheckStages(int lastStageIndex) {
            bool stagesCompleted = true;

            for (int stage = 0; stage < Stages.Count; stage++) {
                if (!Stages[stage].Completed) {
                    stagesCompleted = false;
                    break;
                }
            }
            Completed = stagesCompleted;

            if (Completed) {
                onComplete();
                GiveReward();
                GiveExperience();
            } else if (lastStageIndex < Stages.Count) {
                Stages[lastStageIndex + 1].Activate();;
            }
        }

        public void GiveReward() {
            Debug.Log("Giving quest reward");
            if (ItemReward != null) {
                Debug.Log("Giving item reward");
                /* TODO Item rewards after inventory system is implemented
                ItemReward.Spawn(...);
                */
            }
        }

        private void GiveExperience() {
            Debug.Log("Giving experience reward");
            if (experience == null && ExperienceReward == 0) {
                return;
            }
            experience.GainExperience(ExperienceReward);

        }
    }

}
