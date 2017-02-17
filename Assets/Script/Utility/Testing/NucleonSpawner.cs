/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/frames-per-second/ ---
 */

using UnityEngine;

namespace Utility.Testing
{
    public class NucleonSpawner : MonoBehaviour 
    {
        public float timeBetweenSpawns;
        public float spawnDistance;
        public Nucleon[] nucleonPrefabs;
        
        private float timeSinceLastSpawn;
        [SerializeField] private float nucleonCount = 0;
        
        private void FixedUpdate()
        {
            timeSinceLastSpawn += Time.deltaTime;
            
            if(timeSinceLastSpawn >= timeBetweenSpawns)
            {
                timeSinceLastSpawn -= timeBetweenSpawns;
                SpawnNucleon();
            }
        }
        
        private void SpawnNucleon()
        {
            Nucleon nucleonPrefab = nucleonPrefabs[Random.Range(0, nucleonPrefabs.Length)];
            Nucleon nucleon = Instantiate<Nucleon>(nucleonPrefab);
            
            nucleon.transform.localPosition = Random.onUnitSphere * spawnDistance;
            nucleon.transform.parent = transform;
            nucleonCount++;
        }
    }
}
