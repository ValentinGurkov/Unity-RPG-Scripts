using System;
using RPG.Saving;
using RPG.UI;
using UnityEngine;

namespace RPG.Questing
{
    public class QuestManager : MonoBehaviour, ISaveable
    {
        [SerializeField] private QuestToDisplay questHUD;

        private Quest m_LatestQuest;
        private StageS m_LatestStage;
        private QuestGiver m_QuestGiver;
        private string m_QuestGiverName;

        private void OnDisable()
        {
            if (m_LatestQuest == null) return;
            m_LatestQuest.OnQuestCompleted -= QuestHasBeenCompleted;
            for (int i = 0; i < m_LatestQuest.Stages.Count; i++)
            {
                m_LatestQuest.Stages[i].OnStageActivated -= StageHasBeenActivated;
                for (int j = 0; j < m_LatestQuest.Stages[i].Goals.Count; j++)
                {
                    m_LatestQuest.Stages[i].Goals[j].OnGoalCompleted -= questHUD.UpdateQuestDisplay;
                }
            }
        }

        private void OnEnable()
        {
            AttachEvents();
        }

        private void AttachEvents()
        {
            if (m_LatestQuest == null) return;
            m_LatestQuest.OnQuestCompleted += QuestHasBeenCompleted;
            for (int i = 0; i < m_LatestQuest.Stages.Count; i++)
            {
                m_LatestQuest.Stages[i].OnStageActivated += StageHasBeenActivated;
                if (m_LatestQuest.Stages[i].Active)
                {
                    m_LatestStage = m_LatestQuest.Stages[i];
                }

                for (int j = 0; j < m_LatestQuest.Stages[i].Goals.Count; j++)
                {
                    m_LatestQuest.Stages[i].Goals[j].OnGoalCompleted += questHUD.UpdateQuestDisplay;
                }
            }
        }

        public void OnPlayeAction(string context)
        {
            if (m_LatestStage == null || m_LatestQuest.Completed) return;
            for (int i = 0; i < m_LatestStage.Goals.Count; i++)
            {
                if (!m_LatestStage.Goals[i].Completed)
                {
                    m_LatestStage.Goals[i].Evaluate(context);
                }
            }
        }

        private void QuestHasBeenCompleted()
        {
            m_QuestGiver.MarkQuestCompleted();
            questHUD.DisplayDefaultText();
        }

        private void StageHasBeenActivated(StageS stage)
        {
            m_LatestStage = stage;
            questHUD.UpdateQuestDisplay(stage);
        }

        public void AddQuest(QuestGiver qg, Quest quest)
        {
            if (qg == null || quest == null)
            {
                return;
            }

            m_QuestGiverName = qg.name;
            m_QuestGiver = qg;
            m_LatestQuest = Instantiate(quest);
            m_LatestQuest.Init();
            AttachEvents();
            questHUD.UpdateQuestDisplay(m_LatestStage);
        }

        public object CaptureState()
        {
            if (m_LatestQuest == null) return new Tuple<string, string, int>(null, null, 0);
            // fix scene prefab overrides!
            QuestSaver.Save(m_LatestQuest);
            return new Tuple<string, string, int>(m_LatestQuest.ID, m_QuestGiverName,
                m_LatestQuest.Stages.IndexOf(m_LatestStage));
        }

        public void RestoreState(object state)
        {
            if (m_LatestQuest != null) return;
            (string questName, string questGiverName, int latestStage) = (Tuple<string, string, int>) state;
            if (string.IsNullOrEmpty(questName)) return;
            m_LatestQuest = QuestSaver.Load(questName);
            m_QuestGiverName = questGiverName;
            if (!string.IsNullOrEmpty(m_QuestGiverName))
            {
                GameObject qgGObj = GameObject.Find(m_QuestGiverName);
                if (qgGObj != null)
                {
                    m_QuestGiver = qgGObj.GetComponent<QuestGiver>();
                }
            }

            m_LatestQuest.Init();
            m_LatestStage = m_LatestQuest.Stages[latestStage];
            AttachEvents();
            questHUD.UpdateQuestDisplay(m_LatestStage);
        }
    }
}