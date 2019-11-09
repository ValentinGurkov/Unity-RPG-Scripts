using RPG.Questing;

public class Liberator : Quest {
  private void Awake() {
    QuestName = "Ultimate Slayer";
    Description = "Liberate the village from bandits.";
    ItemReward = null;
    ExperienceReward = 1000;
    Stage firstStage = new Stage(this);
    firstStage.AddGoal(new KillGoal(firstStage, "Windmill Bandit", 0, 1, "KIll the bandit at the windmill"));
    firstStage.AddGoal(new PickuppGoal(firstStage, "Sunflower", 0, 1, "Pick up the Sunflower at the windmill"));
    Stage secondStage = new Stage(this, false, false);
    secondStage.AddGoal(new KillGoal(secondStage, "Entrance", 0, 3, "Clear the bridge to the village from the bandits"));
    Stage thirdStage = new Stage(this, false, false);
    thirdStage.AddGoal(new KillGoal(thirdStage, "Campfire", 0, 2, "Retake the campfire area from the bandits"));
    Stage fourthStage = new Stage(this, false, false);
    fourthStage.AddGoal(new KillGoal(fourthStage, "Tough Bandit", 0, 1, "There's a very tough bandit guarding the village exit. Beware! "));
    fourthStage.AddGoal(new KillGoal(fourthStage, "Backside", 0, 2, "There are two more bandits near the old ruined castle"));
    Stage fifthStage = new Stage(this, false, false);
    fifthStage.AddGoal(new ConversationGoal(fifthStage, "Old Jerry", "Talk to Old Jerry"));

    Stages.Add(firstStage);
    Stages.Add(secondStage);
    Stages.Add(thirdStage);
    Stages.Add(fourthStage);
    Stages.Add(fifthStage);

    for (int i = 0; i < Stages.Count; i++) {
      Stages[i].Init(i);
    }
  }
}
