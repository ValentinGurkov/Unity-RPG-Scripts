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
            if (lastStageIndex < Stages.Count - 1) {
                Stages[lastStageIndex + 1].Activate();
            }
        }

        public void CompleteQuest() {
            onComplete();
            GiveReward();
            GiveExperience();
        }

        public void RefreshRefernces() {
            player = GameObject.FindWithTag("Player");
            experience = player.GetComponent<Experience>();
        }

        public void SetActiveStage(int stage, bool[] goalsCompleted, int[] goalsCurrentAmount) {
            for (int i = 0; i < Stages.Count; i++) {
                if (i == stage) {
                    Stages[i].Activate();
                } else {
                    Stages[i].Active = false;
                    if (i < stage) {
                        Stages[i].Completed = true;
                    }
                }
            }
            if (goalsCompleted.Length != 0 && goalsCurrentAmount.Length != 0) {
                for (int i = 0; i < Stages[stage].Goals.Count; i++) {
                    Stages[stage].Goals[i].Completed = goalsCompleted[i];
                    Stages[stage].Goals[i].CurrentAmount = goalsCurrentAmount[i];
                }
            }
        }

        private void GiveReward() {
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
