namespace RPG.Questing {
  public class PickuppGoal : Goal {
    private string Pickup;
    public PickuppGoal(Quest quest, string pickup, string description, bool completed, int currentAmount, int requiredAmount) {
      this.Quest = quest;
      this.Pickup = pickup;
      this.Description = description;
      this.Completed = completed;
      this.CurrentAmount = currentAmount;
      this.RequiredAmount = requiredAmount;
    }

  }
}
