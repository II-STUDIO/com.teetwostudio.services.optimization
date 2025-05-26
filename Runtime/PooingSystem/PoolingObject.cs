using System;
using UnityEngine;

namespace Services.Optimization.PoolingSystem
{
    /// <summary>
    /// Pooling object controllable prefab instance for pooling.
    /// </summary>
    public class PoolingObject : LoopUpdateMonoBehaviour
    {
        private Transform _originalParent;

        [SerializeField]
        private float lifeTime = 0f;

        [HideInInspector]
        public float m_runtime_lifeTime = 0f;

        public Transform transformCache { get; private set; }
        public GameObject gameObjectCache { get; private set; }

        /// <summary>Unique instance id for this game object.</summary>
        public int GameObjectId { get; private set; }

        private bool _isActive = false;
        public bool IsActive
        {
            get => _isActive;
            private set => _isActive = value;
        }

        public void SetOriginalPerent(Transform targetParent) => _originalParent = targetParent;

        /// <summary>
        /// Called once when object is initialized by pooling system.
        /// </summary>
        public virtual void Initialize()
        {
            GameObjectId = gameObject.GetInstanceID();
            gameObjectCache = gameObject;
            transformCache = gameObjectCache.transform;
        }

        private void ComputeAndUpdate(float deltaTime)
        {
            if (m_runtime_lifeTime <= 0f)
                return;

            m_runtime_lifeTime -= deltaTime;

            if (m_runtime_lifeTime > 0f)
                return;

            DisabledPool();
        }

        /// <summary>
        /// Enables this pooled object and resets its life timer.
        /// </summary>
        public void EnabledPool()
        {
            if (_isActive)
                return;

            gameObjectCache.SetActive(true);
            IsActive = true;

            m_runtime_lifeTime = lifeTime;

            OnEnabled_Evt?.Invoke(GameObjectId);
            OnEnabledPool();
        }

        /// <summary>
        /// Disables this pooled object and returns it to pool.
        /// </summary>
        public void DisabledPool()
        {
            if (!IsActive)
                return;

            if (gameObjectCache)
                gameObjectCache.SetActive(false);

            IsActive = false;

            m_runtime_lifeTime = 0f;

            // Reset position only if not zero to reduce transform overhead
            if (transformCache)
            {
                if (transformCache.localPosition != Vector3.zero)
                    transformCache.localPosition = Vector3.zero;

                if (transformCache.parent != _originalParent)
                    transformCache.SetParent(_originalParent, worldPositionStays: false);
            }

            OnDisabled_Evt?.Invoke(GameObjectId);
            OnDisabledPool();
        }

        /// <summary>
        /// Called when this object is enabled from the pool.
        /// Override to add custom logic.
        /// </summary>
        protected virtual void OnEnabledPool() { }

        /// <summary>
        /// Called when this object is disabled and returned to the pool.
        /// Override to add custom logic.
        /// </summary>
        protected virtual void OnDisabledPool() { }

        /// <summary>
        /// Called before this object is destroyed or disposed.
        /// Override to cleanup.
        /// </summary>
        public virtual void OnDisposeOrDestroy() { }

        /// <summary>
        /// Update loop callback.
        /// </summary>
        public override void LoopUpdateEvent(float deltaTime)
        {
            ComputeAndUpdate(deltaTime);
        }

        // Events for external listeners
        public event Action<int> OnDisabled_Evt;
        public event Action<int> OnEnabled_Evt;
    }
}
