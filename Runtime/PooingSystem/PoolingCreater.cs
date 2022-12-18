using System;

namespace Services.Optimization.PoolingSystem
{
    /// <summary>
    /// This class use for contain prefab to create PoolingSystem
    /// </summary>
    [Serializable]
    public struct PoolingCreater
    {
        public string nameOfIndex;
        public PoolingProfile profile;
        public PoolingSystem poolingSystem;
    }
}
