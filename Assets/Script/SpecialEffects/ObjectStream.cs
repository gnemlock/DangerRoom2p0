/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/object-pools/ ---
 */

using UnityEngine;

namespace SpecialEffects
{
    using Generic;
    
    public class ObjectStream : MonoBehaviour
    {
        public BasicRigidObject[] objectPrefabs;
        public FloatRange spawnTimerRange;
        public FloatRange scaleRange;
        public FloatRange velocityRange;
        public FloatRange angularVelocityRange;
        public float baseVelocity;
        public Material sharedMaterial;

        private float timeSinceLastSpawn;
        private float currentSpawnDelay;

        private void FixedUpdate()
        {
            timeSinceLastSpawn += Time.fixedDeltaTime;

            if(timeSinceLastSpawn >= currentSpawnDelay)
            {
                timeSinceLastSpawn -= currentSpawnDelay;
                currentSpawnDelay = spawnTimerRange.randomRange;
                
                SpawnObject();
            }
        }

        void SpawnObject()
        {
            BasicRigidObject spawnedObjectPrefab 
                = objectPrefabs[Random.Range(0, objectPrefabs.Length)];
            BasicRigidObject spawnedObject 
                = spawnedObjectPrefab.GetPooledInstance<BasicRigidObject>();
            
            spawnedObject.transform.localPosition = transform.position;
            spawnedObject.transform.localScale = Vector3.one * scaleRange.randomRange;
            spawnedObject.transform.rotation = Random.rotation;
            spawnedObject.rigidbody.velocity = (transform.up * baseVelocity)
                + (Random.onUnitSphere * velocityRange.randomRange);
            spawnedObject.rigidbody.angularVelocity 
                = Random.onUnitSphere * angularVelocityRange.randomRange;
            
            spawnedObject.SetMaterial(sharedMaterial);
        }
    }
}