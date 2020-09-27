using System;
using System.Collections.Generic;
using QuestingV1.Stage;
using RPG.Combat;
using RPG.Stats;
using UnityEngine;

namespace QuestingV1
{
    [CreateAssetMenu(fileName = "QuestS", menuName = "Quest/New Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string questName;
        [SerializeField] private string description;
        [SerializeField] private int experienceReward;
        [SerializeField] private bool completed;
        [SerializeField] private bool assigned;
        [SerializeField] private bool active;
        [SerializeField] private WeaponConfig itemReward;
        [SerializeField] private List<StageS> stages;

        private GameObject m_Player;
        private Experience m_Experience;

        public string ID => id;
        public string QuestName => questName;
        public string Description => description;
        public bool Completed => completed;
        public int ExperienceReward => experienceReward;
        public bool Assigned => assigned;
        public bool Active => active;
        public WeaponConfig ItemReward => itemReward;
        public List<StageS> Stages => stages;

        public event Action OnQuestCompleted;

        private void GiveItemReward()
        {
            if (itemReward != null)
            {
                // TODO Item rewards after inventory system is implemented
            }
        }

        private void GiveExperienceReward()
        {
            if (m_Experience == null || experienceReward == 0)
            {
                return;
            }

            m_Experience.GainExperience(experienceReward);
        }

        private void CompleteQuest()
        {
            completed = true;
            GiveItemReward();
            GiveExperienceReward();
            if (OnQuestCompleted == null) return;
            OnQuestCompleted();
            Delegate[] delegates = OnQuestCompleted.GetInvocationList();
            for (int i = 0; i < delegates.Length; i++)
            {
                OnQuestCompleted -= delegates[i] as Action;
            }
        }

        public void Init()
        {
            m_Player = GameObject.FindWithTag("Player");
            m_Experience = m_Player.GetComponent<Experience>();
            assigned = true;
            active = true;
            for (int i = 0; i < stages.Count; i++)
            {
                stages[i].Init();
                stages[i].OnStageCompleted += Evaluate;
            }

            stages[0].Activate();
        }

        private void Evaluate(StageS currentStage)
        {
            //check for slicing by current stage position
            var stagesCompleted = true;
            for (int stage = 0; stage < stages.Count; stage++)
            {
                if (stages[stage].Completed) continue;
                stagesCompleted = false;
                break;
            }

            if (stagesCompleted)
            {
                CompleteQuest();
                return;
            }

            int currentStageIndex = stages.IndexOf(currentStage);
            if (currentStageIndex < stages.Count - 1)
            {
                stages[currentStageIndex + 1].Activate();
            }
        }

        public void SetActiveStage(int stage, bool[] goalsCompleted, int[] goalsCurrentAmount)
        {
            for (int i = 0; i < Stages.Count; i++)
            {
                if (i == stage)
                {
                    Stages[i].Activate();
                }
                else if (i < stage)
                {
                    Stages[i].Complete();
                }
            }

            if (goalsCompleted.Length == 0 || goalsCurrentAmount.Length == 0) return;

            for (int i = 0; i < Stages[stage].Goals.Count; i++)
            {
                Stages[stage].Goals[i].Completed = goalsCompleted[i];
                Stages[stage].Goals[i].CurrentAmount = goalsCurrentAmount[i];
            }
        }
    }
}