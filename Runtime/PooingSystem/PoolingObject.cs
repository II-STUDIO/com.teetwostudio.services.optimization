using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Optimization.PoolingSystem
{
    /// <summary>
    /// Pooling object controllable of prefab for poolable.
    /// </summary>
    public class PoolingObject : MonoBehaviour
    {
        private PoolingProfile _profile;
        private Transform _originalPerent;

        [HideInInspector] public float lifeTimerCountDown = 0f;

        /// <summary>
        /// The fasted for access this transform of this pooling object.
        /// </summary>
        public Transform transformCache { get; private set; }
        public GameObject gameObjectCache { get; private set; }

        public int index { get; set; }

        public string ID { get; private set; }
        public bool IsActive { get; private set; } = false;

        public void SetOriginalPerent(Transform targetPerent) => _originalPerent = targetPerent;

        /// <summary>
        /// Set current life time of this missile.
        /// </summary>
        /// <param name="time"></param>
        public void SetLifeTimeCountDown(float time) => lifeTimerCountDown = time;

        /// <summary>
        /// This invoke one time when this object is init on 'PoolingSystem'.
        /// </summary>
        public virtual void Initialize(PoolingProfile profile, string id) 
        {
            _profile = profile;

            ID = id;

            gameObjectCache = gameObject;
            transformCache = gameObjectCache.transform;

            PoolingManager.AssignGlobalPoolingObject(this);
        }

        /// <summary>
        /// Invoke every frame like 'Update' of unity mono behaviour
        /// </summary>
        /// <param name="deltaTime"></param>
        public void OnUpdate(float deltaTime)
        {

        }

        /// <summary>
        /// Make object enabled.
        /// </summary>
        public void Enabled()
        {
            gameObjectCache.SetActive(true);

            IsActive = true;
            lifeTimerCountDown = _profile.lifeTime;

            if (_profile.lifeTime > 0f)
                PoolingManager.AssignActivatePoolingObject(this);

            OnEnabled();
        }

        /// <summary>
        /// Make object disabled.
        /// </summary>
        public void Disabled()
        {
            if (!gameObjectCache.activeSelf) 
                return;

            gameObjectCache.SetActive(false);

            IsActive = false;
            lifeTimerCountDown = 0f;
            transformCache.localPosition = Vector3.zero;

            if (transformCache.parent != _originalPerent)
                transformCache.SetParent(_originalPerent);

            if (_profile.lifeTime > 0f)
                PoolingManager.UnAssignActivatePoolingObject(this);

            OnDisabled();
        }

        /// <summary>
        /// This fuction is invoke when this object has pooled.
        /// </summary>
        protected virtual void OnEnabled()
        {

        }

        /// <summary>
        /// This fuction is invoke when this object has disabled.
        /// </summary>
        protected virtual void OnDisabled()
        {

        }

        protected virtual void OnDestroy()
        {
            PoolingManager.UnAssigneGlobalPoolingObject(this);
        }
    }
}
