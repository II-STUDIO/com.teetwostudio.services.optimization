using System.Collections.Generic;
using UnityEngine;

namespace Services.Optimization.PoolingSystem {

    /// <summary>
    /// This class is installer of customize pooling object;
    /// </summary>
    public class PoolSystemBaseHnadler : MonoSingleton<PoolSystemBaseHnadler>
    {
        [SerializeField] private bool _dontDestroyOnLoad = true;
        [SerializeField] private InitMethod _initMethod = InitMethod.Awake;
        [SerializeField] [Range(100, 900)] private int _maxCapacity = 600;
        [SerializeField] private Transform _container;
        [SerializeField] private List<CreateLayer> _createLayers = new List<CreateLayer>();

        private Dictionary<string, PoolerCreater> _createrDictionary = new Dictionary<string, PoolerCreater>();
        private PoolingObject _poolingObjectRef;

        private float _deltaTime;

        public List<CreateLayer> CreateLayers 
        { 
            get => _createLayers; 
            set => _createLayers = value; 
        }

        public int MaxCapacity
        {
            get => _maxCapacity;
        }

        /// <summary>
        /// Return true if is initialzed.
        /// </summary>
        public bool IsInitialized { get; private set; } = false;


        protected override void Awake()
        {
            base.Awake();

            SystemBaseUpdater.Instance.AddUpdater(OnUpdate);

            if (PoolManager.SystemBaseHnadler != null)
                Destroy(PoolManager.SystemBaseHnadler.gameObject);

            PoolManager.SystemBaseHnadler = this;

            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            if (_initMethod != InitMethod.Awake)
                return;

            Initialize();
        }

        private void Start()
        {
            if (_initMethod != InitMethod.Start) 
                return;

            if (IsInitialized) 
                return;

            Initialize();
        }

        void Initialize()
        {
            _createrDictionary.Clear();

            foreach (CreateLayer baker in _createLayers)
                baker.Initialize(_createrDictionary, _container);

            IsInitialized = true;
        }

        private void OnUpdate(float deltaTime)
        {
            if (PoolManager.ActivatedPoolingObjectCount == 0)
                return;

            for (int i = 0; i < PoolManager.ActivatedPoolingObjectCount; i++)
            {
                _poolingObjectRef = PoolManager.GlobalPoolingObjects[PoolManager.ActivatePoolingObjects[i]];

                if (_poolingObjectRef == null)
                    continue;

                _poolingObjectRef.OnUpdate(_deltaTime);

                if (_poolingObjectRef.lifeTimerCountDown == 0f)
                    continue;

                _poolingObjectRef.lifeTimerCountDown -= _deltaTime;

                if (_poolingObjectRef.lifeTimerCountDown > 0f)
                    continue;

                _poolingObjectRef.Disabled();
            }
        }
    }

    public enum InitMethod { None, Awake, Start }
}