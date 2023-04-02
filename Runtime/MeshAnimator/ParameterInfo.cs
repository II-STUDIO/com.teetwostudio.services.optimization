using System.Collections.Generic;

namespace Services.Optimization.MeshAnimationSystem
{
    [System.Serializable]
    public class ParameterInfo
    {
        public List<BoolParam> boolParams = new List<BoolParam>();
        public List<FloatParam> floatPrams = new List<FloatParam>();
        public List<IntParam> intParams = new List<IntParam>();
        public List<TriggerParam> triggerParams = new List<TriggerParam>();
    }

    public class Param
    {
        public int index;
    }

    [System.Serializable]
    public class BoolParam  : Param
    {
        public string name;
        public bool value;
    }

    [System.Serializable]
    public class FloatParam : Param
    {
        public string name;
        public float value;
    }

    [System.Serializable]
    public class IntParam : Param
    {
        public string name;
        public int value;
    }

    [System.Serializable]
    public class TriggerParam : Param
    {
        public string name;
        public bool value;
    }
}