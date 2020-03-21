using System;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] private float experiencePoints;

        public float ExperiencePoints => experiencePoints;
        public event Action OnExperienceGained;

        public void GainExperience(float xp)
        {
            experiencePoints += xp;
            OnExperienceGained?.Invoke();
        }

        public object CaptureState()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float) state;
        }
    }
}