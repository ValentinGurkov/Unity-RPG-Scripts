﻿using System;
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

        public void OnPlayeAction(string context) {
            Debug.Log($"Player interacted with: {context}");
            if (latestStage != null) {
                for (int i = 0; i < latestStage.Goals.Count; i++) {
                    latestStage.Goals[i].Evaluate(context);
                }
            }
        }

        private void UpdateUIOnCompletedGoal(Stage stage, Goal goal) {
            goal.onComplete -= UpdateUIOnCompletedGoal;
            onQuestAdded?.Invoke(stage);

        }

        public void UpdateUIOnCompletedStage(Stage stage) {
            stage.onActive -= UpdateUIOnCompletedStage;
            latestStage = stage;
            onQuestAdded?.Invoke(stage);
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
            onQuestAdded?.Invoke(latestStage);

            return latestQuest;
        }

        public object CaptureState() {
            (int, bool[], int[]) stageInfo = GetActiveStageInfo();
            return new Tuple<string, string, int, bool[], int[]>(questName, questGiverName, stageInfo.Item1, stageInfo.Item2, stageInfo.Item3);
        }

        public void RestoreState(object state) {
            var t = (Tuple<string, string, int, bool[], int[]>) state;
            questName = t.Item1;
            if (questName != null) {
                if (GetComponent(questName) == null) {
                    questGiverName = t.Item2;
                    GameObject qgGObj = GameObject.Find(questGiverName);
                    if (qgGObj != null) {
                        QuestGiver qg = qgGObj.GetComponent<QuestGiver>();
                        qg.SetQuest(latestQuest);
                    }
                    latestQuest = AddQuest(questGiverName, questName);
                    latestQuest.SetActiveStage(t.Item3, t.Item4, t.Item5);
                }
            }
        }
    }
}
