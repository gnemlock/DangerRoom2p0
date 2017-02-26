/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/procedural-grid/ ---
 */

using UnityEngine;

namespace Drawing.Meshes
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Surface : MonoBehaviour
    {
        public int width, height;
        
        private Vector3[] vertices;
        private Mesh mesh;
        
        private void Awake()
        {
            Generate();
        }
        
        private void Generate()
        {
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            mesh.name = "Procedural Grid";
            vertices = new Vector3[(width + 1) * (height + 1)];
            Vector2[] uvMap = new Vector2[vertices.Length];
            Vector4[] tangents = new Vector4[vertices.Length];
            Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
            int[] triangles = new int[width * height * 6];
            
            for(int i = 0, x = 0; x <= width; x++)
            {
                for(int y = 0; y <= height; y++, i++)
                {
                    vertices[i] = new Vector3(x, y);
                    uvMap[i] = new Vector2((float)x / width, (float)y / height);
                    tangents[i] = tangent;
                }
            }
            
            for(int triangleIndex = 0, vertexIndex = 0, y = 0; y < height; y++, vertexIndex++)
            {
                for(int x = 0; x < width; x++, triangleIndex += 6, vertexIndex++)
                {
                    triangles[triangleIndex] = vertexIndex;
                    triangles[triangleIndex + 3] = triangles[triangleIndex + 2] = vertexIndex + 1;
                    triangles[triangleIndex + 4] = triangles[triangleIndex + 1] 
                        = vertexIndex + 1 + width;
                    triangles[triangleIndex + 5] = vertexIndex + 2 + width;
                }
            }
            //TODO:Why the hells is this drawing backwards?
            mesh.vertices = vertices; 
            mesh.uv = uvMap;
            mesh.triangles = triangles;
            mesh.tangents = tangents;
            mesh.RecalculateNormals();
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(vertices != null)
            {
                Gizmos.color = Color.black;
            
                for(int i = 0; i < vertices.Length; i++)
                {
                    Gizmos.DrawSphere(transform.TransformPoint(vertices[i]), 0.1f);
                }
            }
        }
        #endif
    }
}