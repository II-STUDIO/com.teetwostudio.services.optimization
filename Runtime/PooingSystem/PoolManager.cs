using System.Collections.Generic;
using UnityEngine;

namespace Services.Optimization.PoolingSystem
{
    /// <summary>
    /// This class supports calling one way of global pooler with easy methods.
    /// </summary>
    public static class PoolManager
    {
        // Cache PoolSetting.Instance for reuse (optional)
        private static readonly PoolSetting poolSetting = PoolSetting.Instance;

        // Dictionary stores prefab as key, pooler as value
        private static readonly Dictionary<PoolingObject, IPooler> poolersDict = new Dictionary<PoolingObject, IPooler>(300);

        /// <summary>
        /// Gets pooler of PoolingObject profile matching the prefab.
        /// </summary>
        public static Pooler<TPoolingObject> GetPooler<TPoolingObject>(TPoolingObject prefab) where TPoolingObject : PoolingObject
        {
            if (prefab == null)
            {
                Debug.LogError("PoolManager GetPooler failed: prefab is null");
                return null;
            }

            if (poolersDict.TryGetValue(prefab, out var pooler))
                return pooler as Pooler<TPoolingObject>;

            Debug.LogError($"PoolManager: Pooler for prefab '{prefab.name}' not found.");
            return null;
        }

        /// <summary>
        /// Creates or returns existing pooler of PoolingObject profile.
        /// </summary>
        public static Pooler<TPoolingObject> CreatePooler<TPoolingObject>(TPoolingObject prefab) where TPoolingObject : PoolingObject
        {
            if (prefab == null)
            {
                Debug.LogError("PoolManager CreatePooler failed: prefab is null");
                return null;
            }

            if (poolersDict.TryGetValue(prefab, out var existingPooler))
                return existingPooler as Pooler<TPoolingObject>;

            var newPooler = Create(prefab);
            poolersDict.Add(prefab, newPooler);
            return newPooler;
        }

        /// <summary>
        /// Internal create pooler helper.
        /// </summary>
        private static Pooler<TPoolingObject> Create<TPoolingObject>(TPoolingObject prefab) where TPoolingObject : PoolingObject
        {
            string name = $"Pooling System : {prefab.name}";

            var systemObj = new GameObject(name);

            var container = poolSetting?.Container;
            if (container != null)
                systemObj.transform.SetParent(container, false);

            var pooler = new Pooler<TPoolingObject>();
            pooler.Initialize(prefab, systemObj.transform);

            return pooler;
        }

        /// <summary>
        /// Dispose all poolers and clear dictionary.
        /// </summary>
        public static void DisposeAll()
        {
            if (poolersDict.Count == 0) return;

            foreach (var pooler in poolersDict.Values)
            {
                if (pooler == null)
                    continue;
                pooler.Dispose();
            }

            poolersDict.Clear();
        }

        /// <summary>
        /// Disable all pooled objects from all poolers.
        /// </summary>
        public static void DisableAll()
        {
            if (poolersDict.Count == 0) return;

            foreach (var pooler in poolersDict.Values)
            {
                if (pooler == null)
                    continue;
                pooler.DisabledAll();
            }
        }
    }
}
