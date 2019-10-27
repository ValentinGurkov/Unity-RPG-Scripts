using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests {
    public class Goal : MonoBehaviour {
        public Quest Quest { get; set; }
        public bool Completed { get; set; } = false;
        public string Description { get; set; }
        public int CurrentAmount { get; set; }
        public int RequiredAmount { get; set; }

        public virtual void Init() { }

        public void Evaluate() {
            if (CurrentAmount >= RequiredAmount) {
                Complete();
            }
        }

        void Complete() {
            Quest.CheckGols();
            Debug.Log("Goal " + Description + " has been completed!");
            Completed = true;
        }
    }
}
