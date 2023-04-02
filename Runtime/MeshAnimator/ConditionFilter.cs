namespace Services.Optimization.MeshAnimationSystem
{
    [System.Serializable]
    public class Condition
    {
        public virtual bool IsValide(ParameterInfo parameter)
        {
            return true;
        }
    }

    [System.Serializable]
    public class BoolCondition : Condition
    {
        public int parameterIndex;
        public bool targetValue;

        public override bool IsValide(ParameterInfo parameter)
        {
            bool value = parameter.boolParams[parameterIndex].value;
            return value == targetValue;
        }
    }

    [System.Serializable]
    public class FloatCondition : Condition
    {
        public int parameterIndex;
        public FloatConditionType type;
        public float targetValue;

        public override bool IsValide(ParameterInfo parameter)
        {
            float value = parameter.floatPrams[parameterIndex].value;

            switch (type)
            {
                case FloatConditionType.LessThan:
                    return value < targetValue;
                case FloatConditionType.GreaterThan:
                    return value > targetValue;
            }

            return value == targetValue;
        }
    }

    [System.Serializable]
    public class IntCondition : Condition
    {
        public int parameterIndex;
        public IntCondtionType type;
        public int targetValue;

        public override bool IsValide(ParameterInfo parameter)
        {
            int value = parameter.intParams[parameterIndex].value;

            switch (type)
            {
                case IntCondtionType.LessThan:
                    return value < targetValue;
                case IntCondtionType.GreaterThan:
                    return value > targetValue;
                case IntCondtionType.EquarlTo:
                    return value == targetValue;
            }

            return value == targetValue;
        }
    }

    [System.Serializable]
    public class TriggerCondition : Condition
    {
        public int parameterIndex;

        public override bool IsValide(ParameterInfo parameter)
        {
            bool value = parameter.triggerParams[parameterIndex].value;
            if(value == true)
            {
                parameter.triggerParams[parameterIndex].value = false;
                return true;
            }
            return false;
        }
    }

    public enum FloatConditionType
    {
        LessThan,
        GreaterThan
    }

    public enum IntCondtionType
    {
        LessThan,
        GreaterThan,
        EquarlTo
    }
}