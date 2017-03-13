/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/rounded-cube/ ---
 */

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Drawing.Meshes
{
    using Labels = Utility.MeshesLabels;

    #if UNITY_EDITOR
    using Tooltips = Utility.RoundedCubeTooltips;
    using Colours = Utility.RoundedCubeColours;
    using Dimensions = Utility.RoundedCubeDimensions;
    #endif

    /// <summary>Represents an irregular cube mesh with rounded edges.</summary>
    /// <remarks>The mesh is created using three sub meshes. As such, the 
    /// <see cref="UnityEngine.MeshRenderer"/> needs to employ three 
    /// <see cref="UnityEngine.Material"/> references.</remarks>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class RoundedCube : MonoBehaviour, IMeshGeneratable
    {
        /// <summary>The x-dimension of the cube.</summary>
        [Tooltip(Tooltips.width)][Range(1, 255)] public int width = 1;
        /// <summary>The y-dimension of the cube.</summary>
        [Tooltip(Tooltips.height)][Range(1, 255)] public int height = 1;
        /// <summary>The z-dimension of the cube.</summary>
        [Tooltip(Tooltips.depth)][Range(1, 255)] public int depth = 1;
        /// <summary>The amount of curving applied to the edges of the cube.</summary>
        [Tooltip(Tooltips.curveWeight)][Range(1, 127)] public int curveWeight = 1;
        //TODO:If the width height and depth are equal, providing half the value as curveWeight gives you a sphere. This probably implies that curving should not be allowed to exceed half the smallest value.

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
            mesh.name = Labels.roundedCubeName;

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

        private void GenerateBoxCollider(float colliderWidth, float colliderHeight, 
            float colliderDepth)
        {
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(colliderWidth, colliderHeight, colliderDepth);
        }

        private void GenerateCapsuleCollider(int direction, float x, float y, float z)
        {
            CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.center = new Vector3(x, y, z);
            capsuleCollider.direction = direction;
            capsuleCollider.radius = curveWeight;
            capsuleCollider.height = capsuleCollider.center[direction] * 2.0f;
        }

        private void GenerateColliders()
        {
            if(generated)
            {
                RemoveColliders();
            }

            float curveBuffer = curveWeight * 2.0f;

            GenerateBoxCollider(width, height - curveBuffer, depth - curveBuffer);
            GenerateBoxCollider(width - curveBuffer, height, depth - curveBuffer);
            GenerateBoxCollider(width - curveBuffer, height - curveBuffer, depth);

            Vector3 minimumPosition = Vector3.one * curveWeight;
            Vector3 halfwayPosition = new Vector3(width, depth, height) * 0.5f;
            Vector3 maximumPosition = new Vector3(width, depth, height) - minimumPosition;

            GenerateCapsuleCollider(0, halfwayPosition.x, minimumPosition.y, minimumPosition.z);
            GenerateCapsuleCollider(0, halfwayPosition.x, minimumPosition.y, maximumPosition.z);
            GenerateCapsuleCollider(0, halfwayPosition.x, maximumPosition.y, minimumPosition.z);
            GenerateCapsuleCollider(0, halfwayPosition.x, maximumPosition.y, maximumPosition.z);

            GenerateCapsuleCollider(1, minimumPosition.x, halfwayPosition.y, minimumPosition.z);
            GenerateCapsuleCollider(1, minimumPosition.x, halfwayPosition.y, maximumPosition.z);
            GenerateCapsuleCollider(1, maximumPosition.x, halfwayPosition.y, minimumPosition.z);
            GenerateCapsuleCollider(1, maximumPosition.x, halfwayPosition.y, maximumPosition.z);

            GenerateCapsuleCollider(2, minimumPosition.x, minimumPosition.y, halfwayPosition.z);
            GenerateCapsuleCollider(2, minimumPosition.x, maximumPosition.y, halfwayPosition.z);
            GenerateCapsuleCollider(2, maximumPosition.x, minimumPosition.y, halfwayPosition.z);
            GenerateCapsuleCollider(2, maximumPosition.x, maximumPosition.y, halfwayPosition.z);
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
            int[] trianglesX = new int[(depth * height) * 12];
            int[] trianglesY = new int[(width * depth) * 12];
            int[] trianglesZ = new int[(width * height) * 12];
            int ringLength = (width + depth) * 2;
            int triangleXIndex = 0, triangleYIndex = 0, triangleZIndex = 0, vertexIndex = 0;

            for(int y = 0; y < height; y++, vertexIndex++)
            {
                for(int quadIndex = 0; quadIndex < width; quadIndex++, vertexIndex++)
                {
                    triangleZIndex = GenerateQuadTriangles(trianglesZ, triangleZIndex, vertexIndex, 
                        vertexIndex + 1, vertexIndex + ringLength, vertexIndex + ringLength + 1);
                }

                for(int quadIndex = 0; quadIndex < depth; quadIndex++, vertexIndex++)
                {
                    triangleXIndex = GenerateQuadTriangles(trianglesX, triangleXIndex, vertexIndex, 
                        vertexIndex + 1, vertexIndex + ringLength, vertexIndex + ringLength + 1);
                }

                for(int quadIndex = 0; quadIndex < width; quadIndex++, vertexIndex++)
                {
                    triangleZIndex = GenerateQuadTriangles(trianglesZ, triangleZIndex, vertexIndex, 
                        vertexIndex + 1, vertexIndex + ringLength, vertexIndex + ringLength + 1);
                }

                for(int quadIndex = 0; quadIndex < depth - 1; quadIndex++, vertexIndex++)
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
            Vector3 innerVertex = vertices[vertexIndex] = new Vector3(x, y, z);

            if(x < curveWeight)
            {
                innerVertex.x = curveWeight;
            }
            else if(x > width - curveWeight)
            {
                innerVertex.x = width - curveWeight;
            }

            if(y < curveWeight)
            {
                innerVertex.y = curveWeight;
            }
            else if(y > height - curveWeight)
            {
                innerVertex.y = height - curveWeight;
            }

            if(z < curveWeight)
            {
                innerVertex.z = curveWeight;
            }
            else if(z > depth - curveWeight)
            {
                innerVertex.z = depth - curveWeight;
            }

            normals[vertexIndex] = (vertices[vertexIndex] - innerVertex).normalized;
            vertices[vertexIndex] = innerVertex + normals[vertexIndex] * curveWeight;
            cubeUV[vertexIndex] = new Color32((byte)x, (byte)y, (byte)z, 0);
        }

        private void GenerateVertices()
        {
            int cornerVerticesCount = 8;
            int edgeVerticesCount = (width + depth + height - 3) * 4;
            int faceVerticesCount = ((width - 1) * (height - 1) + (width - 1) * (depth - 1)
                + (height - 1) * (depth - 1)) * 2;
            int vertexIndex = 0;

            vertices = new Vector3[cornerVerticesCount + edgeVerticesCount + faceVerticesCount];
            normals = new Vector3[vertices.Length];
            cubeUV = new Color32[vertices.Length];

            for(int y = 0; y <= height; y++)
            {
                for(int x = 0; x <= width; x++)
                {
                    GenerateVertex(vertexIndex++, x, y, 0);
                }

                for(int z = 1; z <= depth; z++)
                {
                    GenerateVertex(vertexIndex++, width, y, z);
                }

                for(int x = width - 1; x >= 0; x--)
                {
                    GenerateVertex(vertexIndex++, x, y, depth);
                }

                for(int z = depth - 1; z > 0; z--)
                {
                    GenerateVertex(vertexIndex++, 0, y, z);
                }
            }

            for(int z = 1; z < depth; z++)
            {
                for(int x = 1; x < width; x++)
                {
                    GenerateVertex(vertexIndex++, x, height, z);
                }
            }

            for(int z = 1; z < depth; z++)
            {
                for(int x = 1; x < width; x++)
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
            Collider[] colliders = gameObject.GetComponents<Collider>();
            Debug.Log("Removing " + colliders.Length + " items.");

            for(int i = 0; i < colliders.Length; i++)
            {
                #if UNITY_EDITOR
                DestroyImmediate(colliders[i]);
                #else
                Destroy(colliders[i]);
                #endif
            }
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
        /// <see cref="Drawing.Meshes.RoundedCube"/>.</summary>
        public const string roundedCubeName = "Procedural Cube";
    }
    public static class RoundedCubeTooltips
    {
        #if UNITY_EDITOR
        public const string width = "The x dimension of the rounded cube.";
        public const string height = "The y dimension of the rounded cube.";
        public const string depth = "The z dimension of the rounded cube.";
        public const string curveWeight = "The curve weight applied to the edges of the rounded " 
            + "cube";
        #endif
    }

    public static class RoundedCubeColours
    {
        #if UNITY_EDITOR
        /// <summary>The colour used to draw vertex markers.</summary>
        public static Color vertexMarkerColour = Color.black;
        /// <summary>The colour used to draw normals.</summary>
        public static Color normalRayColour = Color.yellow;
        #endif
    }

    public static class RoundedCubeDimensions
    {
        #if UNITY_EDITOR
        /// <summary>The radius used to draw vertex markers.</summary>
        public const float vertexMarkerRadius = 0.1f;
        #endif
    }

    [CustomEditor(typeof(RoundedCube))] public class RoundedCubeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            RoundedCube roundedCube = target as RoundedCube;

            DrawDefaultInspector();

            if(GUILayout.Button("Generate Cube"))
            {
                roundedCube.Generate();
            }
        }
    }

}