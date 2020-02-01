namespace RPG.Questing {
  public class PickupGoal : Goal, IGoal {
    public string Pickup;
    public PickupGoal(Stage stage, string pickup, int currentAmount, int requiredAmount, string description = "", bool completed = false) {
      Stage = stage;
      Pickup = pickup;
      CurrentAmount = currentAmount;
      RequiredAmount = requiredAmount;
      Description = description;
      Completed = completed;
    }

    public override void Init() {
      base.Init();
    }

    public bool Evaluate(string item) {
      if (Stage.Active && item.Contains(Pickup)) {
        CurrentAmount++;
        return base.Evaluate();
      }
      return false;
    }
  }
}
