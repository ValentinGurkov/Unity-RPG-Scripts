using UnityEngine;

namespace RPG.Core {

    public class PersistentObjectSpawner : MonoBehaviour {
        [SerializeField] private GameObject persistentObjectsPrefab = default;

        private static bool hasSpawned = false;

        private void Awake() {
            if (hasSpawned) {
                return;
            }

            SpawnPersistentObjects();

            hasSpawned = true;
        }

        private void SpawnPersistentObjects() {
            GameObject persistentObjects = Instantiate(persistentObjectsPrefab);
            DontDestroyOnLoad(persistentObjects);
        }
    }
}
