using System;

namespace QuestingV1.Goal
{
    public interface IGoal
    {
        string Description { get; }
        bool Completed { get; set; }
        int CurrentAmount { get; set; }
        event Action<Goal> OnGoalCompleted;
        bool Evaluate(string context);
    }
}