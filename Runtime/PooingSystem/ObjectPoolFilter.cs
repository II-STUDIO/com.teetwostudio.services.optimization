using System.Collections.Generic;
using UnityEngine;

namespace Services.Optimization.PoolingSystem
{
    public class ObjectPoolFilter<IPoolingObject> where IPoolingObject : PoolingObject
    {
        private Dictionary<string, ObjectPooler<IPoolingObject>> containers = new Dictionary<string, ObjectPooler<IPoolingObject>>();
        private ObjectPooler<IPoolingObject> objectPooler;

        private void Assignment(PoolProfile poolProfile)
        {
            if (containers.TryGetValue(poolProfile.ID, out objectPooler))
            {
                return;
            }

            objectPooler = PoolManager.CreatePooler<IPoolingObject>(poolProfile);
            containers.Add(poolProfile.ID, objectPooler);
        }

        public IPoolingObject Pool(PoolProfile poolProfile)
        {
            Assignment(poolProfile);

            return objectPooler.Pool();
        }

        public IPoolingObject Pool(PoolProfile poolProfile, Transform parent)
        {
            Assignment(poolProfile);

            return objectPooler.Pool(parent);
        }

        public IPoolingObject Pool(PoolProfile poolProfile, Vector3 position, Quaternion rotate)
        {
            Assignment(poolProfile);

            return objectPooler.Pool(position, rotate);
        }

        public IPoolingObject Pool(PoolProfile poolProfile, Vector3 position, Quaternion rotate, Transform parent)
        {
            Assignment(poolProfile);

            return objectPooler.Pool(position, rotate, parent);
        }
    }
}