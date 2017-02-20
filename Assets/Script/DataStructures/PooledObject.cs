/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/object-pools/ ---
 */

using UnityEngine;

namespace DataStructures
{
    public class PooledObject : MonoBehaviour 
    {
        public ObjectPool objectPool { get; set; }

        [System.NonSerialized] private ObjectPool poolInstanceForPrefab;
        
        protected void OnLevelWasLoaded()
        {
            ReturnToPool();
        }
        
        public T GetPooledInstance<T>() where T : PooledObject
        {
            if(!poolInstanceForPrefab)
            {
                poolInstanceForPrefab = ObjectPool.GetPool(this);
            }
            
            return (T)poolInstanceForPrefab.GetObject();
        }
        
        public void ReturnToPool()
        {
            if(objectPool)
            {
                objectPool.AddObject(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}