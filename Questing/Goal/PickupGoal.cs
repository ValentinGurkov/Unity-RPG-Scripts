using RPG.Combat;
using RPG.Events;
using UnityEngine;

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
