using System.Collections.Generic;
using UnityEngine;

namespace Services.Optimization.PoolingSystem {

    /// <summary>
    /// This class is installer of customize pooling object;
    /// </summary>
    public class PoolingSystemBaseHnadler : MonoBehaviour
    {
        [SerializeField] private InitMethod _initMethod;
        [SerializeField] private Transform _container;
        [SerializeField] private List<CreateLayer> _bakerLayers = new List<CreateLayer>();

        private Dictionary<string, PoolingCreater> _createrDictionary = new Dictionary<string, PoolingCreater>();
        private PoolingObject _poolingObjectRef;

        private float _deltaTime;

        public List<CreateLayer> BakerLayers 
        { 
            get => _bakerLayers; 
            set => _bakerLayers = value; 
        }

        /// <summary>
        /// Return true if is initialzed.
        /// </summary>
        public bool Initialized { get; private set; } = false;

        private void Awake()
        {
            if (_initMethod != InitMethod.Awake) return;
            Initialize();
        }

        private void Start()
        {
            if (_initMethod != InitMethod.Start) return;
            if (Initialized) return;
            Initialize();
        }

        void Initialize()
        {
            _createrDictionary.Clear();
            foreach (CreateLayer baker in _bakerLayers)
                baker.Initialize(_createrDictionary, _container);

            Initialized = true;
        }

        private void Update()
        {
            if (PoolingManager.ActivatedPoolingObjectCount == 0)
                return;

            _deltaTime = Time.deltaTime;

            for(int i = 0; i < PoolingManager.ActivatedPoolingObjectCount; i++)
            {
                _poolingObjectRef = PoolingManager.GlobalPoolingObjects[PoolingManager.ActivatePoolingObjects[i]];

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