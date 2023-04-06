using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = Util.Logger;

namespace Saving
{
    /// <summary>
    /// Core saving system functionality based on the BinaryFormatter.
    /// </summary>
    public class SavingSystem : MonoBehaviour
    {
        private const string LastSceneBuildIndex = "lastSceneBuildIndex";

        private void SaveFile(string fileName, object state)
        {
            string path = GetPathFromSaveFile(fileName);
            Logger.Log($"Saving to {path}");
            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        private Dictionary<string, object> LoadFile(string fileName)
        {
            string path = GetPathFromSaveFile(fileName);
            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }


            Logger.Log($"Loading from {GetPathFromSaveFile(fileName)}");
            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                return (Dictionary<string, object>) formatter.Deserialize(stream);
            }
        }

        private static void CaptureState(IDictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.UUID] = saveable.CaptureState();
            }

            state[LastSceneBuildIndex] = SceneManager.GetActiveScene().buildIndex;
        }

        private static void RestoreState(IReadOnlyDictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                if (state.ContainsKey(saveable.UUID))
                {
                    saveable.RestoreState(state[saveable.UUID]);
                }
            }
        }

        private static string GetPathFromSaveFile(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName + ".sav");
        }

        public IEnumerator LoadLastScene(string fileName)
        {
            Dictionary<string, object> state = LoadFile(fileName);
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            if (state.ContainsKey(LastSceneBuildIndex))
            {
                buildIndex = (int) state[LastSceneBuildIndex];
            }

            yield return SceneManager.LoadSceneAsync(buildIndex);
            RestoreState(state);
        }

        public void Save(string fileName)
        {
            Dictionary<string, object> state = LoadFile(fileName);
            CaptureState(state);
            SaveFile(fileName, state);
        }

        public void Load(string fileName)
        {
            RestoreState(LoadFile(fileName));
        }

        public void Delete(string fileName)
        {
            Logger.Log($"Deleting {GetPathFromSaveFile(fileName)} if present");
            File.Delete(GetPathFromSaveFile(fileName));
        }
    }
}