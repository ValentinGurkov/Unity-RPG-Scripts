using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Questing {
  public class Stage {
    protected Quest Quest { get; set; }
    public List<IGoal> Goals { get; set; } = new List<IGoal>();
    public bool Active { get; set; } = true;
    public bool Completed { get; set; } = false;
    public string Description { get; set; }
    public int Index { get; set; }
    public event Action<Stage> onActive;

    public Stage(Quest quest, IGoal[] goals, bool active = true, bool completed = false, string description = "") {
      Quest = quest;
      Goals = new List<IGoal>(goals.Length);
      Goals.AddRange(goals);
      Active = active;
      Completed = completed;
      Description = description;

      Init();
    }

    public Stage(Quest quest, bool active = true, bool completed = false, string description = "") {
      Quest = quest;
      Active = active;
      Completed = completed;
      Description = description;
    }

    public void AddGoal(IGoal goal) {
      Goals.Add(goal);
    }

    public void Init(int index = 0) {
      Index = index;
      for (int i = 0; i < Goals.Count; i++) {
        Goals[i].Init();
      }
    }

    public void Activate() {
      Active = true;
      onActive?.Invoke(this);
    }

    public void CheckGoals() {
      bool goalsCompleted = true;

      for (int goal = 0; goal < Goals.Count; goal++) {
        if (!Goals[goal].Completed) {
          goalsCompleted = false;
          break;
        }
      }
      Completed = goalsCompleted;

      if (Completed) {
        Active = false;
        Debug.Log("Stage " + Index + " has been completed!");
        Quest.CheckStages(Index);
      }
    }
  }
}
