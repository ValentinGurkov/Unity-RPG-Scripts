using System.Collections.Generic;
using UnityEngine;

namespace RPG.Util {
    public class ObjectPooler : MonoBehaviour {

        [System.Serializable]
        public struct Pool {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        [SerializeField] private List<Pool> pools = default;
        [SerializeField] private Dictionary<string, Queue<GameObject>> poolDict = default;

        public static ObjectPooler Instace;

        private void Awake() {
            Instace = this;
            poolDict = new Dictionary<string, Queue<GameObject>>();

            FillPools();
        }

        private void FillPool(string tag) {
            if (!poolDict.ContainsKey(tag)) {
                return;
            }
            Pool pool = pools.Find(p => p.tag == tag);
            for (int j = 0; j < pool.size; j++) {
                GameObject obj = Instantiate(pool.prefab);
                obj.transform.SetParent(transform);
                obj.SetActive(false);
                poolDict[tag].Enqueue(obj);
            }
        }

        private void FillPools() {
            for (int i = 0; i < pools.Count; i++) {
                Queue<GameObject> objectPool = new Queue<GameObject>();
                for (int j = 0; j < pools[i].size; j++) {
                    GameObject obj = Instantiate(pools[i].prefab);
                    obj.transform.SetParent(transform);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }
                poolDict.Add(pools[i].tag, objectPool);
            }
        }

        public void AddToPool(string tag, GameObject instance) {
            if (!poolDict.ContainsKey(tag)) {
                return;
            }
            instance.SetActive(false);
            poolDict[tag].Enqueue(instance);
        }

        public GameObject SpawnFromPool(string tag) {
            if (!poolDict.ContainsKey(tag)) {
                return null;
            }

            if (poolDict[tag].Count == 0) {
                FillPool(tag);
            }

            GameObject instance = poolDict[tag].Dequeue();
            instance.SetActive(true);

            return instance;
        }
    }
}
