using System;
using RPG.Saving;
using RPG.UI;
using UnityEngine;

namespace RPG.Questing {

    public class QuestManager : MonoBehaviour, ISaveable {
        [SerializeField] private QuestToDisplay questHUD = default;

        private Quest latestQuest = null;
        private StageS latestStage = null;
        private QuestGiver questGiver = null;
        private string questGiverName;

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
            AttachEvents();
        }

        private void AttachEvents() {
            if (latestQuest != null) {
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
            }
        }

        public void OnPlayeAction(string context) {
            Debug.Log($"Player interacted with: {context}");
            if (latestStage != null && !latestQuest.Completed) {
                for (int i = 0; i < latestStage.Goals.Count; i++) {
                    if (!latestStage.Goals[i].Completed) {
                        latestStage.Goals[i].Evaluate(context);
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

        public void AddQuest(QuestGiver qg, Quest quest) {
            if (qg == null || quest == null) {
                return;
            }
            questGiverName = qg.name;
            questGiver = qg;
            latestQuest = Instantiate(quest);
            latestQuest.Init();
            AttachEvents();
            questHUD.UpdateQuestDisplay(latestStage);
        }

        public object CaptureState() {
            if (latestQuest != null) {
                // fix scene prefab overrides!
                QuestSaver.Save(latestQuest);
                return new Tuple<string, string, int>(latestQuest.ID, questGiverName, latestQuest.Stages.IndexOf(latestStage));
            }
            return new Tuple<string, string, int>(null, null, 0);
        }

        public void RestoreState(object state) {
            var t = (Tuple<string, string, int>) state;
            if (!string.IsNullOrEmpty(t.Item1)) {
                if (latestQuest == null) {
                    latestQuest = QuestSaver.Load(t.Item1);
                    questGiverName = t.Item2;
                    if (!string.IsNullOrEmpty(questGiverName)) {
                        GameObject qgGObj = GameObject.Find(questGiverName);
                        if (qgGObj != null) {
                            questGiver = qgGObj.GetComponent<QuestGiver>();
                        }
                    }
                    latestQuest.Init();
                    latestStage = latestQuest.Stages[t.Item3];
                    AttachEvents();
                    questHUD.UpdateQuestDisplay(latestStage);
                }
            }
        }
    }
}
