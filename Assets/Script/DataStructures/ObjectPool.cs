/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/object-pools/ ---
 */

using UnityEngine;

namespace DataStructures
{
    using System.Collections.Generic;
    using System;

    using Labels = Utility.ObjectPoolLabels;
    using Log = Utility.DataStructuresDebug;
    using StringFormat = Utility.DataStructuresStringFormats;

    /// <summary>Pools together like-objects of identical functionality for efficient recycling.
    /// </summary>
    /// <remarks>An <see cref="DataStructures.ObjectPool"/> pools together objects of a shared 
    /// prefab. When objects are not in use, they are stored inside a 
    /// <see cref="System.Collections.Generic.List"/> for quick re-use. In order for an object to 
    /// be re-usable, it needs to include a <see cref="DatStructures.PooledObject"/> component.
    /// </remarks>
    public class ObjectPool : MonoBehaviour 
    {
        #region Variables
        /// <summary>The <see cref="DataStructures.PooledObject"/> reference to the prefab for the 
        /// objects used in the objects pool.</summary>
        private PooledObject pooledObjectPrefab;
        /// <summary>List of available <see cref="DataStructures.PooledObject"/> objects ready for 
        /// re-use.</summary>
        private List<PooledObject> availableObjects = new List<PooledObject>();
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the <see cref="DataStructures.ObjectPool"/> 
        /// class with a <see cref="DataStructures.PooledObject"/> prefab.</summary>
        /// <param name="pooledObjectPrefab">The prefab to use for creating more instances.</param>
        /// <remarks><see cref="DataStructures.ObjectPool.pooledObjectPrefab"/> is a required 
        /// component for instantiating new <see cref="DataStructures.PooledObject"/> instances. As 
        /// such, care should be made that the reference is not broken. Ideally, reference will be 
        /// passed to a prefab, not an instance.</remarks>
        public ObjectPool(PooledObject pooledObjectPrefab)
        {
            this.pooledObjectPrefab = pooledObjectPrefab;
        }
        #endregion

        #region MonoBehaviour Events and Parent Overrides
        /// <summary>This method will be called just before the first Update call.</summary>
        private override void Start()
        {
            if(pooledObjectPrefab == null)
            {
                // If there is no pooledObjectPrefab, send a warning to the Debug.
                Log.CreatingObjectPoolWithNoPrefabWarning(this);
            }
        }

        /// <summary>Returns a <see cref="System.String"/> that represents the current 
        /// <see cref="DataStructures.ObjectPool"/>.</summary>
        /// <returns>A <see cref="System.String"/> that represents the current 
        /// <see cref="DataStructures.ObjectPool"/>.</returns>
        public override string ToString()
        {
            // If the pooledObjectPrefab is null, we do not have a reference; ommit the name of the 
            // pooledObjectPrefab and print the size of the availableObjects list. If there is a 
            // pooledObjectPrefab, do not ommit it's name.
            return (pooledObjectPrefab == null ? 
                string.Format(StringFormat.objectPoolWithoutPrefab, availableObjects.Count)
                : string.Format(StringFormat.objectPoolWithPrefab, pooledObjectPrefab.name, 
                    availableObjects.Count));
        }
        #endregion

