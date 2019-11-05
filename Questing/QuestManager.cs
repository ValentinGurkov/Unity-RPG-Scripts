using System;
using UnityEngine;

namespace RPG.Questing {

    public class QuestManager : MonoBehaviour {
        private Quest latestQuest;

        public event Action<Quest> onQuestAdded;

        public Quest AddQuest(string quest) {
            latestQuest = gameObject.AddComponent(Type.GetType(quest)) as Quest;

            for (int i = 0; i < latestQuest.Goals.Count; i++) {
                latestQuest.Goals[i].onComplete += UpdateOnCompletedGoal;
            }
            onQuestAdded(latestQuest);
            return latestQuest;
        }

        public void UpdateOnCompletedGoal() {
            onQuestAdded(latestQuest);
        }
    }

}
