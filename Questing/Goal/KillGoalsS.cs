using UnityEngine;

namespace RPG.Questing {
    [CreateAssetMenu(fileName = "GoalS", menuName = "Quest/Goals/New Kill Goal", order = 0)]
    public class KillGoalsS : GoalS, IGoalS {
        [SerializeField] private string enemy;

        string Enemy => enemy;

        public bool Evaluate(string enemy) {
            if (enemy.Contains(Enemy)) {
                CurrentAmount++;
                return base.Evaluate();
            }

            return false;
        }
    }

}
