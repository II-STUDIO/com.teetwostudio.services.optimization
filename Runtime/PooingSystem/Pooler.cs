using System.Collections.Generic;
using UnityEngine;

namespace Services.Optimization.PoolingSystem
{
    /// <summary>
    /// Pooling system for target of initialized prefab object.
    /// </summary>
    public class Pooler<TPoolingObject> : IPooler where TPoolingObject : PoolingObject
    {
        private Dictionary<int, TPoolingObject> container = new(PoolSetting.Instance.MaxCapacity);
        private Queue<int> valiableId = new(PoolSetting.Instance.MaxCapacity);

        private TPoolingObject prefab;
        private Transform originalPerent;

        public int ObjectCount { get => container.Count; }

        public Transform root { get => originalPerent; }

        private Vector3 prefabScale;


        /// <summary>
        /// This method use for installation pooling system and init all pooling objects of target prefab width parent.
        /// </summary>
        public void Initialize(TPoolingObject prefab, Transform parent)
        {
            this.prefab = prefab;
            prefabScale = prefab.transform.localScale;

            originalPerent = parent;
        }

        /// <summary>
        /// This metod use for add pooling count of original prefab.
        /// </summary>
        /// <param name="count"></param>
        public void AddCount(int count)
        {
            for (int i = 0; i < count; i++)
                AddNew();
        }

        private void AddNew()
        {
            if(prefab == null)
            {
                Debug.LogErrorFormat("Prefab of Pooling profile can't be null or emty");
                return;
            }

            var objectPool = UnityEngine.Object.Instantiate(prefab, originalPerent);
            if (!objectPool)
            {
                Debug.LogError("'ObjectPooing' can't create prefab <" + prefab.name + ">");
                return;
            }

            objectPool.Initialize();
            objectPool.OnDisabled_Evt += OnDisabledPool;
            objectPool.gameObject.SetActive(false);
            objectPool.SetOriginalPerent(originalPerent);

            container.Add(objectPool.GameObjectId, objectPool);
            valiableId.Enqueue(objectPool.GameObjectId);
        }

        /// <summary>
        /// Call for pool object prefab out width position and rotaion and enabled.
        /// </summary>
        /// <param name="poolPosition"></param>
        /// <param name="poolRotation"></param>
        /// <returns>Lasted object prefab index</returns>
        public TPoolingObject Pool(Vector3 poolPosition, Quaternion poolRotation)
        {
            var objectPool = GetValidableObject();

            objectPool.transformCache.SetPositionAndRotation(poolPosition, poolRotation);
            objectPool.transformCache.localScale = prefabScale;
            objectPool.EnabledPool();

            return objectPool;
        }

        public TPoolingObject Pool(Vector3 poolPosition, Quaternion poolRotation, Transform parent)
        {
            var objectPool = GetValidableObject();

            objectPool.transformCache.SetPositionAndRotation(poolPosition, poolRotation);
            objectPool.transformCache.localScale = prefabScale;
            objectPool.transformCache.SetParent(parent);
            objectPool.EnabledPool();

            return objectPool;
        }

        /// <summary>
        /// Call for pool object prefab out width set parent and enabled.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns>Lasted object prefab index</returns>
        public TPoolingObject Pool(Transform parent)
        {
            var objectPool = GetValidableObject();

            objectPool.transformCache.SetParent(parent);
            objectPool.transformCache.localPosition = Vector3.zero;
            objectPool.transformCache.localScale = prefabScale;
            objectPool.EnabledPool();

            return objectPool;
        }

        /// <summary>
        /// Call for pool object prefab and enabled.
        /// </summary>
        /// <returns>Lasted object prefab index</returns>
        public TPoolingObject Pool()
        {
            var objectPool = GetValidableObject();

            objectPool.transformCache.localPosition = Vector3.zero;
            objectPool.transformCache.localScale = prefabScale;
            objectPool.EnabledPool();

            return objectPool;
        }

        /// <summary>
        /// Calculate validabe prefab to pool.
        /// </summary>
        /// <returns></returns>
        private TPoolingObject GetValidableObject()
        {
            if (valiableId.Count == 0)
                AddNew();

            return container[valiableId.Dequeue()];
        }

        /// <summary>
        /// Use this to disabled all pooling that create by this group and that destroy them all.
        /// </summary>
        public void Dispose()
        {
            DisabledAll();

            var values = container.Values;
            foreach(var obj in values)
            {
                obj.OnDisposeOrDestroy();

                Object.Destroy(obj.gameObject);
            }

            container.Clear();

            valiableId.Clear();
        }

        /// <summary>
        /// Disabed all pooling.
        /// </summary>
        public void DisabledAll()
        {
            var values = container.Values;
            foreach (var obj in values)
            {
                obj.DisabledPool();
            }
        }

        private void OnDisabledPool(int objectId)
        {
            if (!container.ContainsKey(objectId))
            {
                Debug.LogError($"Object id <{objectId}> not exited in pooler");
                return;
            }

            valiableId.Enqueue(objectId);
        }
    }

    public interface IPooler { }
}
