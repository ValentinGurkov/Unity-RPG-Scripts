using System;
using UnityEngine;

namespace RPG.Questing
{
    [Serializable]
    public abstract class Goal
    {
        [SerializeField] private string description;
        [SerializeField] private bool completed;
        [SerializeField] private int currentAmount;
        [SerializeField] private int requiredAmount = 1;

        public int CurrentAmount
        {
            get => currentAmount;
            set => currentAmount = value;
        }

        public bool Completed
        {
            get => completed;
            set => completed = value;
        }

        public int RequiredAmount => requiredAmount;
        public string Description => description;

        public event Action<Goal> OnGoalCompleted;

        public bool Evaluate()
        {
            if (CurrentAmount >= RequiredAmount)
            {
                Complete();
            }

            return Completed;
        }

        protected void Complete()
        {
            Completed = true;
            Debug.Log("Goal " + Description + " has been completed!");
            if (OnGoalCompleted == null) return;
            OnGoalCompleted(this);
            Delegate[] delegates = OnGoalCompleted.GetInvocationList();
            for (int i = 0; i < delegates.Length; i++)
            {
                OnGoalCompleted -= delegates[i] as Action<Goal>;
            }
        }
    }
}