        #region Class Methods
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
        /// <see cref="DataStructures.ObjectPool"/>. If one is not available, one will be created.
        /// </summary>
        /// <returns>The resulting <see cref="DataStructures.ObjectPool"/>.</returns>
        /// <remarks>This method uses a try..catch statement to handle the possibility of needing 
        /// to instantiate a new instance of the <see cref="DataStructures.ObjectPool"/>, when we 
        /// do not have a valid <see cref="DataStructures.ObjectPool.pooledObjectPrefab"/> set. In 
        /// such circumstances, a default <see cref="DataStructures.PooledObject"/> will be 
        /// returned, instead.</remarks>
        public PooledObject GetObject()
        {
            // Create a new PooledObject reference, and find the last index pointing to a valid 
            // entry in the availableObjects list. We use the last index, so the list does not 
            // have to shuffle all other objects down.
            PooledObject pooledObject;
            int lastAvailableIndex = availableObjects.Count - 1;
            
            if(lastAvailableIndex >= 0)
            {
                // If the lastAvailableIndex points to a valid index in the availableObjects list, 
                // we have a PooledObject available for re-use. Set it as our pooledObject 
                // reference, remove it from the availableObjects list and re-activate the game 
                // object.
                pooledObject = availableObjects[lastAvailableIndex];
                availableObjects.RemoveAt(lastAvailableIndex);
                pooledObject.gameObject.SetActive(true);
            }
            else
            {
                // Else, the availableObjects list does not contain any PooledObject references, so 
                // we do not have one available. Send notification to the Debug.
                Log.InstantiatingPooledObjectNotification(this);

                try
                {
                    // Try to create a new instance using our pooledObjectPrefab; set it's parent 
                    // transform to the local transform to retain hierarchial organisation, and set 
                    // the pooledObject's objectPool reference to this ObjectPool, so it knows 
                    // where to return to when it needs to be recycled.
                    pooledObject = Instantiate<PooledObject>(pooledObjectPrefab);
                    pooledObject.transform.SetParent(transform, false);
                    pooledObject.objectPool = this;
                }
                catch(NullReferenceException nullReferenceException)
                {
                    // If the above throws a NullReferenceException, we do not have a valid 
                    // pooledObjectPrefab reference. Send an error to the Debug, create a default 
                    // game object, attach a default PooledObject, and set it to our pooledObject 
                    // reference.
                    Log.MissingPooledObjectError(this, nullReferenceException.Message);
                    GameObject newGameObject = new GameObject();
                    pooledObject = newGameObject.AddComponent<PooledObject>();
                }
            }
            
            // Return the resulting pooledObject reference.
            return pooledObject;
                 }

        /// <summary>Overwrites the current 
        /// <see cref="DataStructures.ObjectPool.pooledObjectPrefab"/>.</summary>
        /// <returns><c>true</c>, if there was an existing prefab, and this method did have to 
        /// overwrite it; <c>false</c> otherwise.</returns>
        /// <param name="pooledObjectPrefab">The new 
        /// <see cref="DataStructures.ObjectPool.pooledObjectPrefab"/>.</param>
        /// <remarks>This method will overwrite any existing 
        /// <see cref="DataStructures.ObjectPool.pooledObjectPrefab"/>. If the intention is to 
        /// provide a <see cref="DataStructures.ObjectPool.pooledObjectPrefab"/> reference in the 
        /// absence of one, consider using <see cref="DataStructures.ObjectPool.SetPrefab"/>,
        /// instead.</remarks>
        public bool OverwritePrefab(PooledObject pooledObjectPrefab)
        {
            // Create a boolean to determine if there was a previous pooledObjectPrefab, and set
            // the new pooledObjectPrefab.
            bool isOverwriting = (this.pooledObjectPrefab != null);
            this.pooledObjectPrefab = pooledObjectPrefab;

            // Return the boolean to determine if a previous pooledObjectPrefab was overwritten.
            return isOverwriting;
        }

        /// <summary>Sets the <see cref="DataStructures.ObjectPool.pooledObjectPrefab"/> reference, 
        /// if there was previously no reference. If there was a previous 
        /// <see cref="DataStructures.ObjectPool.pooledObjectPrefab"/> reference, this method will 
        /// not overwrite it.</summary>
        /// <returns><c>true</c>, if there was no existing prefab, <c>false</c> otherwise.</returns>
        /// <param name="pooledObjectPrefab">The new 
        /// <see cref="DataStructures.ObjectPool.pooledObjectPrefab"/>.</param>
        /// <remarks>This method will not replace any pre-existing 
        /// <see cref="DataStructures.ObjectPool.pooledObjectPrefab"/> reference. If the intention 
        /// is to replace this reference, regardless of it's previous state, consider using 
        /// <see cref="DataStructures.ObjectPool.OverwritePrefab"/>.</remarks>
        public bool SetPrefab(PooledObject pooledObjectPrefab)
        {
            if(this.pooledObjectPrefab == null)
            {
                // If there is currently no pooledObjectPrefab, set the new pooledObjectPrefab, 
                // and return true.
                this.pooledObjectPrefab = pooledObjectPrefab;
                return true;
            }
            else
            {
                // Else, there is already a pooledObjectPrefab, return false.
                return false;
            }
        }
        #endregion

