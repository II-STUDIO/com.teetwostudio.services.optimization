using System;
using UnityEngine;

namespace Services.Optimization.PoolingSystem
{
    /// <summary>
    /// Pooling object controllable of prefab for poolable.
    /// </summary>
    public class PoolingObject : MonoBehaviour
    {
        private Transform m_originalPerent;

        [SerializeField] private float lifeTime = 0f;
        [Space]

        [HideInInspector] public float m_runtime_lifeTime = 0f;

        public Transform transformCache { get; private set; }
        public GameObject gameObjectCache { get; private set; }

        /// <summary>
        /// Id of game object.
        /// </summary>
        public int GameObjectId { get; private set; }

        public bool IsActive { get; private set; } = false;

        public void SetOriginalPerent(Transform targetPerent) => m_originalPerent = targetPerent;

        public event Action<int> OnDisabled_Evt, OnEnabled_Evt;

        /// <summary>
        /// This invoke one time when this object is init on 'PoolingSystem'.
        /// </summary>
        public virtual void Initialize() 
        {
            GameObjectId = gameObject.GetInstanceID();

            gameObjectCache = gameObject;
            transformCache = gameObjectCache.transform;
        }

        private void ComputeAndUdpate(float deltaTime)
        {
            if (m_runtime_lifeTime == 0f)
                return;

            m_runtime_lifeTime -= deltaTime;

            if (m_runtime_lifeTime > 0f)
                return;

            DisabledPool();
        }

        protected virtual void Update()
        {
            ComputeAndUdpate(Time.deltaTime);
        }

        /// <summary>
        /// Enabled object *** not use this pool will automatic call this when it take form pool.
        /// </summary>
        public void EnabledPool()
        {
            gameObjectCache.SetActive(true);

            IsActive = true;

            m_runtime_lifeTime = lifeTime;

            OnEnabled_Evt?.Invoke(GameObjectId);

            OnEnabledPool();
        }

        /// <summary>
        /// Disabled object and return to pool.
        /// </summary>
        public void DisabledPool()
        {
            if (!IsActive) 
                return;

            gameObjectCache.SetActive(false);

            IsActive = false;

            m_runtime_lifeTime = 0f;
            transformCache.localPosition = Vector3.zero;

            if (transformCache.parent != m_originalPerent)
                transformCache.SetParent(m_originalPerent);

            OnDisabled_Evt?.Invoke(GameObjectId);

            OnDisabledPool();
        }

        /// <summary>
        /// This fuction is invoke when this object has pooled.
        /// </summary>
        protected virtual void OnEnabledPool()
        {

        }

        /// <summary>
        /// This fuction is invoke when this object has disabled.
        /// </summary>
        protected virtual void OnDisabledPool()
        {

        }

        /// <summary>
        /// This function is invoke before destroy this object.
        /// </summary>
        public virtual void OnDisposeOrDestroy()
        {

        }
    }
}
