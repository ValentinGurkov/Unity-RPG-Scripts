using System;
using UnityEngine;

namespace RPG.Questing {
    [Serializable]
    public abstract class Goal {
        [SerializeField] private string description = default;
        [SerializeField] private bool completed = false;
        [SerializeField] private int currentAmount = 0;
        [SerializeField] private int requiredAmount = 1;

        public int CurrentAmount { get => currentAmount; set => currentAmount = value; }
        public bool Completed { get => completed; set => completed = value; }
        public int RequiredAmount => requiredAmount;
        public string Description => description;

        public event Action<Goal> onGoalCompleted;

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
                onGoalCompleted -= delegates[i] as Action<Goal>;
            }
        }
    }

}
