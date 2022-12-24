using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Services.Optimization.PoolingSystem
{
    /// <summary>
    /// This class is supported to call one way of global pooling system width eazy method.
    /// </summary>
    public static class PoolingManager
    {
        /// <summary>
        /// Pooing cachse dictionay collection this use for collect pooling system for optimize staking same pooling.
        /// </summary>
        private static Dictionary<string, PoolingSystem> poolingRecycleDictionary = new Dictionary<string, PoolingSystem>();

        /// <summary>
        /// Pooling object has been arive and active or enabled.
        /// </summary>
        public static NativeList<int> ActivatePoolingObjects { get; private set; } = new NativeList<int>(1000, AllocatorManager.TempJob);

        /// <summary>
        /// Pooling object that create into the world.
        /// </summary>
        public static Dictionary<int, PoolingObject> GlobalPoolingObjects { get; private set; } = new Dictionary<int, PoolingObject>();

        public static int ActivatedPoolingObjectCount { get; private set; } = 0;
        public static int GlobalPoolingObjectCount { get; private set; } = 0;


        private static PoolingSystem poolingSystem_Ref;
        private static PoolingObject poolingObject_Ref;
        private static GameObject gameObject_Ref;

        public static void AssignGlobalPoolingObject(PoolingObject poolingObject)
        {
            poolingObject.index = GlobalPoolingObjectCount;

            GlobalPoolingObjects.Add(poolingObject.index, poolingObject);
            GlobalPoolingObjectCount++;
        }

        public static void UnAssigneGlobalPoolingObject(PoolingObject poolingObject)
        {
            GlobalPoolingObjects.Remove(poolingObject.index);
            GlobalPoolingObjectCount--;
        }

        /// <summary>
        /// Assign pooling object to active pooling object for make system base handle.
        /// </summary>
        /// <param name="poolingObject"></param>
        /// <returns>Arive index</returns>
        public static void AssignActivatePoolingObject(PoolingObject poolingObject)
        {
            ActivatePoolingObjects.Add(poolingObject.index);
            ActivatedPoolingObjectCount++;
        }

        /// <summary>
        /// Un assigne active pooling object for remove system base handle.
        /// </summary>
        /// <param name="index"></param>
        public static void UnAssignActivatePoolingObject(PoolingObject poolingObject)
        {
            for(int i = 0; i < ActivatedPoolingObjectCount; i++)
            {
                if(ActivatePoolingObjects[i] == poolingObject.index)
                {
                    ActivatePoolingObjects.RemoveAt(i);
                    ActivatedPoolingObjectCount--;
                    return;
                }
            }
        }

        public static PoolingObject PoolObject(string prefabID)
        {
            poolingSystem_Ref = GetPoolingSystem(prefabID);
            if (poolingSystem_Ref != null)
                return poolingSystem_Ref.PoolObject();
            else
            {
                Debug.LogError("The id '" + prefabID + "' is not exited you should create and init system for this prefab");
                return null;
            }
        }

        /// <summary>
        /// Use for pool object fom system had created with id
        /// </summary>
        /// <param name="prefabID"></param>
        /// <param name="poolPosition"></param>
        /// <param name="poolRotation"></param>
        /// <returns>Pooling object</returns>
        public static PoolingObject PoolObject(string prefabID, Vector3 poolPosition, Quaternion poolRotation)
        {
            poolingSystem_Ref = GetPoolingSystem(prefabID);
            if (poolingSystem_Ref != null)
                return poolingSystem_Ref.PoolObject(poolPosition, poolRotation);
            else
            {
                Debug.LogError("The " + prefabID + " is not exited you should create and init system for this prefab");
                return null;
            }
        }

        /// <summary>
        /// Use for pool object form system had created.
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns>Pooling object</returns>
        public static PoolingObject PoolObject(PoolingProfile prefab)
        {
            poolingSystem_Ref = GetPoolingSystem(prefab);
            if (poolingSystem_Ref != null)
                return poolingSystem_Ref.PoolObject();
            else
            {
                Debug.LogError("The " + prefab + " is not exited you should create and init system for this prefab");
                return null;
            }
        }

        /// <summary>
        /// Use for pool object form system had created width position and rotation.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="poolPosition"></param>
        /// <param name="poolRotation"></param>
        /// <returns>Pooling object</returns>
        public static PoolingObject PoolObject(PoolingProfile profile, Vector3 poolPosition, Quaternion poolRotation)
        {
            poolingSystem_Ref = GetPoolingSystem(profile);
            if (poolingSystem_Ref != null) 
                return poolingSystem_Ref.PoolObject(poolPosition, poolRotation);
            else
            {
                Debug.LogError("The " + profile + " is not exited you should create and init system for this prefab");
                return null;
            }
        }

        /// <summary>
        /// Use for pool object form system had created width set parent
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="parent"></param>
        /// <returns>Pooling object</returns>
        public static PoolingObject PoolObject(PoolingProfile profile, Transform parent)
        {
            poolingSystem_Ref = GetPoolingSystem(profile);
            if (poolingSystem_Ref != null) 
                return poolingSystem_Ref.PoolObject(parent);
            else
            {
                Debug.LogError("The " + profile + " is not exited you should create and init system for this prefab");
                return null;
            }
        }

        /// <summary>
        /// Use for pool object form system had created width set parent and set position, rotation
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="parent"></param>
        /// <returns>Pooling object</returns>
        public static PoolingObject PoolObject(PoolingProfile profile, Transform parent, Vector3 poolPosition, Quaternion poolRotation)
        {
            poolingSystem_Ref = GetPoolingSystem(profile);
            if (poolingSystem_Ref != null)
            {
                poolingObject_Ref = poolingSystem_Ref.PoolObject(poolPosition, poolRotation);
                poolingObject_Ref.transform.SetParent(parent);
                return poolingObject_Ref;
            }          
            else
            {
                Debug.LogError("The " + profile + " is not exited you should create and init system for this prefab");
                return null;
            }
        }

        /// <summary>
        /// Call for find pool system of PoolingObject prefab
        /// </summary>
        /// <param name="profile"></param>
        /// <returns>PoolingSystem Component of target prefab</returns>
        public static PoolingSystem GetPoolingSystem(PoolingProfile profile)
        {
            return GetPoolingSystem(profile.ID);
        }

        /// <summary>
        /// Call for find pool system of matches id
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns>PoolingSystem Component of id prefab</returns>
        public static PoolingSystem GetPoolingSystem(string id)
        {
            if (poolingRecycleDictionary.ContainsKey(id))
                return poolingRecycleDictionary[id];
            return null;
        }

        /// <summary>
        /// Call for create poolingSystem of target prefab defualt.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="amount"></param>
        /// <returns>PoolingSystem Componet of created target prefab</returns>
        public static PoolingSystem CreatePoolingSystem(PoolingProfile profile, int amount = 5)
        {
            poolingSystem_Ref = TryCreate(profile, amount);

            if (poolingSystem_Ref != null) 
                return poolingSystem_Ref;

            poolingSystem_Ref = Create(profile, amount);

            poolingRecycleDictionary.Add(profile.ID, poolingSystem_Ref);

            return
                poolingSystem_Ref;
        }

        /// <summary>
        /// Try create pooling system of target prefab if is existed this method will add amound staked.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="amount"></param>
        /// <returns>PoolingSystem Componet of created target prefab</returns>
        static PoolingSystem TryCreate(PoolingProfile profile, int amount = 5)
        {
            if (poolingRecycleDictionary.TryGetValue(profile.ID, out poolingSystem_Ref))
            {
                if (amount > poolingSystem_Ref.ObjectCount)
                    poolingSystem_Ref.AddCount(amount - poolingSystem_Ref.ObjectCount);
                return
                    poolingSystem_Ref;
            }
            else
                return null;
        }

        /// <summary>
        /// Use to create pooling system
        /// </summary>
        /// <param name="proifle"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        static PoolingSystem Create(PoolingProfile proifle, int amount = 5)
        {
            string id = proifle.ID == string.Empty ? proifle.name : proifle.ID;
            string name = "Pooling System name : <" + proifle.name + "> id : <" + id + ">";

            gameObject_Ref = new GameObject(name);
            poolingSystem_Ref = new PoolingSystem();

            poolingSystem_Ref.Initialize(proifle, amount, gameObject_Ref.transform);

            return poolingSystem_Ref;
        }

        /// <summary>
        /// Disabled all pooling object.
        /// </summary>
        public static void DisabledAll()
        {
            foreach(PoolingSystem system in poolingRecycleDictionary.Values)
            {
                system.DisabledAll();
            }
        }

        /// <summary>
        /// Call to clear dictionary of pooling data case.
        /// </summary>
        public static void ClearRecycleDictionary() => poolingRecycleDictionary.Clear();

        /// <summary>
        /// Call to remove pooling width matches id.
        /// </summary>
        /// <param name="id"></param>
        public static void RemoveFormRecycleDictionary(string id) => poolingRecycleDictionary.Remove(id);
    }
}
