using System.Collections.Generic;
using UnityEngine;

namespace Services.Optimization.PoolingSystem
{
    /// <summary>
    /// Pooling system for target of initialized prefab object.
    /// </summary>
    public class Pooler<TPoolingObject> where TPoolingObject : PoolingObject
    {
        private Dictionary<int, TPoolingObject> objectContainer = new Dictionary<int, TPoolingObject>();
        private Queue<int> valiableIndex = new Queue<int>(PoolSetting.Instance.MaxCapacity);

        private TPoolingObject prefab;
        private Transform originalPerent;

        public int ObjectCount { get => objectContainer.Count; }

        public Transform root { get => originalPerent; }


        /// <summary>
        /// This method use for installation pooling system and init all pooling objects of target prefab width parent.
        /// </summary>
        public void Initialize(TPoolingObject prefab, Transform parent)
        {
            this.prefab = prefab;

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

            objectContainer.Add(objectPool.GameObjectId, objectPool);
            valiableIndex.Enqueue(objectPool.GameObjectId);
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

            objectPool.transform.position = poolPosition;
            objectPool.transform.rotation = poolRotation;
            objectPool.transform.localScale = prefab.transform.localScale;
            objectPool.EnabledPool();

            return objectPool;
        }

        public TPoolingObject Pool(Vector3 poolPosition, Quaternion poolRotation, Transform parent)
        {
            var objectPool = GetValidableObject();

            objectPool.transform.position = poolPosition;
            objectPool.transform.rotation = poolRotation;
            objectPool.transform.localScale = prefab.transform.localScale;
            objectPool.transform.SetParent(parent);
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

            objectPool.transform.SetParent(parent);
            objectPool.transform.localPosition = Vector3.zero;
            objectPool.transform.localScale = prefab.transform.localScale;
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

            objectPool.transform.localPosition = Vector3.zero;
            objectPool.transform.localScale = prefab.transform.localScale;
            objectPool.EnabledPool();

            return objectPool;
        }

        /// <summary>
        /// Calculate validabe prefab to pool.
        /// </summary>
        /// <returns></returns>
        private TPoolingObject GetValidableObject()
        {
            if (valiableIndex.Count == 0)
                AddNew();

            return objectContainer[valiableIndex.Dequeue()];
        }

        public void Dispose(bool autoRecycle = true)
        {
            DisabledAll();

            if (autoRecycle)
                PoolManager.RemoveFormRecycleDictionary(prefab);

            var values = objectContainer.Values;
            foreach(var obj in values)
            {
                obj.OnDisposeOrDestroy();

                Object.Destroy(obj.gameObject);
            }

            objectContainer.Clear();

            valiableIndex.Clear();
        }

        public void DisabledAll()
        {
            var values = objectContainer.Values;
            foreach (var obj in values)
            {
                obj.DisabledPool();
            }
        }

        private void OnDisabledPool(int objectId)
        {
            if (!objectContainer.ContainsKey(objectId))
            {
                Debug.LogError($"Object id <{objectId}> not exited in pooler");
                return;
            }

            valiableIndex.Enqueue(objectId);
        }
    }
}
