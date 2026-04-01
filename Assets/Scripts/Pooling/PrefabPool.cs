using System.Collections.Generic;
using UnityEngine;

namespace Pooling
{
    public sealed class PrefabPool
    {
        private readonly GameObject _prefab;
        private readonly Transform _root;
        private readonly Queue<GameObject> _available = new();
        private readonly HashSet<GameObject> _allInstances = new();
        private readonly bool _autoExpand;

        public GameObject Prefab => _prefab;
        public Transform Root => _root;

        public PrefabPool(GameObject prefab, Transform root, bool autoExpand = true)
        {
            _prefab = prefab;
            _root = root;
            _autoExpand = autoExpand;
        }

        public void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject instance = CreateInstance();
                Release(instance);
            }
        }

        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            GameObject instance = null;

            while (_available.Count > 0 && instance == null)
            {
                instance = _available.Dequeue();
            }

            if (instance == null)
            {
                if (!_autoExpand && _allInstances.Count > 0)
                {
                    Debug.LogWarning($"[{nameof(PrefabPool)}] Pool exhausted for prefab {_prefab.name}.");
                    return null;
                }

                instance = CreateInstance();
            }

            Transform tr = instance.transform;
            tr.SetParent(null);
            tr.SetPositionAndRotation(position, rotation);

            if (instance.TryGetComponent(out PoolMember member))
            {
                member.MarkTaken();
            }

            instance.SetActive(true);
            NotifyTaken(instance);

            return instance;
        }

        public void Release(GameObject instance)
        {
            if (instance == null)
            {
                return;
            }

            if (!_allInstances.Contains(instance))
            {
                Debug.LogWarning($"[{nameof(PrefabPool)}] Instance {instance.name} does not belong to pool {_prefab.name}.");
                return;
            }

            if (instance.TryGetComponent(out PoolMember member) && member.IsInPool)
            {
                return;
            }

            NotifyReturned(instance);

            if (instance.TryGetComponent(out member))
            {
                member.MarkReturned();
            }

            instance.transform.SetParent(_root);
            instance.SetActive(false);

            _available.Enqueue(instance);
        }

        private GameObject CreateInstance()
        {
            GameObject instance = Object.Instantiate(_prefab, _root);
            instance.name = $"{_prefab.name}_Pooled";

            PoolMember member = instance.GetComponent<PoolMember>();
            if (member == null)
            {
                member = instance.AddComponent<PoolMember>();
            }

            member.SetOwner(this);
            member.MarkReturned();

            _allInstances.Add(instance);
            return instance;
        }

        private static void NotifyTaken(GameObject instance)
        {
            IPoolable[] poolables = instance.GetComponentsInChildren<IPoolable>(true);
            for (int i = 0; i < poolables.Length; i++)
            {
                poolables[i].OnTakenFromPool();
            }
        }

        private static void NotifyReturned(GameObject instance)
        {
            IPoolable[] poolables = instance.GetComponentsInChildren<IPoolable>(true);
            for (int i = 0; i < poolables.Length; i++)
            {
                poolables[i].OnReturnedToPool();
            }
        }
    }
}