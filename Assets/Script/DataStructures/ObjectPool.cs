/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/object-pools/ ---
 */

using UnityEngine;
using System.Collections.Generic;

namespace DataStructures
{
    public class ObjectPool : MonoBehaviour 
    {
        PooledObject pooledObjectPrefab;
        
        [System.NonSerialized] ObjectPool poolInstanceForPrefab;
        
        public PooledObject GetObject()
        {
            PooledObject pooledObject = Instantiate<PooledObject>(pooledObjectPrefab);
            
            pooledObject.transform.SetParent(transform, false);
            pooledObject.objectPool = this;
            
            return pooledObject;
        }
        
        public T GetPooledInstance<T>() where T : PooledObject
        {
            if(!poolInstanceForPrefab)
            {
                poolInstanceForPrefab = ObjectPool.GetPool(this);
            }
            
            return (T)poolInstanceForPrefab.GetObject();
        }
        
        public void AddObject(PooledObject pooledObject)
        {
            Object.Destroy(pooledObject);
        }
        
        public static ObjectPool GetPool(PooledObject pooledObjectPrefab)
        {
            GameObject objectPoolGameObject 
                = new GameObject(pooledObjectPrefab.name + "Pool");
            ObjectPool objectPool = objectPoolGameObject.AddComponent<ObjectPool>();
            
            objectPool.pooledObjectPrefab = pooledObjectPrefab;
            return objectPool;
        }
    }
}