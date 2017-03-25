/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/object-pools/ ---
 */

using UnityEngine;

namespace DataStructures
{
    using Log = Utility.DataStructuresDebug;
    using StringFormat = Utility.DataStructuresStringFormats;
    using Tooltips = Utility.PooledObjectTooltips;

    /// <summary>Provides basic functionality for using a <see cref="UnityEngine.GameObject"/> in an 
    /// <see cref="DataStructures.ObjectPool"/>.</summary>
    public class PooledObject : MonoBehaviour 
    {
        #region Variables
        /// <summary>The <see cref="DataStructures.ObjectPool"/> to which this <see cref="DataStructures.PooledObject"/> 
        /// belongs.</summary>
        /// <remarks>This is the <see cref="DataStructures.ObjectPool"/> to which this
        ///  <see cref="DataStructures.PooledObject"/>  will return on level load or destroy.</remarks>
        [Tooltip(Tooltips.objectPool)] public ObjectPool objectPool;

        /// <summary>The <see cref="DataStructures.ObjectPool"/> instance reference. Primarily used 
        /// to allow instance references via prefabs.</summary>
        [System.NonSerialized] private ObjectPool poolInstanceForPrefab;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the <see cref="DataStructures.PooledObject"/> 
        /// class.</summary>
        public PooledObject()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="DataStructures.PooledObject"/> 
        /// class with a parent <see cref="DataStructures.ObjectPool"/> reference.</summary>
        /// <param name="objectPool">The <see cref="DataStructures.ObjectPool"/> to which this 
        /// <see cref="DataStructures.PooledObject"/> belongs, and returns, when recycled.</param>
        public PooledObject(ObjectPool objectPool)
        {
            this.objectPool = objectPool;
        }
        #endregion

        #region MonoBehaviour Events and Parent Overrides
        /// <summary>This method will be called just before the first Update call.</summary>
        protected virtual void Start()
        {
            if(gameObject == null)
            {
                // If the gameObject reference is null, send a warning to the Debug.
                Log.CreatingPooledObjectWithoutGameObject(this);
            }
        }

        /// <summary>This method will be called when this instance of 
        /// <see cref="DataStructures.PooledObject"/>  is enabled and set as active.</summary>
        protected void OnEnable()
        {
            // Add the OnSceneLoaded method to the sceneLoaded event.
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>This method will be called when this instance of 
        /// <see cref="DataStructures.PooledObject"/> is deactivated or set as disabled.</summary>
        protected void OnDisable()
        {
            // Remove the OnSceneLoaded method from the sceneLoaded event.
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>Returns a <see cref="System.String"/> that represents the current 
        /// <see cref="DataStructures.PooledObject"/>.</summary>
        /// <returns>A <see cref="System.String"/> that represents the current 
        /// <see cref="DataStructures.PooledObject"/>.</returns>
        public override string ToString()
        {
            // If the objectPool is null, we do not have a reference; print the name of this 
            // PooledObject. If there is an objectPool, also print the name of the objectPool.
            return (objectPool == null ? string.Format(StringFormat.pooledObjectWithoutPool, name)
                : string.Format(StringFormat.pooledObjectWithPool, name, objectPool.name));
        }
        #endregion

        #region Class Methods
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

        /// <summary>This method will be called every time a new scene is loaded.</summary>
        /// <remarks> This method allows interface with the 
        /// <see cref="UnityEngine.SceneManagement.SceneManager.sceneLoaded"/> event.</remarks>
        /// <param name="scene">The current <see cref="UnityEngine.SceneManagement.Scene"/> 
        /// being loaded.</param>
        /// <param name="loadSceneMode">The <see cref="UnityEngine.SceneManagement.LoadSceneMode"/> 
        /// for the <see cref="UnityEngine.SceneManagement.Scene"/> being loaded.</param>
        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, 
            UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
        {
            // Return the PooledObject to the objectPool.
            ReturnToPool();
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
        #endregion
    }
}//TODO:Make advanced version that also includes data structure for initialisation?

namespace DataStructures.Utility
{
    public static partial class DataStructuresDebug
    {
        #region Warning Messages
        private const string creatingPooledObjectWithoutGameObject = "Warning: Creating a "
            + "PooledObject component without a valid gameObject reference.";
        #endregion

        #region Warnings
        /// <summary>Provides debug concerning creation of a PooledObject without a valid 
        /// gameObject.</summary>
        /// <param name="pooledObject">The <see cref="DataStructures.PooledObject"/>.</param>
        /// <remarks>If a <see cref="DataStructures.PooledObject"/> does not have a valid 
        /// <see cref="UnityEngine.GameObject"/>, to which it is attached, the 
        /// <see cref="DataStructures.PoolableObject"/> likely lacks any functionality.</remarks>
        public static void CreatingPooledObjectWithoutGameObject(PooledObject pooledObject)
        {
            Debug.Log(creatingPooledObjectWithoutGameObject, pooledObject);
        }
        #endregion
    }

    /// <summary>This class holds string formats for the DataStructures namespace.</summary>
    public static partial class DataStructuresStringFormats
    {
        /// <summary>String output prefix for a PooledObject, with the intention of being formatted 
        /// with the object name.</summary>
        public const string pooledObject = "PooledObject ({0} : ";
        /// <summary>String output suffix for a PooledObject with a valid ObjectPool, with the 
        /// intention of being formatted with the objectPool name.</summary>
        public const string pooledObjectPool = "{1})";
        /// <summary>String output suffix for a PooledObject with no valid ObjectPool.</summary>
        public const string noObjectPool = "NULL POOL)";
        /// <summary>Formats the string output of a PooledObject with a valid ObjectPool, with the 
        /// intention of being formatted with the object name, and the objectPool name.</summary>
        public const string pooledObjectWithPool = pooledObject + pooledObjectPool;
        /// <summary>Formats the string output of a PooledObject with no valid ObjectPool, with the 
        /// intention of being formatted with the object name.</summary>
        public const string pooledObjectWithoutPool = pooledObject + noObjectPool;
    }

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