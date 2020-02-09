using System;
using RPG.Util;
using UnityEngine;

namespace RPG.Questing {
    public class GoalS : ScriptableObject, IUnloadedSceneHandler {
        [SerializeField] private bool active;
        [SerializeField] private bool completed;
        [SerializeField] private string description;
        [SerializeField] private int currentAmmount = 0;
        public int CurrentAmount = 0;
        public int RequiredAmount = 1;

        public bool Active => active; //maybe we dont need active
        public string Description => description;
        public bool Completed => completed;

        public event Action<GoalS> onGoalCompleted;

        public bool Evaluate() {
            if (CurrentAmount >= RequiredAmount) {
                Complete();
            }

            return Completed;
        }

        protected void Complete() {
            completed = true;
            Debug.Log("Goal " + Description + " has been completed!");
            onGoalCompleted(this);
            Delegate[] delegates = onGoalCompleted.GetInvocationList();
            for (int i = 0; i < delegates.Length; i++) {
                onGoalCompleted -= delegates[i] as Action<GoalS>;
            }
        }

        public void OnSceneUnloaded() {
            active = false;
            completed = false;
        }
    }

}
