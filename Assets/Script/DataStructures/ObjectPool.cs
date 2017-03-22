/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/object-pools/ ---
 */

using UnityEngine;

namespace DataStructures
{
    using System.Collections.Generic;
    using Labels = Utility.ObjectPoolLabels;

     /// <summary>Pools together like-objects of identical functionality for efficient recycling.</summary>
		 /// <remarks>An <see cref="DataStructures.ObjectPool"/> pools together objects of a shared prefab. When objects 
		 /// are not in use, they are stored inside a <see cref="System.Collections.Generic.List"/> for quick re-use. In 
		 /// order for an object to be re-usable, it needs to include a <see cref="DatStructures.PooledObject"/> 
		 /// component.</remarks>
    public class ObjectPool : MonoBehaviour 
    {
        /// <summary>The <see cref="DataStructures.PooledObject"/> reference to the prefab for the objects used in the 
        /// objects pool.</summary>
        private PooledObject pooledObjectPrefab;
        /// <summary>List of available <see cref="DataStructures.PooledObject"/> objects ready for 
        /// re-use.</summary>
        private List<PooledObject> availableObjects = new List<PooledObject>();

        /// <summary>Takes a <see cref="DataStructures.PooledObject"/>, and returns it to the 
        /// <see cref="DataStructures.ObjectPool"/>, deactivating the game object, and preparing 
        /// the <see cref="DataStructures.PooledObject"/> for re-use, later.</summary>
        /// <param name="pooledObject">The <see cref="DataStructures.PooledObject"/> being returned 
        /// to the <see cref="DataStructures.ObjectPool"/>.</param>
        public void AddObject(PooledObject pooledObject)
        {
            // De-activate the pooledObject being passed in, and add it to the availableObjects 
            // list for re-use.
            pooledObject.gameObject.SetActive(false);
            availableObjects.Add(pooledObject);
        }

        /// <summary>Gets a recycled <see cref="DataStructures.PooledObject"/> from the 
        /// <see cref="DataStructures.ObjectPool"/>. If one is not available, one will be created.</summary>
        /// <returns>The resulting <see cref="DataStructures.ObjectPool"/>.</returns>
        public PooledObject GetObject()
        {
            // Create a new PooledObject reference, and find the last index pointing to a valid 
            // entry in the availableObjects list. We use the last index, so the list does not 
            // have to shuffle all other objects down.
            PooledObject pooledObject;
            int lastAvailableIndex = availableObjects.Count - 1;
            
            if(lastAvailableIndex >= 0)
            {
                // If the lastAvailableIndex points to a valid index in the availableObjects list, we have a 
                // PooledObject available for re-use. Set it as our pooledObject reference, remove it from the 
                // availableObjects list and re-activate the game object.
                pooledObject = availableObjects[lastAvailableIndex];
                availableObjects.RemoveAt(lastAvailableIndex);
                pooledObject.gameObject.SetActive(true);
            }
            else
            {
                // Else, the availableObjects list does not contain any PooledObject references, so we do not have one 
                // available. Create a new instance using our pooledObjectPrefab; set it's parent transform to the 
                // local transform to retain hierarchial organisation, and set the pooledObject's objectPool reference 
                // to this ObjectPool, so it knows where to return to when it needs to be re-used.
                pooledObject = Instantiate<PooledObject>(pooledObjectPrefab);
                pooledObject.transform.SetParent(transform, false);
                pooledObject.objectPool = this;
            }
            
            // Return the resulting pooledObject reference.
            return pooledObject;
        }

        /// <summary>Creates an <see cref="DataStructures.ObjectPool"/> for use with the passed-in 
        /// <see cref="DataStructures.PooledObject"/>. If this method is called from within the editor, 
        /// attempts will be made to find a suitable <see cref="DataStructures.PooledObject"/> that has 
        /// already been created.</summary>
        /// <remarks>This method will try to find a suitable <see cref="DataStructures.ObjectPool"/> prior to 
        /// creating one, using <see cref="UnityEngine.GameObject.Find"/> and the <see cref="GameObject.name"/> 
        /// associated with the passed-in <see cref="DataStructures.PooledObject"/>. This functionality will only 
        /// run in the editor, as it is too expensive to be viable in a finished product.</remarks>
        /// <returns>The resulting <see cref="DataStructures.ObjectPool"/> for use in pooling instances of the passed-
        /// in <see cref="DataStructures.PooledObject"/>.</returns>
        /// <param name="pooledObjectPrefab">The <see cref="DataStructures.PooledObject"/> for which we intend to 
        /// get a working <see cref="DataStructures.ObjectPool"/>.</param>
        public static ObjectPool GetPool(PooledObject pooledObjectPrefab)
        {
            // Create references to the resulting ObjectPool, and the GameObject containing it.
            GameObject objectPoolGameObject;
            ObjectPool objectPool;

            #if UNITY_EDITOR
            // If we are in the editor, we can afford to perform a GameObject.Find, using the name standard we use 
            // when we create ObjectPools to attempt to locate a suitable ObjectPool.
            objectPoolGameObject = GameObject.Find(pooledObjectPrefab.name + Labels.objectPool);

            if(objectPoolGameObject != null)
            {
                // If objectPoolGameObject is not null, GameObject.Find was able to locate a suitable ObjectPool.
                // Attempt to retrieve the ObjectPool component attached to the objectPoolGameObject.
                objectPool = objectPoolGameObject.GetComponent<ObjectPool>();

                if(objectPool != null)
                {
                    // If objectPool is not null, our objectPoolGameObject was holding an ObjectPool; return it for use.
                    return objectPool;
                }
            }
            #endif

            // If we are not in the editor, or we could not find a usable ObjectPool, create a new GameObject with the 
            // name standard of "[pooledObjectPrefab.name] pool", set it to persist through level loading, and add 
            // an ObjectPool component, which we will also set as our objectPool reference. Set the prefab of our new 
            // objectPool as the passed-in pooledObjectPrefab.
            objectPoolGameObject = new GameObject(pooledObjectPrefab.name + Labels.objectPool);
            DontDestroyOnLoad(objectPoolGameObject);
            objectPool = objectPoolGameObject.AddComponent<ObjectPool>();
            objectPool.pooledObjectPrefab = pooledObjectPrefab;

            // Return our derived objectPool.
            return objectPool;
        }
    }
}

namespace DataStructures.Utility
{
    /// <summary>Used to store strings used for labelling buttons, "Prepare Change" descriptions and game objects.
    /// </summary>
    public static class ObjectPoolLabels
    {
        #if UNITY_EDITOR
        #endif

        /// <summary>The suffix used to append to the end of the name of the game object containing 
        /// object pools. The complete name should be "[objectPrefab.name] pool".</summary>
        public const string objectPool = " Pool";
    }
}