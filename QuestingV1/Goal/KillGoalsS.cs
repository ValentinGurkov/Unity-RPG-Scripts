using UnityEngine;

namespace QuestingV1.Goal
{
    [System.Serializable]
    public class KillGoalsS : Goal, IGoal
    {
        [Tooltip("string or substring to match against")] [SerializeField]
        private string enemy;

        public string Enemy => enemy;

        public bool Evaluate(string enemy)
        {
            if (!enemy.Contains(Enemy)) return false;
            CurrentAmount++;
            return base.Evaluate();
        }
    }
}