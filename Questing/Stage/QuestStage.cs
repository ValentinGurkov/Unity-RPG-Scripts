namespace RPG.Questing {
  public class QuestStage {
    public Goal[] StageGoals { get; set; }

    public bool IsStageComplete() {
      for (int i = 0; i < StageGoals.Length; i++) {
        if (!StageGoals[i].Completed) {
          return false;
        }
      }
      return true;
    }
  }
}
