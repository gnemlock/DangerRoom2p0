/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/constructing-a-fractal/ ---
 */

using UnityEngine;

namespace SpecialEffects
{
    using System.Collections;
    
    public class Fractal : MonoBehaviour 
    {
        //TODO: It would be a good idea to enforce a static max depth so all fractals can be made to work within local confinements
        public int maxDepth = 5;
        public float spawnProbability;
        public float maxSpawnTime = 0.5f;
        public float minSpawnTime = 0.1f;
        public float minRotationSpeed = 30f;
        public float maxRotationSpeed = -30f;
        public int localDepth = 0;
        public float childScale = 0.5f;
        public Mesh[] meshes;
        public Material material;
        
        private Material[,] materials;
        private float rotationSpeed;
        
        private static Vector3[] childDirections =
        {
            Vector3.up,
            Vector3.right,
            Vector3.left,
            Vector3.forward,
            Vector3.back
        };
        
        private static Quaternion[] childOrientations =
        {
            Quaternion.identity,
            Quaternion.Euler(0f, 0f, -90f),
            Quaternion.Euler(0f, 0f, 90f),
            Quaternion.Euler(90f, 0f, 0f),
            Quaternion.Euler(-90f, 0f, 0f)
        };
        
        private void Start()
        {
            if(materials == null)
            {       
                InitialiseMaterials();
            }
            
            rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
            

            gameObject.AddComponent<MeshFilter>().mesh = meshes[Random.Range(0, meshes.Length)];
            gameObject.AddComponent<MeshRenderer>().material 
                = materials[localDepth, Random.Range(0, 2)];
            
            if(localDepth < maxDepth)
            {
                StartCoroutine(CreateChildren());
            }
        }
        
        private void Update()
        {
            transform.Rotate(0f, rotationSpeed   * Time.deltaTime, 0f);
        }
        
        //TODO: The parent fractal should create a child in the Vector3.down direction. further children should not.
        public IEnumerator CreateChildren()
        {
            for(int i = 0; i < childDirections.Length; i++)
            {
                if(Random.value < spawnProbability)
                {
                    yield return new WaitForSeconds(Random.Range(0.1f, maxSpawnTime));
                    new GameObject("Fractal Child").AddComponent<Fractal>().Initialise(this, i);
                }
            }
        }
        
        public void Initialise(Fractal parent, int childIndex)
        {
            meshes = parent.meshes;
            materials = parent.materials;
            maxDepth = parent.maxDepth;
            localDepth = parent.localDepth + 1;
            transform.parent = parent.transform;
            childScale = parent.childScale;
            transform.localScale = Vector3.one * childScale;
            transform.localPosition = childDirections[childIndex] * (0.5f + 0.5f * childScale);
            transform.localRotation = childOrientations[childIndex];
            spawnProbability = parent.spawnProbability;
            minRotationSpeed = parent.minRotationSpeed;
            maxRotationSpeed = parent.maxRotationSpeed;
        }
        
        private void InitialiseMaterials()
        {
            materials = new Material[maxDepth + 1, 2];
            
            for(int i = 0; i <= maxDepth; i++)
            {
                float t = i / (maxDepth - 1f);
                t *= t;
                
                materials[i, 0] = new Material(material);
                materials[i, 0].color = Color.Lerp(Color.white, Color.yellow, t);
                materials[i, 1] = new Material(material);
                materials[i, 1].color = Color.Lerp(Color.white, Color.red, t);
            }
            
            materials[maxDepth, 0].color = Color.magenta;
            materials[maxDepth, 1].color = Color.red;
        }
    }
}