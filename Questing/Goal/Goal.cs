using System;
using UnityEngine;

namespace RPG.Questing {
    public class Goal {
        public event Action<Stage, Goal> onComplete;
        public bool Completed { get; set; } = false;
        public string Description { get; set; } = "";
        public int CurrentAmount { get; set; } = 0;
        public Stage Stage { get; set; } = null;
        public int RequiredAmount { get; set; } = 1;

        public virtual void Init() { }

        public bool Evaluate() {
            if (CurrentAmount >= RequiredAmount) {
                Complete();
            }

            return Completed;
        }

        protected void Complete() {
            Completed = true;
            Debug.Log("Goal " + Description + " has been completed!");
            onComplete?.Invoke(Stage, this);
            Stage.CheckGoals();
        }
    }
}
