using RPG.Questing;

public class UltimateSlayer : Quest {
  private void Awake() {
    QuestName = "Ultimate Slayer";
    Description = "Kill some things";
    ItemReward = null;
    ExperienceReward = 1;
    Goals.Add(new KillGoal(this, "Windmill Bandit", "KIll 1 Enemy Bandit", false, 0, 1));
    //Goals.Add(new KillGoal(this, CharacterClass.Archer, "KIll 1 Enemy Archer", false, 0, 1));
    for (int goal = 0; goal < Goals.Count; goal++) {
      Goals[goal].Init();
    }
  }
}
