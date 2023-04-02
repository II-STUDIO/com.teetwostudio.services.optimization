using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Optimization.MeshAnimationSystem
{
    public class MeshAnimatorController : ScriptableObject
    {
        public int fps;
        public int defaultStateIndex = 0;
        public ParameterInfo parameterInfo;
        [Space]
        public List<MeshAnimationState> states = new List<MeshAnimationState>();
    }
}