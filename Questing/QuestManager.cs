using System;
using UnityEngine;

namespace RPG.Questing {

    public class QuestManager : MonoBehaviour {
        private Quest latestQuest;
        private Stage latestStage;

        public event Action<Stage> onQuestAdded;
        public event Action onQuestComplete;

        //TODO make quest stage to be index
        public Quest AddQuest(string quest) {
            latestQuest = gameObject.AddComponent(Type.GetType(quest)) as Quest;
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

            onQuestAdded(latestStage);
            return latestQuest;
        }

        private Stage GetActiveQuestStage(Quest quest) {
            for (int i = 0; i < quest.Stages.Count; i++) {
                if (quest.Stages[i].Active) {
                    return quest.Stages[i];
                }
            }

            return quest.Stages[0];
        }

        private void UpdateUIOnCompletedGoal(Stage stage, Goal goal) {
            goal.onComplete -= UpdateUIOnCompletedGoal;
            onQuestAdded(stage);
        }

        private void UpdateUIOnCompletedStage(Stage stage) {
            latestStage = stage;
            stage.onActive -= UpdateUIOnCompletedStage;
            onQuestAdded(stage);
        }
    }

}
