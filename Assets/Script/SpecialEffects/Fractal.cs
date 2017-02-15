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
        
        public float spawnTimer = 0.5f;
        public int localDepth = 0;
        public float childScale = 0.5f;
        public Mesh mesh;
        public Material[] materials;
        
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
                //
                InitialiseMaterials();
            }
            
            gameObject.AddComponent<MeshFilter>().mesh = mesh;
            gameObject.AddComponent<MeshRenderer>().material = materials[localDepth];
            
            if(localDepth < maxDepth)
            {
                StartCoroutine(CreateChildren());
            }
        }
        
        //TODO: The parent fractal should create a child in the Vector3.down direction. further children should not.
        public IEnumerator CreateChildren()
        {
            for(int i = 0; i < childDirections.Length; i++)
            {
                yield return new WaitForSeconds(spawnTimer);
                new GameObject("Fractal Child").AddComponent<Fractal>().Initialise(this, i);
            }
        }
        
        public void Initialise(Fractal parent, int childIndex)
        {
            mesh = parent.mesh;
            materials = parent.materials;
            maxDepth = parent.maxDepth;
            localDepth = parent.localDepth + 1;
            transform.parent = parent.transform;
            childScale = parent.childScale;
            transform.localScale = Vector3.one * childScale;
            transform.localPosition = childDirections[childIndex] * (0.5f + 0.5f * childScale);
            transform.localRotation = childOrientations[childIndex];
        }
        
        private void InitialiseMaterials()
        {
            materials = new Material[maxDepth + 1];
            
            for(int i = 0; i <= maxDepth; i++)
            {
                materials[i].color = Color.Lerp(Color.white, Color.yellow, ((float)i / maxDepth));
            }
        }
    }
}