        #region Static Functionality
        /// <summary>Creates an <see cref="DataStructures.ObjectPool"/> for use with the passed-in 
        /// <see cref="DataStructures.PooledObject"/>. If this method is called from within the 
        /// editor, attempts will be made to find a suitable 
        /// <see cref="DataStructures.PooledObject"/> that has already been created.</summary>
        /// <remarks>This method will try to find a suitable 
        /// <see cref="DataStructures.ObjectPool"/> prior to creating one, using 
        /// <see cref="UnityEngine.GameObject.Find"/> and the <see cref="GameObject.name"/> 
        /// associated with the passed-in <see cref="DataStructures.PooledObject"/>. This 
        /// functionality will only run in the editor, as it is too expensive to be viable in a 
        /// finished product.</remarks>
        /// <returns>The resulting <see cref="DataStructures.ObjectPool"/> for use in pooling 
        /// instances of the passed-in <see cref="DataStructures.PooledObject"/>.</returns>
        /// <param name="pooledObjectPrefab">The <see cref="DataStructures.PooledObject"/> for 
        /// which we intend to get a working <see cref="DataStructures.ObjectPool"/>.</param>
        public static ObjectPool GetPool(PooledObject pooledObjectPrefab)
        {
            // Create references to the resulting ObjectPool, and the GameObject containing it.
            GameObject objectPoolGameObject;
            ObjectPool objectPool;

            #if UNITY_EDITOR
            // If we are in the editor, we can afford to perform a GameObject.Find, using the name 
            // standard we use when we create ObjectPools to attempt to locate a suitable 
            // ObjectPool.
            objectPoolGameObject = GameObject.Find(pooledObjectPrefab.name + Labels.objectPool);

            if(objectPoolGameObject != null)
            {
                // If objectPoolGameObject is not null, GameObject.Find was able to locate a 
                // suitable ObjectPool. Attempt to retrieve the ObjectPool component attached to 
                // the objectPoolGameObject.
                objectPool = objectPoolGameObject.GetComponent<ObjectPool>();

                if(objectPool != null)
                {
                    // If objectPool is not null, our objectPoolGameObject was holding an 
                    // ObjectPool; return it for use.
                    return objectPool;
                }
            }
            #endif

            // If we are not in the editor, or we could not find a usable ObjectPool, create a new 
            // GameObject with the name standard of "[pooledObjectPrefab.name] pool", set it to 
            // persist through level loading, and add an ObjectPool component, which we will also 
            // set as our objectPool reference. Set the prefab of our new objectPool as the 
            // passed-in pooledObjectPrefab.
            objectPoolGameObject = new GameObject(pooledObjectPrefab.name + Labels.objectPool);
            DontDestroyOnLoad(objectPoolGameObject);
            objectPool = objectPoolGameObject.AddComponent<ObjectPool>();
            objectPool.pooledObjectPrefab = pooledObjectPrefab;

            // Return our derived objectPool.
            return objectPool;
        }
        #endregion
    }
}

namespace DataStructures.Utility
{
    /// <summary>This class holds debug functionality for the DataStructures namespace.</summary>
    public static partial class DataStructuresDebug
    {
        #region Notification Messages
        private const string instantiatingPooledObjectNotification = "Notification: Instantiating " 
            + "object, as availableObjects is empty.";
        #endregion
        #region Warning Messages
        private const string creatingObjectPoolWithNoPrefabWarning = "Warning: Creating an " 
            + "ObjectPool without a valid prefab reference. ObjectPool will not be able to " 
            + "instantiate copies of it's PoolableObject.";
        #endregion
        #region Error Messages
        private const string missingPooledObjectError = "Error: Can not instantiate object, " 
            + "as there is no pooledObjectPrefab.";
        #endregion

