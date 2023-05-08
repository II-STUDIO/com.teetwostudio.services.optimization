using UnityEngine;

namespace Services.Optimization.PoolingSystem
{
    /// <summary>
    /// Pooling object controllable of prefab for poolable.
    /// </summary>
    public class PoolingObject : MonoBehaviour
    {
        private PoolProfile _profile;
        private Transform _originalPerent;

        [SerializeField] private bool isUpdatable = true;
        [Space]
        [HideInInspector] public float lifeTimerCountDown = 0f;

        /// <summary>
        /// The fasted for access this transform of this pooling object.
        /// </summary>
        public Transform transformCache { get; private set; }
        public GameObject gameObjectCache { get; private set; }

        public int Index { get; set; }

        /// <summary>
        /// ID of the profile of this pooling Object.
        /// </summary>
        public string ID { get; private set; }
        public bool IsActive { get; private set; } = false;
        public bool IsUpdatable { get => isUpdatable; }

        public void SetOriginalPerent(Transform targetPerent) => _originalPerent = targetPerent;


        /// <summary>
        /// This invoke one time when this object is init on 'PoolingSystem'.
        /// </summary>
        public virtual void Initialize(PoolProfile profile, string profileId) 
        {
            _profile = profile;

            ID = profileId;

            gameObjectCache = gameObject;
            transformCache = gameObjectCache.transform;

            PoolManager.AssignGlobalPoolingObject(this);
        }

        private void ComputeAndUdpate(float deltaTime)
        {
            OnUpdate(deltaTime);

            if (lifeTimerCountDown == 0f)
                return;

            lifeTimerCountDown -= deltaTime;

            if (lifeTimerCountDown > 0f)
                return;

            Disabled();
        }

        /// <summary>
        /// Invoke every frame like 'Update' of unity mono behaviour
        /// </summary>
        /// <param name="deltaTime"></param>
        public virtual void OnUpdate(float deltaTime)
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

            if (IsUpdatable)
            {
                //SystemBaseUpdater.Instance.AddUpdater(ComputeAndUdpate);
                PoolManager.AssignActivatePoolingObject(this);
            }

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

            if (IsUpdatable)
            {
               // SystemBaseUpdater.Instance.RemoveUpdater(ComputeAndUdpate);
                PoolManager.UnAssignActivatePoolingObject(this);
            }

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
            PoolManager.UnAssigneGlobalPoolingObject(this);
        }
    }
}
