using System.Collections.Generic;
using UnityEngine;

namespace Services.Optimization.MeshAnimationSystem
{
    public class MeshAnimation : ScriptableObject
    {
        public bool isLoop = false;

        [HideInInspector] public float time;
        [HideInInspector] public List<MeshConllection> meshesCollection = new List<MeshConllection>();
    }

    [System.Serializable]
    public class MeshConllection
    {
        public string skinName;
        public List<Mesh> meshes = new List<Mesh>();
    }
}