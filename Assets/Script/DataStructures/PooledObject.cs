/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/object-pools/ ---
 */

using UnityEngine;

namespace DataStructures
{
    using Tooltips = Utility.PooledObjectTooltips;

    /// <summary>Provides basic functionality for using a <see cref="UnityEngine.GameObject"/> in an 
    /// <see cref="DataStructures.ObjectPool"/>.</summary>
    public class PooledObject : MonoBehaviour 
    {
        /// <summary>The <see cref="DataStructures.ObjectPool"/> to which this <see cref="DataStructures.PooledObject"/> 
        /// belongs.</summary>
        /// <remarks>This is the <see cref="DataStructures.ObjectPool"/> to which this
        ///  <see cref="DataStructures.PooledObject"/>  will return on level load or destroy.</remarks>
        [Tooltip(Tooltips.objectPool)] public ObjectPool objectPool { get; set; }

        [System.NonSerialized] private ObjectPool poolInstanceForPrefab;

        /// <summary>This method will be called when this instance of 
        /// <see cref="DataStructures.PooledObject"/>  is enabled and set as active.</summary>
        private void OnEnable()
        {
            // Add the ReturnToPool method to the sceneLoaded event.
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += ReturnToPool;
        }

        /// <summary>This method will be called when this instance of 
        /// <see cref="DataStructures.PooledObject"/> is deactivated or set as disabled.</summary>
        private void OnDisable()
        {
            // Remove the ReturnToPool method from the sceneLoaded event.
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= ReturnToPool;
        }

        /// <summary>Gets a pooled instance of this <see cref="DataStructures.PooledObject"/>.
        /// </summary>
        /// <remarks>This method specifically allows for instance access through a prefab. This 
        /// method has been set up as a generic, to allow compatibility with more advanced 
        /// pooled objects.</remarks>
        /// <returns>The pooled instance of this this <see cref="DataStructures.PooledObject"/>.
        /// </returns>
        /// <typeparam name="T">The type variation of <see cref="DataStructures.PooledObject"/> 
        /// being retrieved from this method.</typeparam>
        public T GetPooledInstance<T>() where T : PooledObject
        {
            if(poolInstanceForPrefab == null)
            {
                // If there is currently no poolInstanceForPrefab ObjectPool available, 
                // Set up the poolInstaceForPrefab ObjectPool using this PooledObject as the prefab.
                poolInstanceForPrefab = ObjectPool.GetPool(this);
            }

            // Retrieve an object from the poolInstanceForPrefab ObjectPool, and return it as the 
            // specified type.
            return (T)poolInstanceForPrefab.GetObject();
        }

        /// <summary>Returns the <see cref="UnityEngine.GameObject"/> associated with this 
        /// <see cref="DataStructures.PooledObject"/> to it's 
        /// <see cref="DataStructures.ObjectPool"/>. If there is no 
        /// <see cref="DataStructures.ObjectPool"/>, the object will be destroyed.</summary>
        public void ReturnToPool()
        {
            if(objectPool)
            {
                // If there is an objectPool associated with this PooledObjet, return it back to that objectPool.
                // This will also deactivate the object.
                objectPool.AddObject(this);
            }
            else
            {
                // Else, there is no associated objectPool, so we need to Destroy this object.
                #if UNITY_EDITOR
                // If we are still in the Unity Editor, we can Destroy the gameObject Immediately.
                DestroyImmediate(gameObject);
                #else
                // Else, we should mark the gameObject for the garbage collector to Destroy.
                Destroy(gameObject);
                #endif
            }
        }
    }
}//TODO:Make advanced version that also includes data structure for initialisation?

namespace DataStructures.Utility
{
    /// <summary>Used to store tooltip strings for use in the Inspector.</summary>
    /// <remarks>Tooltip strings share a name with their corresponding variable.</remarks>
    public static class PooledObjectTooltips
    {
        #if UNITY_EDITOR
        public const string objectPool = "The ObjectPool this PooledObject belongs to, and "
            + "returns to, when recycled.";
        #endif
    }
}