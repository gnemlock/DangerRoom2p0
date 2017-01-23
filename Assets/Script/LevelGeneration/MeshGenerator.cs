/* 
 * Created by Matthew F Keating with help from tutorials created by Sebastian Lague
 *  ---------- https://www.youtube.com/channel/UCmtyQOKKmrMVaKuRXz02jbQ ----------
 */

using UnityEngine;

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
        
        #if UNITY_EDITOR
        [SerializeField] private bool showGizmos = true;
        #endif
        
        public void GenerateMesh(int [,] map, float squareSize)
        {
            grid = new Grid(map, squareSize);
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
                            controlNodes[x + 1, y], controlNodes[x, y]);
                    }
                }
            }
        }
        
        public class Square
        {
            public ControlNode topLeft, topRight, bottomLeft, bottomRight;
            public Node centerTop, centerLeft, centerRight, centerBottom;
            
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
            }
        }
        public class Node
        {
            public Vector3 position;
            public int index = -1;
            
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