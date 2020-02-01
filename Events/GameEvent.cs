using System.Collections.Generic;
using UnityEngine;

namespace RPG.Events {
    [CreateAssetMenu(fileName = "GameEvent", menuName = "Events/New Game Event", order = 0)]
    public class GameEvent : ScriptableObject {
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<GameEventListener> eventListeners =
            new List<GameEventListener>();

        public void Raise(string something) {
            for (int i = eventListeners.Count - 1; i >= 0; i--) {
                eventListeners[i].OnEventRaised(something);
            }

        }

        public void RegisterListener(GameEventListener listener) {
            if (!eventListeners.Contains(listener)) {
                eventListeners.Add(listener);
            }

        }

        public void UnregisterListener(GameEventListener listener) {
            if (eventListeners.Contains(listener)) {
                eventListeners.Remove(listener);
            }
        }
    }
}
