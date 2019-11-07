using RPG.Combat;
using RPG.Events;
using UnityEngine;

namespace RPG.Questing {
  public class PickuppGoal : Goal {
    private string Pickup;
    public PickuppGoal(Stage stage, string pickup, int currentAmount, int requiredAmount, string description = "", bool completed = false) {
      this.Stage = stage;
      this.Pickup = pickup;
      this.CurrentAmount = currentAmount;
      this.RequiredAmount = requiredAmount;
      this.Description = description;
      this.Completed = completed;
    }

    public override void Init() {
      base.Init();
      EventSystem.OnItemPickedUp += ItemPickedUp;
    }

    private void ItemPickedUp(PickupBase pickup) {
      Debug.Log($"Enemy has died: {pickup.name}!");
      if (pickup.name == Pickup) {
        this.CurrentAmount++;
        if (Evaluate()) {
          EventSystem.OnItemPickedUp -= ItemPickedUp;
        }
      }

    }
  }
}
