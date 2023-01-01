using System.Collections.Generic;
using UnityEngine;

namespace Services.Optimization.PoolingSystem {

    /// <summary>
    /// This class is installer of customize pooling object;
    /// </summary>
    public class PoolSystemBaseHnadler : MonoBehaviour
    {
        [SerializeField] private InitMethod _initMethod;
        [SerializeField] private int maxCapacity = 600;
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

        /// <summary>
        /// Return true if is initialzed.
        /// </summary>
        public bool Initialized { get; private set; } = false;

        private void Awake()
        {
            PoolManager.SystemBaseHnadler = this;

            if (_initMethod != InitMethod.Awake) 
                return;

            Initialize();
        }

        private void Start()
        {
            if (_initMethod != InitMethod.Start) 
                return;

            if (Initialized) 
                return;

            Initialize();
        }

        void Initialize()
        {
            _createrDictionary.Clear();

            foreach (CreateLayer baker in _createLayers)
                baker.Initialize(_createrDictionary, _container);

            Initialized = true;
        }

        private void Update()
        {
            if (PoolManager.ActivatedPoolingObjectCount == 0)
                return;

            _deltaTime = Time.deltaTime;

            for(int i = 0; i < PoolManager.ActivatedPoolingObjectCount; i++)
            {
                _poolingObjectRef = PoolManager.GlobalPoolingObjects[PoolManager.ActivatePoolingObjects[i]];

                if (_poolingObjectRef == null)
                    continue;

                if (_poolingObjectRef.lifeTimerCountDown == 0f)
                    continue;

                _poolingObjectRef.lifeTimerCountDown -= _deltaTime;
                _poolingObjectRef.OnUpdate(_deltaTime);

                if (_poolingObjectRef.lifeTimerCountDown > 0f)
                    continue;

                _poolingObjectRef.Disabled();
            }
        }
    }

    public enum InitMethod { None, Awake, Start }
}