        #region Notifications
        /// <summary>Provides debug concerning the need for an 
        /// <see cref="DataStructures.ObjectPool"/> to instantiate a new instance of it's 
        /// <see cref="DataStructures.ObjectPool.pooledObjectPrefab"/>.</summary>
        /// <param name="objectPool">The <see cref="DataStructures.ObjectPool"/>.</param>
        /// <remarks>An <see cref="DataStructures.ObjectPool"/> will need a reference to a 
        /// prefabbed <see cref="DataStructures.PoolableObject"/> in order to be able to 
        /// instantiate copies</remarks>
        public static void InstantiatingPooledObjectNotification(ObjectPool objectPool)
        {
            Debug.Log(instantiatingPooledObjectNotification, objectPool);
        }
        #endregion

        #region Warnings
        /// <summary>Provides debug concerning creation of an ObjectPool without reference to a 
        /// PoolableObject f</summary>
        /// <param name="objectPool">The <see cref="DataStructures.ObjectPool"/>.</param>
        /// <remarks>An <see cref="DataStructures.ObjectPool"/> will need a reference to a 
        /// prefabbed <see cref="DataStructures.PoolableObject"/> in order to be able to 
        /// instantiate copies</remarks>
        public static void CreatingObjectPoolWithNoPrefabWarning(ObjectPool objectPool)
        {
            Debug.Log(creatingObjectPoolWithNoPrefabWarning, objectPool);
        }
        #endregion

        #region Errors
        /// <summary>Provides debug concerning attempts to instantiate an instance of a nulled 
        /// pooledObjectPrefab associated with an ObjectPool</summary>
        /// <param name="objectPool">The <see cref="DataStructures.ObjectPool"/>.</param>
        /// <param name="errorMessage">The thrown error message.</param>
        /// <remarks><see cref="DataStructures.ObjectPool"/> will attempt to instantiate a new 
        /// <see cref="DataStructures.PoolableObject"/>, when a new 
        /// <see cref="DataStructures.PoolableObject"/> is requested, but the availableObjects list 
        /// is empty. This error alerts the user when that pooledObjectPrefab has not been setup.
        /// </remarks>
        public static void MissingPooledObjectError(ObjectPool objectPool, 
            string errorMessage = "no thrown error")
        {
            Debug.Log(missingPooledObjectError, objectPool);
            Debug.LogError(errorMessage, objectPool);
        }
        #endregion
    }

    /// <summary>This class holds string formats for the DataStructures namespace.</summary>
    public static partial class DataStructuresStringFormats
    {
        /// <summary>Formats the string output of an ObjectPool with the intention of being 
        /// formatted with the pooledObjectPrefab name and the availableObjects count.</summary>
        /// <summary>String output prefix for a PooledObject.</summary>
        public const string objectPool = "ObjectPool (";
        /// <summary>String output middle for a PooledObject with a valid pooledObjectPrefab, with 
        /// the intention of being formatted with the pooledObjectPrefab name and the 
        /// availableObjects count.</summary>
        public const string pooledObjectPrefab = "{0} : {1}";
        /// <summary>String output middle for a PooledObject without a valid pooledObjectPrefab, 
        /// with the intention of being formatted with the availableObjects count.</summary>
        public const string noPooledObjectPrefab = "NULL PREFAB : {0}";
        /// <summary>String output suffix for a PooledObject.</summary>
        public const string available = " available)";
        /// <summary>Formats the string output of an ObjectPool with a valid pooledObjectPrefab, 
        /// with the intention of being formatted with the pooledObjectPrefab name and the 
        /// availableOjects count.</summary>
        public const string objectPoolWithPrefab = objectPool + pooledObjectPrefab + available;
        /// <summary>Formats the string output of an ObjectPool without a valid pooledObjectPrefab, 
        /// with the intention of being formatted with the availableOjects count.</summary>
        public const string objectPoolWithoutPrefab = objectPool + noPooledObjectPrefab + available;
    }

    /// <summary>Used to store strings used for labelling buttons, "Prepare Change" descriptions 
    /// and game objects.</summary>
    public static class ObjectPoolLabels
    {
        #if UNITY_EDITOR
        #endif

        /// <summary>The suffix used to append to the end of the name of the game object containing 
        /// object pools. The complete name should be "[objectPrefab.name] pool".</summary>
        public const string objectPool = " Pool";
    }
}