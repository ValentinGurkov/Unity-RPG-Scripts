using System;
using RPG.Saving;
using UnityEngine;

namespace RPG.Questing {

    public class QuestManager : MonoBehaviour, ISaveable {
        private Quest latestQuest = null;
        private Stage latestStage = null;
        private string questName = null;
        private string questGiverName = null;

        public event Action<Stage> onQuestAdded;
        public event Action onQuestComplete;

        public Stage LatestStage => latestStage;

        private void OnDisable() {
            if (latestQuest != null) {
                for (int i = 0; i < latestQuest.Stages.Count; i++) {
                    latestQuest.Stages[i].onActive -= UpdateUIOnCompletedStage;
                    for (int j = 0; j < latestQuest.Stages[i].Goals.Count; j++) {
                        latestQuest.Stages[i].Goals[j].onComplete -= UpdateUIOnCompletedGoal;
                    }
                }
            }
        }

        private(int, bool[], int[]) GetActiveStageInfo() {
            int len = latestStage != null ? latestStage.Goals.Count : 0;
            bool[] goalsCompleted = new bool[len];
            int[] goalsCurrentAmount = new int[len];
            for (int i = 0; i < len; i++) {
                goalsCompleted[i] = latestStage.Goals[i].Completed;
                goalsCurrentAmount[i] = latestStage.Goals[i].CurrentAmount;
            }
            return (latestStage != null ? latestStage.Index : 0, goalsCompleted, goalsCurrentAmount);
        }

        private void UpdateUIOnCompletedGoal(Stage stage, Goal goal) {
            goal.onComplete -= UpdateUIOnCompletedGoal;
            if (onQuestAdded != null) {
                onQuestAdded(stage);
            }
        }

        private void UpdateUIOnCompletedStage(Stage stage) {
            stage.onActive -= UpdateUIOnCompletedStage;
            latestStage = stage;
            if (onQuestAdded != null) {
                onQuestAdded(stage);
            }
        }

        public Quest AddQuest(string questGiver, string quest) {
            questGiverName = questGiver;
            questName = quest;
            latestQuest = gameObject.AddComponent(Type.GetType(quest)) as Quest;
            if (latestQuest == null) {
                return null;
            }
            latestQuest.onComplete += onQuestComplete;
            for (int i = 0; i < latestQuest.Stages.Count; i++) {
                latestQuest.Stages[i].onActive += UpdateUIOnCompletedStage;
                if (latestQuest.Stages[i].Active) {
                    latestStage = latestQuest.Stages[i];
                }
                for (int j = 0; j < latestQuest.Stages[i].Goals.Count; j++) {
                    latestQuest.Stages[i].Goals[j].onComplete += UpdateUIOnCompletedGoal;
                }
            }

            if (onQuestAdded != null) {
                onQuestAdded(latestStage);
            }

            return latestQuest;
        }

        // used to update references between scenes
        public Stage Subscribe() {
            if (latestQuest != null) {
                latestQuest.onComplete += onQuestComplete;
            }
            return latestStage;
        }

        public object CaptureState() {
            (int, bool[], int[]) stageInfo = GetActiveStageInfo();
            return new Tuple<string, string, int, bool[], int[]>(questName, questGiverName, stageInfo.Item1, stageInfo.Item2, stageInfo.Item3);
        }

        public void RestoreState(object state) {
            var t = (Tuple<string, string, int, bool[], int[]>) state;
            questName = t.Item1;
            if (GetComponent(questName) == null) {
                questGiverName = t.Item2;
                GameObject qgGObj = GameObject.Find(questGiverName);
                if (qgGObj != null) {
                    QuestGiver qg = qgGObj.GetComponent<QuestGiver>();
                    latestQuest = AddQuest(qg.name, questName);
                    latestQuest.SetActiveStage(t.Item3, t.Item4, t.Item5);
                    qg.SetQuest(latestQuest);
                }
            }
        }

        public void Restore() {
            if (questGiverName != null && questName != null) {
                GameObject qgGObj = GameObject.Find(questGiverName);
                if (qgGObj != null) {
                    QuestGiver qg = qgGObj.GetComponent<QuestGiver>();
                    latestQuest.RefreshRefernces();
                    qg.SetQuest(latestQuest);
                }
            }
        }
    }

}
