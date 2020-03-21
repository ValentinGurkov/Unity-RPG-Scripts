using UnityEngine;

namespace RPG.Questing
{
    [System.Serializable]
    public class PickupGoalS : Goal, IGoal
    {
        [Tooltip("string or substring to match against")] [SerializeField]
        private string pickup;

        public string Pickup => pickup;

        public bool Evaluate(string item)
        {
            if (!item.Contains(Pickup)) return false;
            CurrentAmount++;
            return base.Evaluate();
        }
    }
}