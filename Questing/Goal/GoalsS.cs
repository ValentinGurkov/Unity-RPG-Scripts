using System;
using UnityEngine;

namespace RPG.Questing {
    [Serializable]
    public abstract class GoalS {
        [SerializeField] private bool active;
        [SerializeField] private string description;
        [SerializeField] private int currentAmmount = 0;
        [SerializeField] private int RequiredAmount = 1;

        public int CurrentAmount { get; set; } = 0;
        public bool Completed { get; set; } = false;
        public bool Active => active; //maybe we dont need active
        public string Description => description;

        public event Action<GoalS> onGoalCompleted;

        public bool Evaluate() {
            if (CurrentAmount >= RequiredAmount) {
                Complete();
            }

            return Completed;
        }

        protected void Complete() {
            Completed = true;
            Debug.Log("Goal " + Description + " has been completed!");
            onGoalCompleted(this);
            Delegate[] delegates = onGoalCompleted.GetInvocationList();
            for (int i = 0; i < delegates.Length; i++) {
                onGoalCompleted -= delegates[i] as Action<GoalS>;
            }
        }
    }

}
