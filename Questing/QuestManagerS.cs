using System;
using System.IO;
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

        public void AddQuest(QuestGiverS qg, QuestS quest) {
            if (qg == null || quest == null) {
                return;
            }
            questName = quest.QuestName;
            questGiverName = qg.name;
            questGiver = qg;
            latestQuest = Instantiate(quest);
            latestQuest.Init();
            questHUD.UpdateQuestDisplay(latestStage);
            AttachEvents();
        }

        public object CaptureState() {
            if (latestQuest != null) {
                // create a quest saver class!
                // fix scene prefab overrides!
                string json = JsonUtility.ToJson(latestQuest);
                File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + questName + ".json", json);
                return new Tuple<string, string, int>(questName, questGiverName, latestQuest.Stages.IndexOf(latestStage));
            }
            return new Tuple<string, string, int>("", "", 0);
        }

        public void RestoreState(object state) {
            var t = (Tuple<string, string, int>) state;
            questName = t.Item1;
            if (!string.IsNullOrEmpty(questName)) {
                if (latestQuest == null) {
                    if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + questName + ".json")) {
                        latestQuest = ScriptableObject.CreateInstance<QuestS>();
                        string json = File.ReadAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + questName + ".json");
                        JsonUtility.FromJsonOverwrite(json, latestQuest);
                    } else {
                        latestQuest = Resources.Load<QuestS>(questName);
                    }
                    questGiverName = t.Item2;
                    if (!string.IsNullOrEmpty(questGiverName)) {
                        GameObject qgGObj = GameObject.Find(questGiverName);
                        if (qgGObj != null) {
                            questGiver = qgGObj.GetComponent<QuestGiverS>();
                        }
                        latestQuest.Init();
                        latestStage = latestQuest.Stages[t.Item3];
                        questHUD.UpdateQuestDisplay(latestStage);
                        AttachEvents();
                    }
                }
            }
        }
    }
}
