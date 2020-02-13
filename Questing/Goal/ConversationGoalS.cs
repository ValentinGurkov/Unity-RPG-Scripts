using UnityEngine;

namespace RPG.Questing {
    [System.Serializable]
    public class ConversationGoalS : GoalS, IGoalS {
        [SerializeField] private string target;

        public string Target => target;

        public bool Evaluate(string npc) {
            if (npc.Contains(Target)) {
                Complete();
                return true;
            }
            return false;
        }
    }

}
