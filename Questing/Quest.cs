using System.Collections.Generic;
using RPG.Combat;
using RPG.Stats;
using UnityEngine;

namespace RPG.Questing {
    //[CreateAssetMenu(fileName = "Quest", menuName = "Quest/Create New  Quest", order = 0)]
    public class Quest : MonoBehaviour {
        public List<Goal> Goals { get; set; } = new List<Goal>();
        public string QuestName { get; set; }
        public string Description { get; set; }
        public int ExperienceReward { get; set; }
        public WeaponConfig ItemReward { get; set; }
        public bool Completed { get; set; }

        private GameObject player;
        private BaseStats baseStats;
        private Experience experience;

        private void Awake() {
            player = GameObject.FindWithTag("Player");
            baseStats = player.GetComponent<BaseStats>();
            experience = player.GetComponent<Experience>();
        }

        public void CheckGols() {
            bool goalsCompleted = true;

            for (int goal = 0; goal < Goals.Count; goal++) {
                if (!Goals[goal].Completed) {
                    goalsCompleted = false;
                    break;
                }
            }
            Completed = goalsCompleted;

            if (Completed) {
                GiveReward();
                GiveExperience();
            }
        }

        public void GiveReward() {
            Debug.Log("Giving quest reward");
            if (ItemReward != null) {
                Debug.Log("Giving item reward");
                // ItemReward.Spawn(...);
            }
            GiveExperience();
        }

        private void GiveExperience() {
            Debug.Log("giving experience reward");
            if (experience == null && ExperienceReward == 0) {
                return;
            }
            experience.GainExperience(ExperienceReward);

        }
    }

}
