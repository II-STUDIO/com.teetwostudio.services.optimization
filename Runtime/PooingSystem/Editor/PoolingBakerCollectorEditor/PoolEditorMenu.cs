#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Services.Optimization.PoolingSystem.ScriptEditor
{
    public static class PoolEditorMenu
    {
        [MenuItem("GameObject/Pool SystemBase Handler")]
        public static void CreateAreaEventTriggerBox()
        {
            if (Object.FindObjectOfType<PoolSystemBaseHnadler>())
            {
                Debug.LogWarning("The object type of 'PoolSystemBaseHandler' was created in the scene");
                return;
            }

            GameObject gameObject = new GameObject("System - Pool Systembase Handler");
            gameObject.AddComponent(typeof(PoolSystemBaseHnadler));

            gameObject.transform.position = Vector3.zero;
        }
    }
}
#endif