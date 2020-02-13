using UnityEngine;

namespace RPG.Questing {
    [System.Serializable]
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
