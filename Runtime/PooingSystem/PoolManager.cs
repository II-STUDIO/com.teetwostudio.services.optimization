using System.Collections.Generic;
using UnityEngine;

namespace Services.Optimization.PoolingSystem
{
    /// <summary>
    /// This class is supported to call one way of global pooler width eazy method.
    /// </summary>
    public static class PoolManager
    {
        /// <summary>
        /// Pooler container.
        /// </summary>
        private static Dictionary<PoolingObject, IPooler> poolersDict = new Dictionary<PoolingObject, IPooler>(300);

        /// <summary>
        /// Call for find pooler of PoolingObject profile with matches id.
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns>Pooler of proifle object prefab of matches id prefab</returns>
        public static Pooler<TPoolingObject> GetPooler<TPoolingObject>(TPoolingObject prefab) where TPoolingObject : PoolingObject
        {
            if (poolersDict.ContainsKey(prefab))
                return poolersDict[prefab] as Pooler<TPoolingObject>;

            Debug.LogErrorFormat($"Object Pooler of the profile id <{prefab.name}> not find or created");

            return null;
        }

        ///
        /// Summary:
        ///     Use to create pooler of pool profile or get exited.
        /// <summary>
        /// Call for create pooler of PoolingObject profile.
        /// </summary>
        /// <returns>PoolingSystem Componet of created target prefab</returns>
        public static Pooler<TPoolingObject> CreatePooler<TPoolingObject>(TPoolingObject prefab) where TPoolingObject : PoolingObject
        {
            Pooler<TPoolingObject> pooler = GetExisted(prefab);

            if (pooler != null) 
                return pooler;

            pooler = Create(prefab);

            poolersDict.Add(prefab, pooler);

            return pooler;
        }

        /// <summary>
        /// Try create pooler of pooling object prefab if is existed this method will add amound staked.
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns>PoolingSystem Componet of created target prefab</returns>
        private static Pooler<TPoolingObject> GetExisted<TPoolingObject>(TPoolingObject prefab) where TPoolingObject : PoolingObject
        {
            if (poolersDict.TryGetValue(prefab, out var poolerObj))
            {
                Pooler<TPoolingObject> pooler = poolerObj as Pooler<TPoolingObject>;

                return pooler;
            }

            return null;
        }

        /// <summary>
        /// Use to create pooler.
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        private static Pooler<TPoolingObject> Create<TPoolingObject>(TPoolingObject prefab) where TPoolingObject : PoolingObject
        {
            string name = $"Pooling System name : <{prefab.name}>";

            var systemObj = new GameObject(name);

            var systemContainer = PoolSetting.Instance.Container;
            if (systemContainer)
                systemObj.transform.SetParent(systemContainer);

            Pooler<TPoolingObject> pooler = new Pooler<TPoolingObject>();

            pooler.Initialize(prefab, systemObj.transform);

            return pooler;
        }

        /// <summary>
        /// Dispose all pooeler.
        /// </summary>
        public static void DisposeAll()
        {
            var values = poolersDict.Values;

            foreach (IPooler system in values)
            {
                system.Dispose();
            }
        }

        /// <summary>
        /// Disabled all pooling object.
        /// </summary>
        public static void DisabledAll()
        {
            var values = poolersDict.Values;
            foreach (IPooler system in values)
            {
                system.DisabledAll();
            }
        }
    }
}
