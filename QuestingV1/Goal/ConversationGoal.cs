using UnityEngine;

namespace QuestingV1.Goal
{
    [System.Serializable]
    public class ConversationGoal : Goal, IGoal
    {
        [Tooltip("string or substring to match against")] [SerializeField]
        private string target = default;

        public string Target => target;

        public bool Evaluate(string npc)
        {
            if (!npc.Contains(Target)) return false;
            Complete();
            return true;
        }
    }
}