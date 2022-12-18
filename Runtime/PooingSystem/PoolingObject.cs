using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Optimization.PoolingSystem
{
    /// <summary>
    /// Pooling object controllable of prefab for poolable.
    /// </summary>
    public class PoolingObject
    {
        private PoolingProfile _profile;
        private Transform _originalPerent;

        /// <summary>
        /// The fasted for access this transform of this pooling object.
        /// </summary>
        public Transform transform { get; private set; }
        public GameObject gameObject { get; private set; }

        public string ID { get; private set; }
        public bool IsActive { get; private set; } = false;

        public void SetOriginalPerent(Transform targetPerent) => _originalPerent = targetPerent;

        /// <summary>
        /// Set current life time of this missile.
        /// </summary>
        /// <param name="time"></param>
        public void SetLifeTimeCountDown(float time) => lifeTimerCountDown = time;

        public PoolingObject(GameObject referenceObject, PoolingProfile profile, string id)
        {
            _profile = profile;

            ID = id;

            Initialize(referenceObject);
        }

        /// <summary>
        /// This invoke one time when this object is init on 'PoolingSystem'.
        /// </summary>
        protected virtual void Initialize(GameObject referenceObject) 
        {
            gameObject = referenceObject;
            transform = referenceObject.transform;

            PoolingManager.GlobalPoolingObjects.Add(this);
        }

        /// <summary>
        /// Life count down of this object.
        /// </summary>
        [HideInInspector] public float lifeTimerCountDown = 0f;

        /// <summary>
        /// Make object enabled.
        /// </summary>
        public void Enabled()
        {
            gameObject.SetActive(true);

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
            if (!gameObject.activeSelf) 
                return;

            gameObject.SetActive(false);

            IsActive = false;
            lifeTimerCountDown = 0f;
            transform.localPosition = Vector3.zero;

            if (transform.parent != _originalPerent)
                transform.SetParent(_originalPerent);

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
            PoolingManager.GlobalPoolingObjects.Remove(this);
        }
    }
}
