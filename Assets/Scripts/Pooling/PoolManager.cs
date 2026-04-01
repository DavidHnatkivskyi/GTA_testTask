using System.Collections.Generic;
using UnityEngine;

namespace Pooling
{
    public sealed class PoolManager : MonoBehaviour
    {
        [System.Serializable]
        private sealed class PoolSetup
        {
            public GameObject prefab;
            [Min(0)] public int prewarmCount = 0;
        }

        [Header("Initial Pools")]
        [SerializeField] private PoolSetup[] initialPools;

        [Header("Settings")]
        [SerializeField] private bool autoCreatePools = true;
        [SerializeField] private bool autoExpandPools = true;

        private readonly Dictionary<GameObject, PrefabPool> _poolsByPrefab = new();

        private void Awake()
        {
            for (int i = 0; i < initialPools.Length; i++)
            {
                PoolSetup setup = initialPools[i];
                if (setup == null || setup.prefab == null)
                {
                    continue;
                }

                PrefabPool pool = GetOrCreatePool(setup.prefab);
                pool.Prewarm(setup.prewarmCount);
            }
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null)
            {
                Debug.LogError($"[{nameof(PoolManager)}] Tried to spawn null prefab.", this);
                return null;
            }

            PrefabPool pool = GetOrCreatePool(prefab);
            if (pool == null)
            {
                return null;
            }

            return pool.Get(position, rotation);
        }

        public T Spawn<T>(GameObject prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            GameObject instance = Spawn(prefab, position, rotation);
            if (instance == null)
            {
                return null;
            }

            if (!instance.TryGetComponent(out T component))
            {
                Debug.LogError($"[{nameof(PoolManager)}] Spawned object {instance.name} has no component {typeof(T).Name}.", instance);
                return null;
            }

            return component;
        }

        public void Despawn(GameObject instance)
        {
            if (instance == null)
            {
                return;
            }

            if (!instance.TryGetComponent(out PoolMember member) || member.OwnerPool == null)
            {
                Debug.LogWarning($"[{nameof(PoolManager)}] Object {instance.name} is not pooled. Destroying.");
                Destroy(instance);
                return;
            }

            member.OwnerPool.Release(instance);
        }

        private PrefabPool GetOrCreatePool(GameObject prefab)
        {
            if (_poolsByPrefab.TryGetValue(prefab, out PrefabPool existingPool))
            {
                return existingPool;
            }

            if (!autoCreatePools)
            {
                Debug.LogWarning($"[{nameof(PoolManager)}] No pool found for prefab {prefab.name}, and auto-create is disabled.", this);
                return null;
            }

            GameObject rootObject = new GameObject($"{prefab.name}_Pool");
            rootObject.transform.SetParent(transform);

            PrefabPool newPool = new PrefabPool(prefab, rootObject.transform, autoExpandPools);
            _poolsByPrefab.Add(prefab, newPool);

            return newPool;
        }
    }
}