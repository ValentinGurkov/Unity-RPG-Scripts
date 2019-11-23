using RPG.Combat;
using RPG.Events;

namespace RPG.Questing {
  public class PickuppGoal : Goal {
    private string Pickup;
    public PickuppGoal(Stage stage, string pickup, int currentAmount, int requiredAmount, string description = "", bool completed = false) {
      Stage = stage;
      Pickup = pickup;
      CurrentAmount = currentAmount;
      RequiredAmount = requiredAmount;
      Description = description;
      Completed = completed;
    }

    public override void Init() {
      base.Init();
      EventSystem.OnItemPickedUp += ItemPickedUp;
    }

    private void ItemPickedUp(PickupBase pickup) {
      if (Stage.Active && pickup.name.Contains(Pickup)) {
        CurrentAmount++;
        if (Evaluate()) {
          EventSystem.OnItemPickedUp -= ItemPickedUp;
        }
      }

    }
  }
}
