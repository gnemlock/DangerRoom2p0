/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/rounded-cube/ ---
 */

using UnityEngine;

namespace Drawing.Meshes
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class RoundedCube : MonoBehaviour
    {
        public int width, height, depth;
        
        private Mesh mesh;
        public Vector3[] vertices;
        
        private void Awake()
        {
            Generate();
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

        private void Generate()
        {
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            mesh.name = "Procedural Cube";
            
            GenerateVertices();
            GenerateTriangles();
        }
        
        private int GenerateBottomFace(int[] triangles, int triangleIndex, int ringLength)
        {
            int vertexIndex = 1;
            int vertexMinimum = ringLength - 2;
            int vertexMiddle = vertices.Length - (width - 1) * (depth - 1);
            int vertexMaximum;
            int vertexTop;
            
            triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, ringLength - 1, 
                vertexMiddle, 0, 1);

            for(int x = 1; x < width - 1; x++, vertexIndex++, vertexMiddle++)
            {
                triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexMiddle, 
                    vertexMiddle + 1, vertexIndex, vertexIndex + 1);
            }

            triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexMiddle, 
                vertexIndex + 2, vertexIndex, vertexIndex + 1);

            vertexMiddle -= width - 2;
            vertexMaximum = vertexIndex + 2;

            for(int z = 1; z < depth - 1; z++, vertexMinimum--, vertexMiddle++, vertexMaximum++)
            {
                triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexMinimum, 
                    vertexMiddle + width - 1, vertexMinimum + 1, vertexMiddle);

                for(int x = 1; x < width - 1; x++, vertexMiddle++)
                {
                    triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, 
                        vertexMiddle + width - 1, vertexMiddle + width, vertexMiddle, 
                        vertexMiddle + 1);
                }

                triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, 
                    vertexMiddle + width - 1, vertexMaximum + 1, vertexMiddle, vertexMaximum);
            }

            vertexTop = vertexMinimum - 1;

            triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexTop + 1, 
                vertexTop, vertexTop + 2, vertexMiddle);

            for(int x = 1; x < width - 1; x++, vertexTop--, vertexMiddle++)
            {
                triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexTop, 
                    vertexTop - 1, vertexMiddle, vertexMiddle + 1);
            }

            triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexTop, 
                vertexTop - 1, vertexMiddle, vertexTop - 2);

            return triangleIndex;
        }
        
        private int GenerateTopFace(int[] triangles, int triangleIndex, int ringLength)
        {
            int vertexIndex = ringLength * height;
            int vertexMinimum = ringLength * (height + 1) - 1;
            int vertexMiddle = vertexMinimum + 1;
            int vertexMaximum;
            int vertexTop;

            for(int x = 0; x < width - 1; x++, vertexIndex++)
            {
                triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexIndex, 
                    vertexIndex + 1, vertexIndex + ringLength - 1, vertexIndex + ringLength);
            }
            
            vertexMaximum = vertexIndex + 2;

            triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexIndex, 
                vertexIndex + 1, vertexIndex + ringLength - 1, vertexIndex + 2);
            
            for(int z = 1; z < depth - 1; z++, vertexMinimum--, vertexMiddle++, vertexMaximum++)
            {
                triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexMinimum, 
                    vertexMiddle, vertexMinimum - 1, vertexMiddle + width - 1);
            
                for(int x = 1; x < width - 1; x++, vertexMiddle++)
                {
                    triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexMiddle, 
                        vertexMiddle + 1, vertexMiddle + width - 1, vertexMiddle + width);
                }
            
                triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexMiddle, 
                    vertexMaximum, vertexMiddle + width - 1, vertexMaximum + 1);
            }
            
           vertexTop = vertexMinimum - 2;
            
            triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexMinimum, 
                vertexMiddle, vertexMinimum - 1, vertexMinimum - 2);
            
            for(int x = 1; x < width - 1; x++, vertexTop--, vertexMiddle++)
            {
                triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexMiddle, 
                    vertexMiddle + 1, vertexTop, vertexTop - 1);
            }
            
            triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexMiddle, 
                vertexTop - 2, vertexTop, vertexTop - 1);

            return triangleIndex;
        }
        
        private void GenerateTriangles()
        {
            int quadCount = (width * height + width * depth + height * depth) * 2;
            int ringLength = (width + depth) * 2;
            int[] triangles = new int[quadCount * 6];
            int triangleIndex = 0, vertexIndex = 0;
            
            for(int y = 0; y < height; y++, vertexIndex++)
            {
            
                for(int quadIndex = 0; quadIndex < ringLength - 1; quadIndex++, vertexIndex++)
                {
                    triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexIndex, 
                        vertexIndex + 1, vertexIndex + ringLength, vertexIndex + ringLength + 1);
                }
            
                triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexIndex, 
                    vertexIndex - ringLength + 1, vertexIndex + ringLength, vertexIndex + 1);
            }
            
            triangleIndex = GenerateTopFace(triangles, triangleIndex, ringLength);
            triangleIndex = GenerateBottomFace(triangles, triangleIndex, ringLength);
            
            mesh.triangles = triangles;
        }
        
        private void GenerateVertices()
        {
            int cornerVerticesCount = 8;
            int edgeVerticesCount = (width + depth + height - 3) * 4;
            int faceVerticesCount = ((width - 1) * (height - 1) + (width - 1) * (depth - 1)
                + (height - 1) * (depth - 1)) * 2;
            int vertexIndex = 0;

            vertices = new Vector3[cornerVerticesCount + edgeVerticesCount + faceVerticesCount];

            for(int y = 0; y <= height; y++)
            {
                for(int x = 0; x <= width; x++)
                {
                    vertices[vertexIndex++] = new Vector3(x, y, 0);
                }

                for(int z = 1; z <= depth; z++)
                {
                    vertices[vertexIndex++] = new Vector3(width, y, z);
                }

                for(int x = width - 1; x >= 0; x--)
                {
                    vertices[vertexIndex++] = new Vector3(x, y, depth);
                }

                for(int z = depth - 1; z > 0; z--)
                {
                    vertices[vertexIndex++] = new Vector3(0, y, z);
                }
            }

            for(int z = 1; z < depth; z++)
            {
                for(int x = 1; x < width; x++)
                {
                    vertices[vertexIndex++] = new Vector3(x, height, z);
                }
            }

            for(int z = 1; z < depth; z++)
            {
                for(int x = 1; x < width; x++)
                {
                    vertices[vertexIndex++] = new Vector3(x, 0, z);
                }
            }

            mesh.vertices = vertices;
        }
        
        private static int GenerateQuadTriangles(int[] triangles, int index, int bottomLeftIndex, 
            int bottomRightIndex, int topLeftIndex, int topRightIndex)
        {
            triangles[index] = bottomLeftIndex;
            triangles[index + 1] = triangles[index + 4] = topLeftIndex;
            triangles[index + 2] = triangles[index + 3] = bottomRightIndex;
            triangles[index + 5] = topRightIndex;
            
            return index + 6;
        }
    }
}