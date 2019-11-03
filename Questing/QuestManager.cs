using System;
using UnityEngine;

namespace RPG.Questing {

    public class QuestManager : MonoBehaviour {
        private Quest latestQuest;

        public event Action<Quest> onQuestAdded;

        public Quest AddQuest(string quest) {
            latestQuest = gameObject.AddComponent(Type.GetType(quest)) as Quest;
            onQuestAdded(latestQuest);
            return latestQuest;
        }
    }

}
