/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/cube-sphere/ ---
 */

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Drawing.Meshes
{
    using Labels = Utility.MeshesLabels;

    #if UNITY_EDITOR
    using Tooltips = Utility.CubedSphereTooltips;
    using Colours = Utility.CubedSphereColours;
    using Dimensions = Utility.CubedSphereDimensions;
    #endif

    /// <summary>Represents an irregular cube mesh with rounded edges.</summary>
    /// <remarks>The mesh is created using three sub meshes. As such, the 
    /// <see cref="UnityEngine.MeshRenderer"/> needs to employ three 
    /// <see cref="UnityEngine.Material"/> references.</remarks>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class CubedSphere : MonoBehaviour, IMeshGeneratable
    {
        /// <summary>The size of the grids making up the sphere. The sphere is made up of 6 grids, 
        /// much like a cube.</summary>
        [Tooltip(Tooltips.gridSize)][Range(1, 255)] public int gridSize = 1;
        public float radius = 1.0f;

        /// <summary>Has this <see cref="Drawing.Meshes.RoundedCube"/> been generated?</summary>
        public bool generated { get; private set; }

        /// <summary>Holds reference to the generated mesh.</summary>
        private Mesh mesh;
        /// <summary>The set of vertices used in <see cref="Drawing.Meshes.RoundedCube.mesh"/>.
        /// </summary>
        private Vector3[] vertices;
        /// <summary>The set of normals used in <see cref="Drawing.Meshes.RoundedCube.mesh"/>.
        /// </summary>
        private Vector3[] normals;
        /// <summary>The set of UV coordinates used in 
        /// <see cref="Drawing.Meshes.RoundedCube.mesh"/>.</summary>
        /// <remarks>We are storing our UV coordinates as a set of 
        /// <see cref="UnityEngine.Color32"/> values for convenience when passing the UV 
        /// coordinates to the custom shader.</remarks>
        private Color32[] cubeUV;

        /// <summary>This method will be called when this instance of 
        /// <see cref="Drawing.Meshes.RoundedCube"/> is loaded.</summary>
        private void Awake()
        {
            // Generate the rounded cube mesh, along with it's colliders.
            Generate();
        }

        #if UNITY_EDITOR
        /// <summary>This method is called when the editor draws the gizmos for an instance of 
        /// <see cref="Drawing.Meshes.RoundedCube"/>. THIS METHOD IS EDITOR ONLY.</summary>
        private void OnDrawGizmos()
        {
            if(vertices != null)
            {
                for(int i = 0; i < vertices.Length; i++)
                {
                    // For each vertice in the vertices array, draw a sphere to mark the vertex, 
                    // and draw a ray to mark the normal. Ensure vertex positions are transformed 
                    // to world space, so they draw correctly if the rounded cube is moved.
                    Gizmos.color = Colours.vertexMarkerColour;
                    Gizmos.DrawSphere(transform.TransformPoint(vertices[i]), 
                        Dimensions.vertexMarkerRadius);
                    Gizmos.color = Colours.normalRayColour;
                    Gizmos.DrawRay(transform.TransformPoint(vertices[i]), normals[i]);
                }
            }
        }
        #endif

        /// <summary>Generates the mesh, including vertices, normals and colliders.</summary>
        public void Generate()
        {
            // Create the mesh, and add it to the mesh filter.
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            mesh.name = Labels.cubedSphereName;

            // Generate the vertices and triangles for the mesh, and the colliders.
            GenerateVertices();
            GenerateTriangles();
            GenerateColliders();
            Vector3[] sor;

            // Mark the mesh as generated.
            generated = true;
        }

        private int GenerateBottomFace(int[] triangles, int triangleIndex, int ringLength)
        {
            int vertexIndex = 1;
            int vertexMinimum = ringLength - 2;
            int vertexMiddle = vertices.Length - (gridSize - 1) * (gridSize - 1);
            int vertexMaximum;
            int vertexTop;

            triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, ringLength - 1, 
                vertexMiddle, 0, 1);

            for(int x = 1; x < gridSize - 1; x++, vertexIndex++, vertexMiddle++)
            {
                triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexMiddle, 
                    vertexMiddle + 1, vertexIndex, vertexIndex + 1);
            }

            triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexMiddle, 
                vertexIndex + 2, vertexIndex, vertexIndex + 1);

            vertexMiddle -= gridSize - 2;
            vertexMaximum = vertexIndex + 2;

            for(int z = 1; z < gridSize - 1; z++, vertexMinimum--, vertexMiddle++, vertexMaximum++)
            {
                triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexMinimum, 
                    vertexMiddle + gridSize - 1, vertexMinimum + 1, vertexMiddle);

                for(int x = 1; x < gridSize - 1; x++, vertexMiddle++)
                {
                    triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, 
                        vertexMiddle + gridSize - 1, vertexMiddle + gridSize, vertexMiddle, 
                        vertexMiddle + 1);
                }

                triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, 
                    vertexMiddle + gridSize - 1, vertexMaximum + 1, vertexMiddle, vertexMaximum);
            }

            vertexTop = vertexMinimum - 1;

            triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexTop + 1, 
                vertexTop, vertexTop + 2, vertexMiddle);

            for(int x = 1; x < gridSize - 1; x++, vertexTop--, vertexMiddle++)
            {
                triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexTop, 
                    vertexTop - 1, vertexMiddle, vertexMiddle + 1);
            }

            triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexTop, 
                vertexTop - 1, vertexMiddle, vertexTop - 2);

            return triangleIndex;
        }

        private void GenerateColliders()
        {
            if(generated)
            {
                RemoveColliders();
            }

            gameObject.AddComponent<SphereCollider>();
        }

        private int GenerateTopFace(int[] triangles, int triangleIndex, int ringLength)
        {
            int vertexIndex = ringLength * gridSize;
            int vertexMinimum = ringLength * (gridSize + 1) - 1;
            int vertexMiddle = vertexMinimum + 1;
            int vertexMaximum;
            int vertexTop;

            for(int x = 0; x < gridSize - 1; x++, vertexIndex++)
            {
                triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexIndex, 
                    vertexIndex + 1, vertexIndex + ringLength - 1, vertexIndex + ringLength);
            }

            vertexMaximum = vertexIndex + 2;

            triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexIndex, 
                vertexIndex + 1, vertexIndex + ringLength - 1, vertexIndex + 2);

            for(int z = 1; z < gridSize - 1; z++, vertexMinimum--, vertexMiddle++, vertexMaximum++)
            {
                triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexMinimum, 
                    vertexMiddle, vertexMinimum - 1, vertexMiddle + gridSize - 1);

                for(int x = 1; x < gridSize - 1; x++, vertexMiddle++)
                {
                    triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexMiddle, 
                        vertexMiddle + 1, vertexMiddle + gridSize - 1, vertexMiddle + gridSize);
                }

                triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexMiddle, 
                    vertexMaximum, vertexMiddle + gridSize - 1, vertexMaximum + 1);
            }

            vertexTop = vertexMinimum - 2;

            triangleIndex = GenerateQuadTriangles(triangles, triangleIndex, vertexMinimum, 
                vertexMiddle, vertexMinimum - 1, vertexMinimum - 2);

            for(int x = 1; x < gridSize - 1; x++, vertexTop--, vertexMiddle++)
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
            int[] trianglesX = new int[(gridSize * gridSize) * 12];
            int[] trianglesY = new int[(gridSize * gridSize) * 12];
            int[] trianglesZ = new int[(gridSize * gridSize) * 12];
            int ringLength = (gridSize + gridSize) * 2;
            int triangleXIndex = 0, triangleYIndex = 0, triangleZIndex = 0, vertexIndex = 0;

            for(int y = 0; y < gridSize; y++, vertexIndex++)
            {
                for(int quadIndex = 0; quadIndex < gridSize; quadIndex++, vertexIndex++)
                {
                    triangleZIndex = GenerateQuadTriangles(trianglesZ, triangleZIndex, vertexIndex, 
                        vertexIndex + 1, vertexIndex + ringLength, vertexIndex + ringLength + 1);
                }

                for(int quadIndex = 0; quadIndex < gridSize; quadIndex++, vertexIndex++)
                {
                    triangleXIndex = GenerateQuadTriangles(trianglesX, triangleXIndex, vertexIndex, 
                        vertexIndex + 1, vertexIndex + ringLength, vertexIndex + ringLength + 1);
                }

                for(int quadIndex = 0; quadIndex < gridSize; quadIndex++, vertexIndex++)
                {
                    triangleZIndex = GenerateQuadTriangles(trianglesZ, triangleZIndex, vertexIndex, 
                        vertexIndex + 1, vertexIndex + ringLength, vertexIndex + ringLength + 1);
                }

                for(int quadIndex = 0; quadIndex < gridSize - 1; quadIndex++, vertexIndex++)
                {
                    triangleXIndex = GenerateQuadTriangles(trianglesX, triangleXIndex, vertexIndex, 
                        vertexIndex + 1, vertexIndex + ringLength, vertexIndex + ringLength + 1);
                }

                triangleXIndex = GenerateQuadTriangles(trianglesX, triangleXIndex, vertexIndex, 
                    vertexIndex - ringLength + 1, vertexIndex + ringLength, vertexIndex + 1);
            }

            triangleYIndex = GenerateTopFace(trianglesY, triangleYIndex, ringLength);
            triangleYIndex = GenerateBottomFace(trianglesY, triangleYIndex, ringLength);

            mesh.subMeshCount = 3;
            mesh.SetTriangles(trianglesZ, 0);
            mesh.SetTriangles(trianglesX, 1);
            mesh.SetTriangles(trianglesY, 2);
        }

        private void GenerateVertex(int vertexIndex, int x, int y, int z)
        {
            Vector3 vertex = new Vector3(x, y, z) * 2.0f / gridSize - Vector3.one;
            normals[vertexIndex] = vertex.normalized;
            vertices[vertexIndex] = normals[vertexIndex] * radius;
            cubeUV[vertexIndex] = new Color32((byte)x, (byte)y, (byte)z, 0);
        }

        private void GenerateVertices()
        {
            int cornerVerticesCount = 8;
            int edgeVerticesCount = (gridSize + gridSize + gridSize - 3) * 4;
            int faceVerticesCount = ((gridSize - 1) * (gridSize - 1) + (gridSize - 1) * (gridSize - 1)
                + (gridSize - 1) * (gridSize - 1)) * 2;
            int vertexIndex = 0;

            vertices = new Vector3[cornerVerticesCount + edgeVerticesCount + faceVerticesCount];
            normals = new Vector3[vertices.Length];
            cubeUV = new Color32[vertices.Length];

            for(int y = 0; y <= gridSize; y++)
            {
                for(int x = 0; x <= gridSize; x++)
                {
                    GenerateVertex(vertexIndex++, x, y, 0);
                }

                for(int z = 1; z <= gridSize; z++)
                {
                    GenerateVertex(vertexIndex++, gridSize, y, z);
                }

                for(int x = gridSize - 1; x >= 0; x--)
                {
                    GenerateVertex(vertexIndex++, x, y, gridSize);
                }

                for(int z = gridSize - 1; z > 0; z--)
                {
                    GenerateVertex(vertexIndex++, 0, y, z);
                }
            }

            for(int z = 1; z < gridSize; z++)
            {
                for(int x = 1; x < gridSize; x++)
                {
                    GenerateVertex(vertexIndex++, x, gridSize, z);
                }
            }

            for(int z = 1; z < gridSize; z++)
            {
                for(int x = 1; x < gridSize; x++)
                {
                    GenerateVertex(vertexIndex++, x, 0, z);
                }
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.colors32 = cubeUV;
        }

        private void RemoveColliders()
        {
            SphereCollider sphereCollider = GetComponent<SphereCollider>();

            #if UNITY_EDITOR
            DestroyImmediate(sphereCollider);
            #else
            Destroy(sphereCollider);
            #endif
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

namespace Drawing.Meshes.Utility
{
    public static partial class MeshesLabels
    {
        /// <summary>Used to name the mesh associated with the 
        /// <see cref="Drawing.Meshes.CubedSphere"/>.</summary>
        public const string cubedSphereName = "Procedural Sphere";
    }
    public static class CubedSphereTooltips
    {
        #if UNITY_EDITOR
        public const string gridSize = "The size of the grids making up the six sides of the "
            + "sphere.";
        #endif
    }

    public static class CubedSphereColours
    {
        #if UNITY_EDITOR
        /// <summary>The colour used to draw vertex markers.</summary>
        public static Color vertexMarkerColour = Color.black;
        /// <summary>The colour used to draw normals.</summary>
        public static Color normalRayColour = Color.yellow;
        #endif
    }

    public static class CubedSphereDimensions
    {
        #if UNITY_EDITOR
        /// <summary>The radius used to draw vertex markers.</summary>
        public const float vertexMarkerRadius = 0.1f;
        #endif
    }

    [CustomEditor(typeof(CubedSphere))] public class CubedSphereEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            CubedSphere cubedSphere = target as CubedSphere;

            DrawDefaultInspector();

            if(GUILayout.Button("Generate Sphere"))
            {
                cubedSphere.Generate();
            }
        }
    }

}