using System.Collections.Generic;
using UnityEngine;

namespace RPG.Events
{
    [CreateAssetMenu(fileName = "GameEvent", menuName = "Events/New Game Event", order = 0)]
    public class GameEvent : ScriptableObject
    {
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<GameEventListener> m_EventListeners = new List<GameEventListener>();

        public void Raise(string context)
        {
            for (int i = m_EventListeners.Count - 1; i >= 0; i--)
            {
                m_EventListeners[i].OnEventRaised(context);
            }
        }

        public void RegisterListener(GameEventListener listener)
        {
            if (!m_EventListeners.Contains(listener))
            {
                m_EventListeners.Add(listener);
            }
        }

        public void UnregisterListener(GameEventListener listener)
        {
            if (m_EventListeners.Contains(listener))
            {
                m_EventListeners.Remove(listener);
            }
        }
    }
}