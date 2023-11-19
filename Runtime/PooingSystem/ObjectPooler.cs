using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Optimization.PoolingSystem
{
    /// <summary>
    /// Pooling system for target of initialized prefab object.
    /// </summary>
    public class ObjectPooler<TPoolingObject> where TPoolingObject : PoolingObject
    {
        private List<TPoolingObject> objectList = new List<TPoolingObject>(PoolManager.SystemBaseHnadler.MaxCapacity);

        private Dictionary<int, TPoolingObject> objectContainer = new Dictionary<int, TPoolingObject>();
        private Queue<int> valiableIndex = new Queue<int>(PoolManager.SystemBaseHnadler.MaxCapacity);

        private PoolProfile profile;
        private Transform originalPerent;

        private string profileID;

        public int ObjectCount { get => objectList.Count; }

        public Transform root { get => originalPerent; }


        /// <summary>
        /// This method use for installation pooling system and init all pooling objects of target prefab width parent.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="amount"></param>
        public void Initialize(PoolProfile profile, int amount, Transform parent)
        {
            this.profile = profile;

            originalPerent = parent;
            profileID = profile.ID;

            for (int i = 0; i < amount; i++)
            {
                AddNew();
            }
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
            if(profile.Prefab == null)
            {
                Debug.LogErrorFormat("Prefab of Pooling profile can't be null or emty");
                return;
            }

            var objectPool = (TPoolingObject)UnityEngine.Object.Instantiate(profile.Prefab, originalPerent);

            objectPool.Initialize(profile, profileID);
            objectPool.OnDisabled_Evt += OnObjectDisabled;

            if (!objectPool.gameObject) 
            { 
                Debug.LogError("'ObjectPooing' can't create prefab <" + profile.name + ">");
                return; 
            }

            objectPool.gameObject.SetActive(false);
            objectPool.SetOriginalPerent(originalPerent);

            objectPool.LocalIndex = objectList.Count;

            objectList.Add(objectPool);
            objectContainer.Add(objectPool.GameObjectId, objectPool);
            valiableIndex.Enqueue(objectPool.LocalIndex);
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
            objectPool.transform.localScale = profile.Prefab.transform.localScale;
            objectPool.Enabled();

            return objectPool;
        }

        public TPoolingObject Pool(Vector3 poolPosition, Quaternion poolRotation, Transform parent)
        {
            var objectPool = GetValidableObject();

            objectPool.transform.position = poolPosition;
            objectPool.transform.rotation = poolRotation;
            objectPool.transform.localScale = profile.Prefab.transform.localScale;
            objectPool.transform.SetParent(parent);
            objectPool.Enabled();

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
            objectPool.transform.localScale = profile.Prefab.transform.localScale;
            objectPool.Enabled();

            return objectPool;
        }

        /// <summary>
        /// Call for pool object prefab and enabled.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns>Lasted object prefab index</returns>
        public TPoolingObject Pool()
        {
            var objectPool = GetValidableObject();

            objectPool.transform.localPosition = Vector3.zero;
            objectPool.transform.localScale = profile.Prefab.transform.localScale;
            objectPool.Enabled();

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

            return objectList[valiableIndex.Dequeue()];
        }

        public void Dispose(bool autoRecycle = true)
        {
            DisabledAll();

            if (autoRecycle)
                PoolManager.RemoveFormRecycleDictionary(profileID);

            UnityEngine.Object.Destroy(originalPerent.gameObject);

            objectList.Clear();
            objectList = null;

            objectContainer.Clear();
            objectContainer = null;

            valiableIndex.Clear();
            valiableIndex = null;

            profile = null;
            originalPerent = null;

            profileID = null;
        }

        public void DisabledAll()
        {
            int count = objectList.Count;
            for (int i = 0; i < count; i++)
            {
                objectList[i].Disabled();
            }
        }

        private void OnObjectDisabled(int objectId)
        {
            if (!objectContainer.TryGetValue(objectId, out TPoolingObject poolingObject))
            {
                Debug.LogError($"Object id <{objectId}> not exited in pooler <{profileID}>");
                return;
            }

            valiableIndex.Enqueue(poolingObject.LocalIndex);
        }
    }
}
