using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Services.Optimization.PoolingSystem
{
    /// <summary>
    /// This class is supported to call one way of global pooler width eazy method.
    /// </summary>
    public static class PoolManager
    {
        /// <summary>
        /// Pooing profile cachse dictionay collection this use for collect pooler for optimize staking same pooling.
        /// </summary>
        private static Dictionary<string, object> poolingProfileRecycleDictionary = new Dictionary<string, object>();

        /// <summary>
        /// Indexes of pooling object has been arive and active or enabled.
        /// </summary>
        public static List<int> ActivatePoolingObjects { get; private set; } = new List<int>(1000);

        /// <summary>
        /// Pooling object that create into the world.
        /// </summary>
        public static Dictionary<int, PoolingObject> GlobalPoolingObjects { get; private set; } = new Dictionary<int, PoolingObject>();

        public static int ActivatedPoolingObjectCount { get; private set; } = 0;
        public static int GlobalPoolingObjectCount { get; private set; } = 0;
        public static PoolSystemBaseHandler SystemBaseHnadler { get; internal set; }

        private static object object_ref;

        private static GameObject gameObject_Ref;

        public static void AssignGlobalPoolingObject(PoolingObject poolingObject)
        {
            poolingObject.Index = GlobalPoolingObjectCount;

            GlobalPoolingObjects.Add(poolingObject.Index, poolingObject);
            GlobalPoolingObjectCount++;
        }

        public static void UnAssigneGlobalPoolingObject(PoolingObject poolingObject)
        {
            GlobalPoolingObjects.Remove(poolingObject.Index);
            GlobalPoolingObjectCount--;
        }

        /// <summary>
        /// Assign pooling object to active pooling object for make system base handle.
        /// </summary>
        /// <param name="poolingObject"></param>
        public static void AssignActivatePoolingObject(PoolingObject poolingObject)
        {
            ActivatePoolingObjects.Add(poolingObject.Index);
            ActivatedPoolingObjectCount++;
        }

        /// <summary>
        /// Un assigne active pooling object for remove system base handle.
        /// </summary>
        /// <param name="poolingObject"></param>
        public static void UnAssignActivatePoolingObject(PoolingObject poolingObject)
        {
            for(int i = 0; i < ActivatedPoolingObjectCount; i++)
            {
                if(ActivatePoolingObjects[i] == poolingObject.Index)
                {
                    ActivatePoolingObjects.RemoveAt(i);
                    ActivatedPoolingObjectCount--;
                    return;
                }
            }
        }

        /// <summary>
        /// Call for find pooler of PoolingObject profile.
        /// </summary>
        /// <param name="profile"></param>
        /// <returns>Pooler of proifle object prefab</returns>
        public static ObjectPooler<TPooingObject> GetPooler<TPooingObject>(PoolProfile profile) where TPooingObject : PoolingObject
        {
            return GetPooler<TPooingObject>(profile.ID);
        }

        /// <summary>
        /// Call for find pooler of PoolingObject profile with matches id.
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns>Pooler of proifle object prefab of matches id prefab</returns>
        public static ObjectPooler<TPooingObject> GetPooler<TPooingObject>(string profileId) where TPooingObject : PoolingObject
        {
            if (poolingProfileRecycleDictionary.ContainsKey(profileId))
                return poolingProfileRecycleDictionary[profileId] as ObjectPooler<TPooingObject>;

            Debug.LogErrorFormat($"Object Pooler of the profile id <{profileId}> not find or created");

            return null;
        }

        //
        // Summary:
        //     Use to create pooler of pool profile or get exited.
        /// <summary>
        /// Call for create pooler of PoolingObject profile.
        /// </summary>
        /// <param name="profile"></param>
        /// <returns>PoolingSystem Componet of created target prefab</returns>
        public static ObjectPooler<TPooingObject> CreatePooler<TPooingObject>(PoolProfile profile) where TPooingObject : PoolingObject
        {
            if(SystemBaseHnadler == null)
            {
                Debug.LogErrorFormat("Please setup 'PoolSystemBaseHandler' on the scene");
                return null;
            }

            ObjectPooler<TPooingObject> pooler = TryCreate<TPooingObject>(profile, profile.Amount);

            if (pooler != null) 
                return pooler;

            pooler = Create<TPooingObject>(profile, profile.Amount);

            poolingProfileRecycleDictionary.Add(profile.ID, pooler);

            return pooler;
        }

        /// <summary>
        /// Try create pooler of pooling object prefab if is existed this method will add amound staked.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="amount"></param>
        /// <returns>PoolingSystem Componet of created target prefab</returns>
        private static ObjectPooler<TPoolingObject> TryCreate<TPoolingObject>(PoolProfile profile, int amount) where TPoolingObject : PoolingObject
        {
            if (poolingProfileRecycleDictionary.TryGetValue(profile.ID, out object_ref))
            {
                ObjectPooler<TPoolingObject> pooler = object_ref as ObjectPooler<TPoolingObject>;

                if (amount > pooler.ObjectCount)
                    pooler.AddCount(amount - pooler.ObjectCount);

                return pooler;
            }

            return null;
        }

        /// <summary>
        /// Use to create pooler.
        /// </summary>
        /// <param name="proifle"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        private static ObjectPooler<TPoolingObject> Create<TPoolingObject>(PoolProfile proifle, int amount = 1) where TPoolingObject : PoolingObject
        {
            string id = proifle.ID == string.Empty ? proifle.name : proifle.ID;
            string name = "Pooling System name : <" + proifle.name + "> id : <" + id + ">";

            gameObject_Ref = new GameObject(name);

            if (SystemBaseHnadler.Container)
                gameObject_Ref.transform.SetParent(SystemBaseHnadler.Container);

            ObjectPooler<TPoolingObject> pooler = new ObjectPooler<TPoolingObject>();

            pooler.Initialize(proifle, amount, gameObject_Ref.transform);

            return pooler;
        }

        /// <summary>
        /// Disabled all pooling object.
        /// </summary>
        public static void DisabledAll()
        {
            foreach(ObjectPooler<PoolingObject> system in poolingProfileRecycleDictionary.Values)
            {
                system.DisabledAll();
            }
        }

        /// <summary>
        /// Call to clear dictionary of pooling data case.
        /// </summary>
        public static void ClearRecycleDictionary() => poolingProfileRecycleDictionary.Clear();

        /// <summary>
        /// Call to remove pooling width matches id.
        /// </summary>
        /// <param name="id"></param>
        public static void RemoveFormRecycleDictionary(string id) => poolingProfileRecycleDictionary.Remove(id);
    }
}
