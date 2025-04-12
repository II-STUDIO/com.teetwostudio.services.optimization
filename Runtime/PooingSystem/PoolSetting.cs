using System.Collections.Generic;
using UnityEngine;

namespace Services.Optimization.PoolingSystem {

    /// <summary>
    /// This class is installer of customize pooling object;
    /// </summary>
    public class PoolSetting : MonoSingleton<PoolSetting>
    {
        [SerializeField] [Range(100, 900)] private int _maxCapacity = 600;

        private Transform _container;

        public Transform Container
        {
            get
            {
                if (_container)
                    return _container;

                _container = new GameObject("Pool Container").transform;
                _container.SetParent(transform);
                return _container;
            }
        }

        public int MaxCapacity
        {
            get => _maxCapacity;
        }

        public void ClearContainer()
        {
            if (!_container)
                return;

            Destroy(_container.gameObject);
            _container = null;
        }
    }
}