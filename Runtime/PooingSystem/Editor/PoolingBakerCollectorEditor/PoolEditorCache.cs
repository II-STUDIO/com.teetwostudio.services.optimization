#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

namespace Services.Optimization.PoolingSystem.ScriptEditor
{
    public enum ObjectPreview { Off,On }
    public static class PoolEditorCache
    {
        public static ObjectPreview objectPreview;
        private static Dictionary<string, PoolProfile> objectsCacheDict = new Dictionary<string, PoolProfile>();

        public static bool AddObjectCacheSucces(PoolProfile profile)
        {
            if (!profile) 
                return true;

            string id = profile.ID;

            if (!objectsCacheDict.ContainsKey(id))
            {
                objectsCacheDict.Add(id, profile);
                return true;
            }

            var cacheObject = objectsCacheDict[id];
            if (cacheObject == profile)
                return true;

            return false;
        }

        [InitializeOnLoadMethod]
        static void OnEditorRecomplute()
        {
            objectsCacheDict.Clear();
        }
    }
}
#endif