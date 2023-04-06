using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pooling
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
        private IDictionary<string, Queue<GameObject>> _poolDict;
        private IDictionary<GameObject, List<Coroutine>> _test;

        private void Awake()
        {
            _poolDict = new Dictionary<string, Queue<GameObject>>();
            _test = new Dictionary<GameObject, List<Coroutine>>();

            FillPools();
        }

        private void FillPool(GameObject instance)
        {
            //TODO can't create dynamic pools at the moment
            if (!_poolDict.ContainsKey(instance.name)) return;

            Pool pool = pools.Find(p => p.prefab == instance);
            for (var j = 0; j < pool.size; j++)
            {
                GameObject obj = Instantiate(pool.prefab, transform, true);
                obj.SetActive(false);
                _poolDict[instance.name].Enqueue(obj);
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
                    obj.name = pools[i].prefab.name;
                    objectPool.Enqueue(obj);
                }

                _poolDict.Add(pools[i].prefab.name, objectPool);
            }
        }

        private IEnumerator ReturnToPoolWithDelay(GameObject instance, WaitForSeconds delay)
        {
            yield return delay;
            AddToPool(instance);
        }

        private IEnumerator ReturnParticleToPool(ParticleSystem particle)
        {
            while (particle.IsAlive())
            {
                yield return null;
            }

            AddToPool(particle.gameObject);
        }

        public void AddToPool(GameObject instance)
        {
            if (!_poolDict.ContainsKey(instance.name)) return;
            instance.transform.SetParent(transform);
            instance.SetActive(false);
            _poolDict[instance.name].Enqueue(instance);
        }

        public void AddToPoolWithDelay(GameObject instance, WaitForSeconds delay)
        {
            if (!_poolDict.ContainsKey(instance.name)) return;
            if (!_test.ContainsKey(instance))
            {
                _test[instance] = new List<Coroutine>();
            }

            _test[instance].Add(StartCoroutine(ReturnToPoolWithDelay(instance, delay)));
        }

        public void AddToPoolWithDelay(ParticleSystem instance)
        {
            if (!_poolDict.ContainsKey(instance.name)) return;
            StartCoroutine(ReturnParticleToPool(instance));
            if (!_test.ContainsKey(instance.gameObject))
            {
                _test[instance.gameObject] = new List<Coroutine>();
            }

            _test[instance.gameObject].Add(StartCoroutine(ReturnParticleToPool(instance)));
        }


        public GameObject SpawnFromPool(GameObject requested, Vector3 position, Quaternion rotation)
        {
            if (!_poolDict.ContainsKey(requested.name)) return null;

            if (_poolDict[requested.name].Count == 0) FillPool(requested);

            GameObject instance = _poolDict[requested.name].Dequeue();
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            instance.SetActive(true);
            return instance;
        }

        public GameObject SpawnFromPool(GameObject requested, Vector3 position)
        {
            if (!_poolDict.ContainsKey(requested.name)) return null;

            if (_poolDict[requested.name].Count == 0) FillPool(requested);

            GameObject instance = _poolDict[requested.name].Dequeue();
            instance.transform.position = position;
            instance.transform.rotation = Quaternion.identity;
            instance.SetActive(true);
            return instance;
        }
    }
}