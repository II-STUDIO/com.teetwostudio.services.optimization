using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Optimization.MeshAnimationSystem
{
    [System.Serializable]
    public class TransitionInfo
    {
        public int targetStateIndex;

        public List<Condition> conditions = new List<Condition>();
    }
}