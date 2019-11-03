using RPG.Questing;

public class UltimateSlayer : Quest {
  private void Awake() {
    QuestName = "Ultimate Slayer";
    Description = "Liberate the village from bandits.";
    ItemReward = null;
    ExperienceReward = 100;
    Goals.Add(new KillGoal(this, "Windmill Bandit", "KIll the bandit at the windmill", false, 0, 1));
    Goals.Add(new PickuppGoal(this, "Sunflower", "Pick up the Sunflower at the windmill", false, 0, 1));
    for (int goal = 0; goal < Goals.Count; goal++) {
      Goals[goal].Init();
    }
  }
}
