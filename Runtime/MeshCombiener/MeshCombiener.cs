using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshCombiener : MonoBehaviour
{
    [SerializeField] MeshFilter outputMesh;
    [SerializeField] MeshFilter[] meshFilters;

    [ContextMenu("Setup")]
    void Setup()
    {
        outputMesh = GetComponent<MeshFilter>();
        meshFilters = GetComponentsInChildren<MeshFilter>();
    }

    void Start()
    {
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }
        outputMesh.mesh = new Mesh();
        outputMesh.mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
    }
}
