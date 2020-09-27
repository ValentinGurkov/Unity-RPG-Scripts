using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class ObjectPooler : MonoBehaviour
    {
        [System.Serializable]
        public struct Pool
        {
            public GameObject prefab;
            public int size;
        }

        [SerializeField] private List<Pool> pools;
        private IDictionary<GameObject, Queue<GameObject>> _poolDict;

        private void Awake()
        {
            _poolDict = new Dictionary<GameObject, Queue<GameObject>>();

            FillPools();
        }

        private void FillPool(GameObject instance)
        {
            //TODO can't create new pools at the moment
            if (!_poolDict.ContainsKey(instance))
            {
                return;
            }

            Pool pool = pools.Find(p => p.prefab == instance);
            for (var j = 0; j < pool.size; j++)
            {
                GameObject obj = Instantiate(pool.prefab, transform, true);
                obj.SetActive(false);
                _poolDict[instance].Enqueue(obj);
            }
        }

        private void FillPools()
        {
            for (var i = 0; i < pools.Count; i++)
            {
                var objectPool = new Queue<GameObject>();
                for (var j = 0; j < pools[i].size; j++)
                {
                    GameObject obj = Instantiate(pools[i].prefab, transform, true);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                _poolDict.Add(pools[i].prefab, objectPool);
            }
        }

        public void AddToPool(GameObject instance)
        {
            if (!_poolDict.ContainsKey(instance))
            {
                return;
            }

            instance.transform.SetParent(transform);
            instance.SetActive(false);
            _poolDict[instance].Enqueue(instance);
        }

        public GameObject SpawnFromPool(GameObject requested)
        {
            if (!_poolDict.ContainsKey(requested))
            {
                return null;
            }

            if (_poolDict[requested].Count == 0)
            {
                FillPool(requested);
            }

            GameObject instance = _poolDict[requested].Dequeue();
            instance.SetActive(true);

            return instance;
        }
    }
}