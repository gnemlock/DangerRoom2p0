/* 
 * Created by Matthew F Keating with help from tutorials created by Sebastian Lague
 *  ---------- https://www.youtube.com/channel/UCmtyQOKKmrMVaKuRXz02jbQ ----------
 */

using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LevelGeneration
{
    #if UNITY_EDITOR
    using Tooltips = Utility.MeshGeneratorTooltips;
    #endif

    public class MeshGenerator : MonoBehaviour 
    {
        public Grid grid;
        
        List<Vector3> vertices;
        List<int> triangles;
        Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();
        
        List<List<int>> outlines = new List<List<int>>();
        HashSet<int> checkedVertices = new HashSet<int>();
        
        public MeshFilter walls;
        
        #if UNITY_EDITOR
        [SerializeField] private bool showGizmos = true;
        #endif
        
        public void GenerateMesh(int [,] map, float squareSize)
        {
            outlines.Clear();
            checkedVertices.Clear();
            triangleDictionary.Clear();
            
            grid = new Grid(map, squareSize);
            vertices = new List<Vector3>();
            triangles = new List<int>();
            
            for(int x = 0; x < grid.squares.GetLength(0); x++)
            {
                for(int y = 0; y < grid.squares.GetLength(1); y++)
                {
                      TriangulateSquare(grid.squares[x, y]);
                }
            }
            
            Mesh mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            
            CreateWallMesh();
        }
        
        void CreateWallMesh()
        {
            CalculateMeshOutlines();
            
            List<Vector3> wallVertices = new List<Vector3>();
            List<int> wallTriangles = new List<int>();
            float wallHeight = 5.0f;
            Mesh wallMesh = new Mesh();
            
            foreach(List<int> outline in outlines)
            {
                for(int i = 0; i < outline.Count - 1; i++)
                {
                    int startIndex = wallVertices.Count;
                    wallVertices.Add(vertices[outline[i]]); // left
                    wallVertices.Add(vertices[outline[i + 1]]); // right
                    wallVertices.Add(vertices[outline[i]] - Vector3.back * wallHeight); // bottom left
                    wallVertices.Add(vertices[outline[i + 1]] - Vector3.back * wallHeight); // bottom right
                    
                    wallTriangles.Add(startIndex);
                    wallTriangles.Add(startIndex + 2);
                    wallTriangles.Add(startIndex + 3);
                    wallTriangles.Add(startIndex + 3);
                    wallTriangles.Add(startIndex + 1);
                    wallTriangles.Add(startIndex);
                }
            }
            wallMesh.vertices = wallVertices.ToArray();
            wallMesh.triangles = wallTriangles.ToArray();
            
            walls.mesh = wallMesh;
        }
        
        void TriangulateSquare(Square square)
        {
            switch(square.configuration)
            {
                /*   
                 *   8 - - - 4
                 *   |       |    1  2  3  4
                 *   |       |    1  2  4  8
                 *   1 - - - 2
                 */
                
                // 0 points selected.
                case 0:
                    break;
                    
                // 1 point selected.
                case 1:
                    MeshFromPoints(square.centerLeft, square.centerBottom, square.bottomLeft);
                    break;
                case 2:
                    MeshFromPoints(square.bottomRight, square.centerBottom, square.centerRight);
                    break;
                case 4:
                    MeshFromPoints(square.topRight, square.centerRight, square.centerTop);
                    break;
                case 8:
                    MeshFromPoints(square.topLeft, square.centerTop, square.centerLeft);
                    break;
                    
                // 2 points selected
                case 3:
                    MeshFromPoints(square.centerRight, square.bottomRight, square.bottomLeft, 
                        square.centerLeft);
                    break;
                case 5:
                    MeshFromPoints(square.centerTop, square.topRight, square.centerRight, 
                        square.centerBottom, square.bottomLeft, square.centerLeft);
                    break;
                case 6:
                    MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, 
                        square.centerBottom);
                    break;
                case 9:
                    MeshFromPoints(square.topLeft, square.centerTop, square.centerBottom, 
                        square.bottomLeft);
                    break;
                case 10:
                    MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, 
                        square.bottomRight, square.centerBottom, square.centerLeft);
                    break;
                case 12:
                    MeshFromPoints(square.topLeft, square.topRight, square.centerRight, 
                        square.centerLeft);
                    break;
                    
                // 3 points selected
                case 7:
                    MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, 
                        square.bottomLeft, square.centerLeft);
                    break;
                case 11:
                    MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, 
                        square.bottomRight, square.bottomLeft);
                    break;
                case 13:
                    MeshFromPoints(square.topLeft, square.topRight, square.centerRight, 
                        square.centerBottom, square.bottomLeft);
                    break;
                case 14:
                    MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, 
                        square.centerBottom, square.centerLeft);
                    break;
                    
                // All 4 points selected
                case 15:
                    MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, 
                        square.bottomLeft);
                    checkedVertices.Add(square.topLeft.vertexIndex);
                    checkedVertices.Add(square.topRight.vertexIndex);
                    checkedVertices.Add(square.bottomRight.vertexIndex);
                    checkedVertices.Add(square.bottomLeft.vertexIndex);
                    break;
            }   
        }
        
        void MeshFromPoints(params Node[] points)
        {
            AssignVerticies(points);
            
            for(int i = 2; i < points.Length; i++)
            {
                CreateTriangle(points[0], points[i - 1], points[i]);
            }
        }
        
        void AssignVerticies(Node[] points)
        {
            for(int i = 0; i < points.Length; i++)
            {
                if(points[i].vertexIndex == -1)
                {
                    points[i].vertexIndex = vertices.Count;
                    vertices.Add(points[i].position);
                }
            }
        } 
        
        void CreateTriangle(Node a, Node b, Node c)
        {
            triangles.Add(a.vertexIndex);
            triangles.Add(b.vertexIndex);
            triangles.Add(c.vertexIndex);
            
            Triangle newTriangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
            AddTriangleToDictionary(a.vertexIndex, newTriangle);
            AddTriangleToDictionary(b.vertexIndex, newTriangle);
            AddTriangleToDictionary(c.vertexIndex, newTriangle);
        }
        
        int GetConnectedOutlineVertex(int vertexIndex)
        {
            List<Triangle> trianglesContainingIndex = triangleDictionary[vertexIndex];
            
            for(int i = 0; i < trianglesContainingIndex.Count; i++)
            {
                Triangle triangle = trianglesContainingIndex[i];
                
                for(int j = 0; j < 3; j++)
                {
                    int vertexB = triangle[j];
                    
                    if(vertexB != vertexIndex && !checkedVertices.Contains(vertexB) 
                        && IsOutlineEdge(vertexIndex, vertexB))
                    {
                        return vertexB;
                    }
                }
            }
            
            return -1;
        }
        
        bool IsOutlineEdge(int vertexA, int vertexB)
        {
            List<Triangle> trianglesA = triangleDictionary[vertexA];
            int sharedTriangleCount = 0;
            
            for(int i = 0; i < trianglesA.Count; i++)
            {
                if(trianglesA[i].Contains(vertexB))
                {
                    sharedTriangleCount++;
                    
                    if(sharedTriangleCount > 1)
                    {
                        break;
                    }
                }
            }
            
            return (sharedTriangleCount == 1);
        }
        
        void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle)
        {
            if(triangleDictionary.ContainsKey(vertexIndexKey))
            {
                triangleDictionary[vertexIndexKey].Add(triangle);
            }
            else
            {
                List<Triangle> newTriangleList = new List<Triangle>();
                newTriangleList.Add(triangle);
                triangleDictionary.Add(vertexIndexKey, newTriangleList);
            }
        }
        
        void CalculateMeshOutlines()
        {
            for(int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++)
            {
                if(!checkedVertices.Contains(vertexIndex))
                {
                    int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
                    
                    if(newOutlineVertex != -1)
                    {
                        checkedVertices.Add(vertexIndex);
                        
                        List<int> newOutline = new List<int>();
                        
                        newOutline.Add(vertexIndex);
                        outlines.Add(newOutline);
                        FollowOutline(newOutlineVertex, outlines.Count - 1);
                        
                        // re add the first vertex to connect the outline
                        outlines[outlines.Count - 1].Add(vertexIndex);
                    }
                }
            }
        }
        
        void FollowOutline(int vertexIndex, int outlineIndex)
        {
            outlines[outlineIndex].Add(vertexIndex);
            checkedVertices.Add(vertexIndex);
            
            int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);
            
            if(nextVertexIndex != -1)
            {
                FollowOutline(nextVertexIndex, outlineIndex);
            }
        }
        
        public void OnDrawGizmos()
        {
            if(showGizmos && grid != null)
            {
                for(int x = 0; x < grid.squares.GetLength(0); x++)
                {
                    for(int y = 0; y < grid.squares.GetLength(1); y++)
                    {
                        Gizmos.color = grid.squares[x, y].topLeft.active ? Color.blue : Color.white;
                        Gizmos.DrawCube(grid.squares[x, y].topLeft.position, Vector3.one * 0.4f);

                        Gizmos.color = grid.squares[x, y].topRight.active ? Color.blue : Color.white;
                        Gizmos.DrawCube(grid.squares[x, y].topRight.position, Vector3.one * 0.4f);
                        
                        Gizmos.color = grid.squares[x, y].bottomLeft.active ? Color.blue : Color.white;
                        Gizmos.DrawCube(grid.squares[x, y].bottomLeft.position, Vector3.one * 0.4f);
                        
                        Gizmos.color = grid.squares[x, y].bottomRight.active ? Color.blue : Color.white;
                        Gizmos.DrawCube(grid.squares[x, y].bottomRight.position, Vector3.one * 0.4f);
                        
                        Gizmos.color = Color.gray;
                        Gizmos.DrawCube(grid.squares[x, y].centerTop.position, Vector3.one * 0.15f);
                        Gizmos.DrawCube(grid.squares[x, y].centerRight.position, Vector3.one * 0.15f);
                        Gizmos.DrawCube(grid.squares[x, y].centerBottom.position, Vector3.one * 0.15f);
                        Gizmos.DrawCube(grid.squares[x, y].centerLeft.position, Vector3.one * 0.15f);
                    }
                }
            }
        }
               
        struct Triangle
        {
            public int vertexIndexA;
            public int vertexIndexB;
            public int vertexIndexC;
            int[] vertices;
            
            public Triangle(int vertexIndexA, int vertexIndexB, int vertexIndexC)
            {
                this.vertexIndexA = vertexIndexA;
                this.vertexIndexB = vertexIndexB;
                this.vertexIndexC = vertexIndexC;
                
                vertices = new int[3];
                vertices[0] = vertexIndexA;
                vertices[1] = vertexIndexB;
                vertices[2] = vertexIndexC;
            }
            
            public int this[int i]
            {
                get
                {
                    return vertices[i];
                }
            }
            
            public bool Contains(int vertexIndex)
            {
                return (vertexIndex == vertexIndexA || vertexIndex == vertexIndexB 
                    || vertexIndex == vertexIndexC);
            }
        }
        
        public class Grid
        {
            public Square [,] squares;
            
            public Grid(int [,] map, float squareSize)
            {
                int xDimension = map.GetLength(0);
                int yDimension = map.GetLength(1);
                float mapWidth = xDimension * squareSize;
                float mapHeight = yDimension * squareSize;
                
                ControlNode[,] controlNodes = new ControlNode[xDimension, yDimension];
                
                for(int x = 0; x < xDimension; x++)
                {
                    for(int y = 0; y < yDimension; y++)
                    {
                        
                        Vector3 position 
                            = new Vector3(-mapWidth / 2.0f + x * squareSize + squareSize/2.0f, 
                            -mapHeight/2 + y * squareSize + squareSize/2, 0);
                        
                        controlNodes[x,y] = new ControlNode(position, map[x,y] == 1, squareSize);
                    }
                }
                
                squares = new Square[xDimension - 1, yDimension - 1];
                
                for(int x = 0; x < xDimension - 1; x++)
                {
                    for(int y = 0; y < yDimension - 1; y++)
                    {
                        squares[x,y] = new Square(controlNodes[x, y+1], controlNodes[x + 1, y + 1], 
                            controlNodes[x, y], controlNodes[x + 1, y]);
                    }
                }
            }
        }
        
        public class Square
        {
            public ControlNode topLeft, topRight, bottomLeft, bottomRight;
            public Node centerTop, centerLeft, centerRight, centerBottom;
            public int configuration;
            
            public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomLeft, 
                ControlNode bottomRight)
            {
                this.topLeft = topLeft;
                this.topRight = topRight;
                this.bottomLeft = bottomLeft;
                this.bottomRight = bottomRight;
                
                centerTop = topLeft.rightNode;
                centerLeft = bottomLeft.topNode;
                centerRight = bottomRight.topNode;
                centerBottom = bottomLeft.rightNode;
                
                if(topLeft.active)
                {
                    configuration += 8;
                }
                
                if(topRight.active)
                {
                    configuration += 4;
                }
                
                if(bottomRight.active)
                {
                    configuration += 2;
                }
                
                if(bottomLeft.active)
                {
                    configuration += 1;
                }
            }
        }
        
        public class Node
        {
            public Vector3 position;
            public int vertexIndex = -1;
            
            public Node(Vector3 position)
            {
                this.position = position;
            }
        }

        public class ControlNode : Node
        {
            public bool active;
            public Node topNode;
            public Node rightNode;
            
            public ControlNode(Vector3 position, bool active, float squareSize) : base(position)
            {
                this.active = active;
                topNode = new Node(position + (Vector3.up * (squareSize / 2.0f)));
                rightNode = new Node(position + (Vector3.right * (squareSize / 2.0f)));
            }
        }
    }
}

namespace LevelGeneration.Utility
{
    /// <summary>This class holds the tooltips for variables serialized to the inspector.</summary>
    public static class MeshGeneratorTooltips
    {
        #if UNITY_EDITOR

        #endif
    }

    /// <summary>This class provides additional functionality to the editor.</summary>
    [CustomEditor(typeof(MeshGenerator))] public class MeshGeneratorInspector : Editor
    {
        #if UNITY_EDITOR
        /// <summary>This method will be called to draw the inspector interface for the target 
        /// class.</summary>
        public override void OnInspectorGUI()
        {
            // Explicitly reference the target class as a CaveGenerator, so we have CaveGenerator 
            // specific access.
            MeshGenerator meshGenerator = (MeshGenerator)target;

            // Draw the default inspector, so we still have the default interface.
            DrawDefaultInspector();
        }
        #endif
    }
}