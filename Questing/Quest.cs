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
        private Experience experience;

        private void Start() {
            player = GameObject.FindWithTag("Player");
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
