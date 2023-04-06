using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Saving
{
    /// <summary>
    /// GameObjects with this component will allow saving the state of its' components.
    /// </summary>
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] private string uniqueIdentifier = "";

        private static readonly Dictionary<string, SaveableEntity> s_GlobalLookUp =
            new Dictionary<string, SaveableEntity>();

        private ISaveable[] _saveables;

        public string UUID => uniqueIdentifier;

        private void Awake()
        {
            _saveables = GetComponents<ISaveable>();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.IsPlaying(gameObject) || string.IsNullOrEmpty(gameObject.scene.path)) return;

            var serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");

            if (property.stringValue == "" || !IsUnique(property.stringValue))
            {
                property.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            s_GlobalLookUp[property.stringValue] = this;
        }
#endif
        private bool IsUnique(string candidate)
        {
            if (!s_GlobalLookUp.ContainsKey(candidate) || s_GlobalLookUp[candidate] == this)
            {
                return true;
            }

            if (s_GlobalLookUp[candidate] != null && s_GlobalLookUp[candidate].UUID == candidate) return false;
            s_GlobalLookUp.Remove(candidate);
            return true;
        }

        public object CaptureState()
        {
            var state = new Dictionary<string, object>();
            foreach (ISaveable saveable in _saveables)
            {
                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }

            return state;
        }

        public void RestoreState(object savedState)
        {
            var state = (Dictionary<string, object>) savedState;
            foreach (ISaveable saveable in _saveables)
            {
                var type = saveable.GetType().ToString();
                if (state.ContainsKey(type))
                {
                    saveable.RestoreState(state[type]);
                }
            }
        }
    }
}