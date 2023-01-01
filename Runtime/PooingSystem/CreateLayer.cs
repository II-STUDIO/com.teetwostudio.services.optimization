using System.Collections.Generic;
using UnityEngine;

namespace Services.Optimization.PoolingSystem
{
    [System.Serializable]
    public struct CreateLayer
    {
        public string layerName;
        public string sourcePath;
        public CreateSource source;
        public List<PoolerCreater> poolingCreaters;

        public void SetupValue()
        {
            layerName = "Untitled";
            sourcePath = "";
            source = CreateSource.Defualt;
            poolingCreaters = new List<PoolerCreater>();
        }

        public void Initialize(Dictionary<string, PoolerCreater> createrDictionary, Transform container)
        {
            if (source == CreateSource.LoadPath)
                LoadPath(createrDictionary);

            StartCreate(createrDictionary, container);
        }

        private void StartCreate(Dictionary<string, PoolerCreater> createrDictionary, Transform container)
        {
            for(int i = 0;i< poolingCreaters.Count; i++)
            {
                PoolerCreater creater = poolingCreaters[i];

                if (createrDictionary.ContainsKey(creater.profile.GetInstanceID().ToString()))
                {
                    Debug.LogError("This object id : " + creater.profile.ID + " has aready initailzed check your prefab id");
                }
                else
                {
                    creater.poolingSystem = PoolManager.CreatePooler<PoolingObject>(creater.profile);
                    if (container) creater.poolingSystem.root.SetParent(container);

                    createrDictionary.Add(creater.profile.ID, creater);
                }

                poolingCreaters[i] = creater;
            }
        }

        private void LoadPath(Dictionary<string, PoolerCreater> createrDictionary)
        {
            poolingCreaters.Clear();

            var assetProfiles = Resources.LoadAll<PoolProfile>(sourcePath);
            var assetCount = assetProfiles.Length;
            for(int i = 0; i < assetCount; i++)
            {
                var profile = assetProfiles[i];

                if (!profile)
                    continue;

                PoolerCreater baker = new PoolerCreater();
                baker.nameOfIndex = profile.ID;
                baker.profile = profile;

                poolingCreaters.Add(baker);
            }
        }
    }
}
