using System.Collections.Generic;
using UnityEngine;

namespace Services.Optimization.PoolingSystem
{
    public class PoolerGroup<TPoolingObject> where TPoolingObject : PoolingObject
    {
        private Dictionary<TPoolingObject, Pooler<TPoolingObject>> containers = new();

        private Pooler<TPoolingObject> GetPooler(TPoolingObject prefab)
        {
            if (containers.TryGetValue(prefab, out var objectPooler))
            {
                return objectPooler;
            }

            objectPooler = PoolManager.CreatePooler(prefab);
            containers.Add(prefab, objectPooler);

            return objectPooler;
        }

        public TPoolingObject Pool(TPoolingObject prefab)
        {
            return GetPooler(prefab).Pool();
        }

        public TPoolingObject Pool(TPoolingObject prefab, Transform parent)
        {
            return GetPooler(prefab).Pool(parent);
        }

        public TPoolingObject Pool(TPoolingObject prefab, Vector3 position, Quaternion rotate)
        {
            return GetPooler(prefab).Pool(position, rotate);
        }

        public TPoolingObject Pool(TPoolingObject prefab, Vector3 position, Quaternion rotate, Transform parent)
        {
            return GetPooler(prefab).Pool(position, rotate, parent);
        }

        public void Dispose(bool autoRecrycle = true)
        {
            var values = containers.Values;
            foreach(var pooler in values)
            {
                pooler.Dispose(autoRecrycle);
            }
        }
    }
}