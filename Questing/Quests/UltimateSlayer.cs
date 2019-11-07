using RPG.Questing;

public class UltimateSlayer : Quest {
  private void Awake() {
    QuestName = "Ultimate Slayer";
    Description = "Liberate the village from bandits.";
    ItemReward = null;
    ExperienceReward = 100;
    Stage firstStage = new Stage(this);
    Stage secondStage = new Stage(this, false, false, "Clear the bridge from bandits");
    firstStage.AddGoal(new KillGoal(firstStage, "Windmill Bandit", 0, 1, "KIll the bandit at the windmill"));
    firstStage.AddGoal(new PickuppGoal(firstStage, "Sunflower", 0, 1, "Pick up the Sunflower at the windmill"));
    secondStage.AddGoal(new KillGoal(firstStage, "Sunflower", 0, 1));

    Stages.Add(firstStage);
    Stages.Add(secondStage);

    for (int i = 0; i < Stages.Count; i++) {
      Stages[i].Init(i);
    }
  }
}
