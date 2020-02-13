using System;

namespace RPG.Questing {
    public interface IGoalS {
        string Description { get; }
        bool Completed { get; set; }
        int CurrentAmount { get; set; }
        event Action<GoalS> onGoalCompleted;
        bool Evaluate(string context);
    }
}
