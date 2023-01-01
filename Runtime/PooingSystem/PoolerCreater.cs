using System;

namespace Services.Optimization.PoolingSystem
{
    /// <summary>
    /// This class use for contain prefab to create PoolingSystem
    /// </summary>
    [Serializable]
    public struct PoolerCreater
    {
        public string nameOfIndex;
        public PoolProfile profile;
        public ObjectPooler<PoolingObject> poolingSystem;
    }
}
