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
        public List<PoolingCreater> poolingCreaters;

        public void SetupValue()
        {
            layerName = "Untitled";
            sourcePath = "";
            source = CreateSource.Defualt;
            poolingCreaters = new List<PoolingCreater>();
        }

        public void Initialize(Dictionary<string, PoolingCreater> createrDictionary, Transform container)
        {
            if (source == CreateSource.LoadPath)
                LoadPath(createrDictionary);

            StartCreate(createrDictionary, container);
        }

        private void StartCreate(Dictionary<string, PoolingCreater> createrDictionary, Transform container)
        {
            for(int i = 0;i< poolingCreaters.Count; i++)
            {
                PoolingCreater creater = poolingCreaters[i];

                if (createrDictionary.ContainsKey(creater.profile.GetInstanceID().ToString()))
                {
                    Debug.LogError("This object id : " + creater.profile.ID + " has aready initailzed check your prefab id");
                }
                else
                {
                    creater.poolingSystem = PoolingManager.CreatePoolingSystem(creater.profile, creater.profile.Amount);
                    if (container) creater.poolingSystem.root.SetParent(container);

                    createrDictionary.Add(creater.profile.ID, creater);
                }

                poolingCreaters[i] = creater;
            }
        }

        private void LoadPath(Dictionary<string, PoolingCreater> createrDictionary)
        {
            poolingCreaters.Clear();

            var assetProfiles = Resources.LoadAll<PoolingProfile>(sourcePath);
            var assetCount = assetProfiles.Length;
            for(int i = 0; i < assetCount; i++)
            {
                var profile = assetProfiles[i];

                if (!profile)
                    continue;

                PoolingCreater baker = new PoolingCreater();
                baker.nameOfIndex = profile.ID;
                baker.profile = profile;

                poolingCreaters.Add(baker);
            }
        }
    }
}
