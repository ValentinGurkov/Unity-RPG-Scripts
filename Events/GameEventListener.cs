using Events;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Events
{
    public class GameEventListener : MonoBehaviour
    {
        [Tooltip("Event to register with.")] [SerializeField]
        private GameEvent @event;

        [Tooltip("Response to invoke when Event is raised.")] [SerializeField]
        private UnityEvent response;

        private void OnEnable()
        {
            @event.RegisterListener(this);
        }

        private void OnDisable()
        {
            @event.UnregisterListener(this);
        }

        public void OnEventRaised()
        {
            response.Invoke();
        }
    }
}