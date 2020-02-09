using System;
using FullSerializer;
using RPG.Saving;
using RPG.UI;
using UnityEngine;

namespace RPG.Questing {

    public class QuestManagerS : MonoBehaviour, ISaveable {
        [SerializeField] private QuestToDisplay questHUD;

        private QuestS latestQuest = null;
        private StageS latestStage = null;
        private QuestGiverS questGiver = null;
        private string questName;
        private string questGiverName;

        public event Action<StageS> onQuestAdded;
        public event Action onQuestComplete;

        private void OnDisable() {
            if (latestQuest != null) {
                latestQuest.onQuestCompleted -= QuestHasBeenCompleted;
                for (int i = 0; i < latestQuest.Stages.Count; i++) {
                    latestQuest.Stages[i].onStageActivated -= StageHasBeenActivated;
                    for (int j = 0; j < latestQuest.Stages[i].Goals.Count; j++) {
                        latestQuest.Stages[i].Goals[j].onGoalCompleted -= questHUD.UpdateQuestDisplay;
                    }
                }
            }
        }

        private void OnEnable() {
            if (latestQuest != null) {
                latestQuest.onQuestCompleted += QuestHasBeenCompleted;
                for (int i = 0; i < latestQuest.Stages.Count; i++) {
                    latestQuest.Stages[i].onStageActivated += StageHasBeenActivated;
                    for (int j = 0; j < latestQuest.Stages[i].Goals.Count; j++) {
                        latestQuest.Stages[i].Goals[j].onGoalCompleted += questHUD.UpdateQuestDisplay;
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
            return (latestStage != null ? latestQuest.Stages.IndexOf(latestStage) : 0, goalsCompleted, goalsCurrentAmount);
        }

        public void OnPlayeAction(string context) {
            Debug.Log($"Player interacted with: {context}");
            if (latestStage != null && !latestQuest.Completed) {
                for (int i = 0; i < latestStage.Goals.Count; i++) {
                    if (!latestStage.Goals[i].Completed) {
                        (latestStage.Goals[i] as IGoalS).Evaluate(context);
                    }
                }
            }
        }

        public void QuestHasBeenCompleted() {
            questGiver.MarkQuestCompleted();
            questHUD.DisplayDefaultText();
        }

        public void StageHasBeenActivated(StageS stage) {
            latestStage = stage;
            questHUD.UpdateQuestDisplay(stage);
        }

        public void AddQuest(QuestGiverS qg, QuestS quest) {
            if (qg == null || quest == null) {
                return;
            }
            questGiver = qg;
            latestQuest = quest;
            latestQuest.Init();
            latestQuest.onQuestCompleted += QuestHasBeenCompleted;
            for (int i = 0; i < latestQuest.Stages.Count; i++) {
                latestQuest.Stages[i].onStageActivated += StageHasBeenActivated;
                if (latestQuest.Stages[i].Active) {
                    latestStage = latestQuest.Stages[i];
                }
                for (int j = 0; j < latestQuest.Stages[i].Goals.Count; j++) {
                    latestQuest.Stages[i].Goals[j].onGoalCompleted += questHUD.UpdateQuestDisplay;
                }
            }
            questHUD.UpdateQuestDisplay(latestStage);
        }

        public object CaptureState() {
            (int, bool[], int[]) stageInfo = GetActiveStageInfo();
            return new Tuple<string, string, int, bool[], int[]>(latestQuest.QuestName, questGiver.name, stageInfo.Item1, stageInfo.Item2, stageInfo.Item3);
        }

        public void RestoreState(object state) {
            var t = (Tuple<string, string, int, bool[], int[]>) state;
            questName = t.Item1;
            if (questName != null) {
                if (latestQuest == null) {
                    latestQuest = Resources.Load<QuestS>(questName);
                    questGiverName = t.Item2;
                    GameObject qgGObj = GameObject.Find(questGiverName);
                    if (qgGObj != null) {
                        QuestGiverS qg = qgGObj.GetComponent<QuestGiverS>();
                        qg.SetQuest(latestQuest);
                        questGiver = qg;
                    }
                }
            }
        }
    }
}
