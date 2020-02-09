using UnityEngine;

namespace RPG.Questing {
    [CreateAssetMenu(fileName = "GoalS", menuName = "Quest/Goals/New Pickup Goal", order = 0)]
    public class PickupGoalS : GoalS, IGoalS {
        [SerializeField] private string pickup;

        public string Pickup => pickup;

        public bool Evaluate(string item) {
            if (item.Contains(Pickup)) {
                CurrentAmount++;
                return base.Evaluate();
            }
            return false;
        }
    }

}
