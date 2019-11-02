using System;
using UnityEngine;

namespace RPG.Questing {
    public class QuestManager : MonoBehaviour {

        public Quest AddQuest(string quest) {
            return gameObject.AddComponent(Type.GetType(quest)) as Quest;
        }
    }

}
