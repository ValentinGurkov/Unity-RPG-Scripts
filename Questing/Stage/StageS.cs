using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Questing {
    [Serializable]
    public class StageS {
        [SerializeField] private bool active;
        [SerializeField] private bool completed;
        [SerializeField] private string description;
        [SerializeReference]
        [SerializeReferenceButton] private List<IGoalS> goals;

        public bool Active => active;
        public string Description => description;
        public bool Completed => completed;
        public List<IGoalS> Goals => goals;

        public event Action<StageS> onStageActivated;
        public event Action<StageS> onStageCompleted;

        public void Init() {
            for (int i = 0; i < Goals.Count; i++) {
                Goals[i].onGoalCompleted += Evalute;
            }
        }

        public void Activate() {
            active = true;
            onStageActivated?.Invoke(this); // maybe find a way to not use ?
        }

        public void Complete() {
            active = false;
            completed = true;
            for (int i = 0; i < Goals.Count; i++) {
                Goals[i].Completed = true;
            }
        }

        public void Evalute(GoalS lastCompletedGoal) {
            bool goalsCompleted = true;

            for (int goal = 0; goal < Goals.Count; goal++) {
                if (!Goals[goal].Completed) {
                    goalsCompleted = false;
                    break;
                }
            }
            completed = goalsCompleted;

            if (completed) {
                Debug.Log("Stage has been completed!");
                active = false;
                onStageCompleted(this);
                Delegate[] activatedDelegates = onStageActivated.GetInvocationList();
                Delegate[] completedDelegates = onStageCompleted.GetInvocationList();
                for (int i = 0; i < activatedDelegates.Length; i++) {
                    onStageActivated -= activatedDelegates[i] as Action<StageS>;
                }
                for (int i = 0; i < completedDelegates.Length; i++) {
                    onStageCompleted -= completedDelegates[i] as Action<StageS>;
                }
            }
        }
    }

}