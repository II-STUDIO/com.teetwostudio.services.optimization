using System.Collections.Generic;
using UnityEngine;

namespace Services.Optimization.PoolingSystem {

    /// <summary>
    /// This class is installer of customize pooling object;
    /// </summary>
    public class PoolSetting : MonoSingleton<PoolSetting>
    {
        [SerializeField] [Range(100, 900)] private int _maxCapacity = 600;
        [SerializeField] private Transform _container;

        public Transform Container
        {
            get => _container;
        }

        public int MaxCapacity
        {
            get => _maxCapacity;
        }
    }
}