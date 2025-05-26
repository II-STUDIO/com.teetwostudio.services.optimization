using System.Collections.Generic;
using UnityEngine;

namespace Services.Optimization.PoolingSystem
{
    /// <summary>
    /// Pooling system for target of initialized prefab object.
    /// </summary>
    public class Pooler<TPoolingObject> : IPooler where TPoolingObject : PoolingObject
    {
        // Use readonly fields for container sizes if possible
        private readonly Dictionary<int, TPoolingObject> container;
        private readonly Queue<int> availableIds;

        private TPoolingObject prefab;
        private Transform originalParent;
        private Vector3 prefabScale;

        public int ObjectCount => container.Count;
        public Transform root => originalParent;

        public Pooler()
        {
            int maxCapacity = PoolSetting.Instance?.MaxCapacity ?? 100; // fallback default

            container = new Dictionary<int, TPoolingObject>(maxCapacity);
            availableIds = new Queue<int>(maxCapacity);
        }

        public void Initialize(TPoolingObject prefab, Transform parent)
        {
            if (prefab == null)
            {
                Debug.LogError("Pooler Initialize failed: prefab is null");
                return;
            }

            this.prefab = prefab;
            prefabScale = prefab.transform.localScale;
            originalParent = parent;
        }

        public void AddCount(int count)
        {
            for (int i = 0; i < count; i++)
                AddNew();
        }

        private void AddNew()
        {
            if (prefab == null)
            {
                Debug.LogError("Pooler AddNew failed: prefab is null");
                return;
            }

            var obj = Object.Instantiate(prefab, originalParent);
            if (obj == null)
            {
                Debug.LogError($"Pooler AddNew failed: instantiate prefab {prefab.name} returned null");
                return;
            }

            obj.Initialize();
            obj.OnDisabled_Evt += OnDisabledPool;
            obj.gameObject.SetActive(false);
            obj.SetOriginalPerent(originalParent);

            container.Add(obj.GameObjectId, obj);
            availableIds.Enqueue(obj.GameObjectId);
        }

        // Pool methods: minimal duplication by common method for setup
        public TPoolingObject Pool(Vector3 position, Quaternion rotation)
            => PoolInternal(position, rotation, originalParent);

        public TPoolingObject Pool(Vector3 position, Quaternion rotation, Transform parent)
            => PoolInternal(position, rotation, parent);

        public TPoolingObject Pool(Transform parent)
            => PoolInternal(Vector3.zero, Quaternion.identity, parent);

        public TPoolingObject Pool()
            => PoolInternal(Vector3.zero, Quaternion.identity, originalParent);

        private TPoolingObject PoolInternal(Vector3 position, Quaternion rotation, Transform parent)
        {
            var obj = GetAvailableObject();

            var t = obj.transformCache;
            t.SetPositionAndRotation(position, rotation);
            t.localScale = prefabScale;
            t.SetParent(parent, false);
            obj.EnabledPool();

            return obj;
        }

        private TPoolingObject GetAvailableObject()
        {
            if (availableIds.Count == 0)
                AddNew();

            int id = availableIds.Dequeue();
            if (!container.TryGetValue(id, out var obj))
            {
                Debug.LogError($"Pooler GetAvailableObject failed: object id {id} missing from container");
                return null;
            }

            return obj;
        }

        public void Dispose()
        {
            DisabledAll();

            // Avoid enumerator allocations by using foreach with cached Values collection
            var values = container.Values;
            foreach (var obj in values)
            {
                if (!obj)
                    continue;

                obj.OnDisposeOrDestroy();
                Object.Destroy(obj.gameObject);
            }

            container.Clear();
            availableIds.Clear();
        }

        public void DisabledAll()
        {
            var values = container.Values;
            foreach (var obj in values)
            {
                if (!obj)
                    continue;

                obj.DisabledPool();
            }
        }

        private void OnDisabledPool(int objectId)
        {
            // Use TryGetValue to avoid double dictionary lookup
            if (!container.ContainsKey(objectId))
            {
                Debug.LogError($"Pooler OnDisabledPool: Object id {objectId} not found in container");
                return;
            }

            availableIds.Enqueue(objectId);
        }
    }

    public interface IPooler
    {
        void Dispose();
        void DisabledAll();
    }
}
