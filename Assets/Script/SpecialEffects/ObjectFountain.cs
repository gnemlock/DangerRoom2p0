/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/object-pools/ ---
 */

using UnityEngine;

namespace SpecialEffects
{
    using Generic;
    using Tags = Utility.SpecialEffectsTags;
    using Log = Utility.SpecialEffectsDebug;
        
    public class ObjectFountain : MonoBehaviour
    {
        public int streamCount;
        public ObjectStream streamPrefab;
        public float radius;
        public float tiltAngle;
        public Material[] streamMaterials;
        
        [SerializeField] private GameObject killZone;
        
        private void Awake()
        {
            for(int i = 0; i < streamCount; i++)
            {
                CreateSpawner(i);
            }
            
            if(killZone == null || !killZone.CompareTag(Tags.killZone))
            {
                Log.NoKillZoneDetected(this);
            }
        }
        
        private void CreateSpawner(int index)
        {
            Transform rotater = new GameObject("Rotater").transform;
            rotater.SetParent(transform, false);
            rotater.localRotation = Quaternion.Euler(0f, (index * (360f / streamCount)), 0f);
            
            ObjectStream objectStream = Instantiate<ObjectStream>(streamPrefab);
            objectStream.transform.SetParent(rotater, false);
            objectStream.transform.localPosition = new Vector3(0f, 0f, radius);
            objectStream.transform.localRotation = Quaternion.Euler(tiltAngle, 0f, 0f);
            
            objectStream.sharedMaterial = streamMaterials[index % streamMaterials.Length];
        }
    }
}

namespace SpecialEffects.Utility
{
    /// <summary>This class holds debug functionality for the UserInterface namespace.</summary>
    public static partial class SpecialEffectsDebug
    {
        private const string noKillZoneDetected = "Warning: No object kill zone detected. Ensure "
            + "object spawner contains a game object tagged as " + SpecialEffectsTags.killZone
            + " to ensure spawned objects are correctly disposed.";
        
        public static void NoKillZoneDetected(MonoBehaviour script)
        {
            Debug.Log(noKillZoneDetected, script);
        }
    }
    
    public static partial class SpecialEffectsTags
    {
        public const string killZone = "Kill Zone";
    }
}