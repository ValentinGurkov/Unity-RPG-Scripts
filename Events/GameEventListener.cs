using UnityEngine;
using UnityEngine.Events;

namespace RPG.Events
{
    public class GameEventListener : MonoBehaviour
    {
        [Tooltip("Event to register with.")] public GameEvent Event;

        [Tooltip("Response to invoke when Event is raised.")]
        public StringEvent Response;

        [System.Serializable]
        public class StringEvent : UnityEvent<string> { }

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised(string context)
        {
            Response.Invoke(context);
        }
    }
}