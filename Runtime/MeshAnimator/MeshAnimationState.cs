using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Optimization.MeshAnimationSystem
{
    [System.Serializable]
    public class MeshAnimationState
    {
        public MeshAnimation animation;
        public List<TransitionInfo> transitionInfos = new List<TransitionInfo>();
    }
}