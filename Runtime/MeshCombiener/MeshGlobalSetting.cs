using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MeshGlobalSetting : MonoBehaviour
{
    [Header("Shadow")]
    public ShadowCastingMode castShadow = ShadowCastingMode.On;
    [Header("Probe")]
    public LightProbeUsage lightProbe = LightProbeUsage.BlendProbes;
    public ReflectionProbeUsage reflectionProbe = ReflectionProbeUsage.BlendProbes;
    public Transform archorOverride;
    [Header("Addition Setting")]
    public bool dynamicOcclusion = true;

    [ContextMenu("ApplyToMeshRenderer")]
    public void MeshRendererFillter()
    {
        MeshRenderer[] renderer = FindObjectsOfType<MeshRenderer>();

        Apply(renderer);
    }

    public void Apply(Renderer[] renderers)
    {
        foreach(Renderer target in renderers)
        {
            target.shadowCastingMode = castShadow;
            target.lightProbeUsage = lightProbe;
            target.reflectionProbeUsage = reflectionProbe;
            target.probeAnchor = archorOverride;
            target.allowOcclusionWhenDynamic = dynamicOcclusion;
        }
    }
}
