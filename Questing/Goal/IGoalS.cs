using System;

namespace RPG.Questing {
    public interface IGoalS {
        event Action<GoalS> onGoalCompleted;
        bool Evaluate(string context);
    }
}
