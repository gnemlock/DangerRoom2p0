/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/object-pools/ ---
 */

using UnityEngine;

namespace DataStructures
{
    using System.Collections.Generic;
    using Labels = Utility.ObjectPoolLabels;
    
    public class ObjectPool : MonoBehaviour 
    {
        private PooledObject pooledObjectPrefab;
        private List<PooledObject> availableObjects = new List<PooledObject>();
        
        public PooledObject GetObject()
        {
            PooledObject pooledObject;
            int lastAvailableIndex = availableObjects.Count - 1;
            
            if(lastAvailableIndex >= 0)
            {
                pooledObject = availableObjects[lastAvailableIndex];
                availableObjects.RemoveAt(lastAvailableIndex);
                pooledObject.gameObject.SetActive(true);
            }
            else
            {
                pooledObject = Instantiate<PooledObject>(pooledObjectPrefab);
                pooledObject.transform.SetParent(transform, false);
                pooledObject.objectPool = this;
            }
            
            return pooledObject;
        }
        
        public void AddObject(PooledObject pooledObject)
        {
            pooledObject.gameObject.SetActive(false);
            availableObjects.Add(pooledObject);
        }
        
        public static ObjectPool GetPool(PooledObject pooledObjectPrefab)
        {
            GameObject objectPoolGameObject;
            ObjectPool objectPool;
            
            #if UNITY_EDITOR
            objectPoolGameObject = GameObject.Find(pooledObjectPrefab.name + Labels.objectPool);
            
            if(objectPoolGameObject)
            {
                objectPool = objectPoolGameObject.GetComponent<ObjectPool>();
                
                if(objectPool)
                {
                    return objectPool;
                }
            }
            #endif
            
            objectPoolGameObject = new GameObject(pooledObjectPrefab.name + Labels.objectPool);
            DontDestroyOnLoad(objectPoolGameObject);
            objectPool = objectPoolGameObject.AddComponent<ObjectPool>();
            objectPool.pooledObjectPrefab = pooledObjectPrefab;
            
            return objectPool;
        }
    }
}

namespace DataStructures.Utility
{
    public static class ObjectPoolLabels
    {
        #if UNITY_EDITOR
        #endif
        
        public const string objectPool = "Pool";
    }
}