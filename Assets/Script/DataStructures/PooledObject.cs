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