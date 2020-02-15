using System;

namespace RPG.Questing {
    public interface IGoal {
        string Description { get; }
        bool Completed { get; set; }
        int CurrentAmount { get; set; }
        event Action<Goal> onGoalCompleted;
        bool Evaluate(string context);
    }
}
