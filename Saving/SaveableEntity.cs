using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Saving {

    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour {
        [SerializeField] private string uniqueIdentifier = "";
        private static Dictionary<string, SaveableEntity> globalLookUp = new Dictionary<string, SaveableEntity>();
        private ISaveable[] saveables;

        public string UUID => uniqueIdentifier;

        private void Awake() {
            saveables = GetComponents<ISaveable>();
        }

        public object CaptureState() {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach (ISaveable saveable in saveables) {
                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }

            return state;
        }

        public void RestoreState(object savedState) {
            Dictionary<string, object> state = (Dictionary<string, object>) savedState;
            foreach (ISaveable saveable in saveables) {
                string type = saveable.GetType().ToString();
                if (state.ContainsKey(type)) {
                    saveable.RestoreState(state[type]);
                }
            }
        }

#if UNITY_EDITOR

        private void Update() {
            if (Application.IsPlaying(gameObject) || string.IsNullOrEmpty(gameObject.scene.path)) {
                return;
            }

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");

            if (property.stringValue == "" || !IsUnique(property.stringValue)) {
                property.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }
            globalLookUp[property.stringValue] = this;
        }

#endif

        private bool IsUnique(string candidate) {
            if (!globalLookUp.ContainsKey(candidate) || globalLookUp[candidate] == this) {
                return true;
            }

            if (globalLookUp[candidate] == null || globalLookUp[candidate].UUID != candidate) {
                globalLookUp.Remove(candidate);
                return true;
            }

            return false;
        }
    }
}
