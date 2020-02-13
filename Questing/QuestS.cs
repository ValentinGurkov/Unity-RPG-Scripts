using System;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Stats;
using RPG.Util;
using UnityEngine;

namespace RPG.Questing {
    [CreateAssetMenu(fileName = "QuestS", menuName = "Quest/New Quest", order = 0)]
    public class QuestS : ScriptableObject {
        [SerializeField] private string id;
        [SerializeField] private string questName;
        [SerializeField] private string description;
        [SerializeField] private int experienceReward;
        [SerializeField] private bool completed;
        [SerializeField] private bool assigned;
        [SerializeField] private bool active;
        [SerializeField] private WeaponConfig itemReward;
        [SerializeField] private List<StageS> stages;

        private GameObject player;
        private Experience experience;

        public string ID => id;
        public string QuestName => questName;
        public string Description => description;
        public bool Completed => completed;
        public int ExperienceReward => experienceReward;
        public bool Assigned => assigned;
        public bool Active => Active;
        public WeaponConfig ItemReward => itemReward;
        public List<StageS> Stages => stages;

        public event Action onQuestCompleted;

        private void GiveItemReward() {
            if (itemReward != null) {
                Debug.Log("Giving item reward");
                // TODO Item rewards after inventory system is implemented
            }
        }

        private void GiveExperienceReward() {
            if (experience == null || experienceReward == 0) {
                return;
            }
            Debug.Log($"Giving {experienceReward} experience reward");
            experience.GainExperience(experienceReward);
        }

        private void CompleteQuest() {
            Debug.Log($"Quest \"{this.name}\" has been completed!");
            completed = true;
            GiveItemReward();
            GiveExperienceReward();
            onQuestCompleted();
            Delegate[] delegates = onQuestCompleted.GetInvocationList();
            for (int i = 0; i < delegates.Length; i++) {
                onQuestCompleted -= delegates[i] as Action;
            }
        }

        public void Init() {
            player = GameObject.FindWithTag("Player");
            experience = player.GetComponent<Experience>();
            assigned = true;
            active = true;
            Debug.Log($"Quest {this.name} started!");
            for (int i = 0; i < stages.Count; i++) {
                stages[i].Init();
                stages[i].onStageCompleted += Evalute;
            }
            stages[0].Activate();
        }

        public void Evalute(StageS currentStage) {
            //check for slicing by current stage position
            bool stagesCompleted = true;
            for (int stage = 0; stage < stages.Count; stage++) {
                if (!stages[stage].Completed) {
                    stagesCompleted = false;
                    break;
                }
            }
            if (stagesCompleted) {
                CompleteQuest();
                return;
            }
            int currentStageIndex = stages.IndexOf(currentStage);
            if (currentStageIndex < stages.Count - 1) {
                stages[currentStageIndex + 1].Activate();
            }
        }

        public void SetActiveStage(int stage, bool[] goalsCompleted, int[] goalsCurrentAmount) {
            for (int i = 0; i < Stages.Count; i++) {
                if (i == stage) {
                    Stages[i].Activate();
                } else if (i < stage) {
                    Stages[i].Complete();
                }
            }
            if (goalsCompleted.Length != 0 && goalsCurrentAmount.Length != 0) {
                for (int i = 0; i < Stages[stage].Goals.Count; i++) {
                    Stages[stage].Goals[i].Completed = goalsCompleted[i];
                    Stages[stage].Goals[i].CurrentAmount = goalsCurrentAmount[i];
                }
            }
        }
    }
}
