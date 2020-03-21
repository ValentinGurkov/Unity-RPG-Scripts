using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Questing
{
    [Serializable]
    public class StageS
    {
        [SerializeField] private bool active;
        [SerializeField] private bool completed;
        [SerializeField] private string description;

        [SerializeReference] [SerializeReferenceButton]
        private List<IGoal> goals = default;

        public bool Active => active;
        public string Description => description;
        public bool Completed => completed;
        public List<IGoal> Goals => goals;

        public event Action<StageS> OnStageActivated;
        public event Action<StageS> OnStageCompleted;

        public void Init()
        {
            for (int i = 0; i < Goals.Count; i++)
            {
                Goals[i].OnGoalCompleted += Evalute;
            }
        }

        public void Activate()
        {
            active = true;
            OnStageActivated?.Invoke(this); // maybe find a way to not use ?
        }

        public void Complete()
        {
            active = false;
            completed = true;
            for (int i = 0; i < Goals.Count; i++)
            {
                Goals[i].Completed = true;
            }
        }

        public void Evalute(Goal lastCompletedGoal)
        {
            var goalsCompleted = true;

            for (int goal = 0; goal < Goals.Count; goal++)
            {
                if (Goals[goal].Completed) continue;
                goalsCompleted = false;
                break;
            }

            completed = goalsCompleted;

            if (!completed) return;
            active = false;
            if (OnStageCompleted == null) return;
            OnStageCompleted(this);
            Delegate[] completedDelegates = OnStageCompleted.GetInvocationList();
            for (int i = 0; i < completedDelegates.Length; i++)
            {
                OnStageCompleted -= completedDelegates[i] as Action<StageS>;
            }

            if (OnStageActivated == null) return;
            Delegate[] activatedDelegates = OnStageActivated.GetInvocationList();
            for (int i = 0; i < activatedDelegates.Length; i++)
            {
                OnStageActivated -= activatedDelegates[i] as Action<StageS>;
            }
        }
    }
}