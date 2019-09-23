using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving {

    public class SavingSystem : MonoBehaviour {

        private const string LAST_SCENE_BUILD_INDEX = "lastSceneBuildIndex";

        public IEnumerator LoadLastScene(string fileName) {
            Dictionary<string, object> state = LoadFile(fileName);
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            if (state.ContainsKey(LAST_SCENE_BUILD_INDEX)) {
                buildIndex = (int) state[LAST_SCENE_BUILD_INDEX];
            }
            yield return SceneManager.LoadSceneAsync(buildIndex);
            RestoreState(state);
        }

        public void Save(string fileName) {
            Dictionary<string, object> state = LoadFile(fileName);
            CaptureState(state);
            SaveFile(fileName, state);
        }

        public void Load(string fileName) {
            RestoreState(LoadFile(fileName));
        }

        public void Delete(string fileName) {
            File.Delete(GetPathFromSaveFile(fileName));
        }

        private void SaveFile(string fileName, object state) {
            string path = GetPathFromSaveFile(fileName);
            print("Saving to " + path);
            using(FileStream stream = File.Open(path, FileMode.Create)) {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        private Dictionary<string, object> LoadFile(string fileName) {
            string path = GetPathFromSaveFile(fileName);
            print("Loading from " + GetPathFromSaveFile(fileName));
            if (!File.Exists(path)) {
                return new Dictionary<string, object>();
            }
            using(FileStream stream = File.Open(path, FileMode.Open)) {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>) formatter.Deserialize(stream);
            }
        }

        private void CaptureState(Dictionary<string, object> state) {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>()) {
                state[saveable.UUID] = saveable.CaptureState();
            }
            state[LAST_SCENE_BUILD_INDEX] = SceneManager.GetActiveScene().buildIndex;
        }

        private void RestoreState(Dictionary<string, object> state) {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>()) {
                if (state.ContainsKey(saveable.UUID)) {
                    saveable.RestoreState(state[saveable.UUID]);
                }
            }
        }

        private string GetPathFromSaveFile(string fileName) {
            return Path.Combine(Application.persistentDataPath, fileName + ".sav");
        }
    }
}
