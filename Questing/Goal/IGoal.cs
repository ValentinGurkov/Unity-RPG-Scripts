using System;

namespace RPG.Questing {
  public interface IGoal {
    event Action<Stage, Goal> onComplete;
    bool Completed { get; set; }
    string Description { get; set; }
    int CurrentAmount { get; set; }
    Stage Stage { get; set; }
    int RequiredAmount { get; set; }
    void Init();
    bool Evaluate(string context);
  }
}
