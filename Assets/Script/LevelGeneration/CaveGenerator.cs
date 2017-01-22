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
    using Tooltips = Utility.Tooltips;
    #endif
    
    public class CaveGenerator : MonoBehaviour
    {
        /// <summary>The current seed this <see cref="CaveGenerator"/> is using for 
        /// <see cref="System.Random"/> generation. This value will be changed using 
        /// <see cref="AssignRandomSeed"/> if <see cref="usingRandomSeed"/> is set to 
        /// <c>true.</c></summary>
        [Tooltip(Tooltips.seed)] public string seed;
        
        /// <summary>If <c>true</c>, this <see cref="CaveGenerator"/> will create a seed using 
        /// <see cref="AssignRandomSeed"/>.</summary>
        [Tooltip(Tooltips.usingRandomSeed)][SerializeField] private bool usingRandomSeed;
        /// <summary>The width of the map, in grid dimension.</summary>
        [Tooltip(Tooltips.width)][SerializeField] private uint width = 40;
        /// <summary>The height of the map, in grid dimension.</summary>
        [Tooltip(Tooltips.height)][SerializeField] private uint height = 40;
        /// <summary>The level of smoothing applied to the generated map.</summary>
        [Tooltip(Tooltips.smoothLevel)][SerializeField] private uint smoothLevel = 5;
        //TODO: This could be better suited as a component-based "smooth filter", as we can provide a variety of effects.
        /// <summary>During each pass, <see cref="SmoothMap"/> will use this value to determine 
        /// if each region on the map should be filled in. If the number of neighbouring tiles 
        /// exceeds this value, the region will be filled in. If the number of neighbouring 
        /// tiles is less than this number, the tile will be made empty. If the numbers match, 
        /// the tile will be left as it is.</summary>
        [Tooltip(Tooltips.wallBleed)][SerializeField][Range(0, 8)] private int wallBleed = 4;
        /// <summary>The approximate percent of grid to fill.</summary>
        [Tooltip(Tooltips.fillPercent)][SerializeField][Range(0, 100)] private int fillPercent = 40;
        
        //TODO: This would be more efficient as a bool array; ensure the project does not use integers outside of 1 and 0, first.
        /// <summary>The generated map, represented as a grid of integers.</summary>
        private int[,] map;
        
        /// <summary>This method will be called just before the first Update call.</summary>
        public void Start()
        {
            GenerateMap();
        }
        
        /// <summary>Assigns a value to <see cref="seed"/> based off the current 
        /// <see cref="Time.time"/> value.</summary>
        private void AssignRandomSeed()
        {
            // Set seed to the string value of the current time since level load, in seconds.
            // This will change so quickly that it imitates a random number to the extent we need.
            seed = Time.time.ToString();
        }
        
        /// <summary>Randomly generates the map using <see cref="RandomFillMap"/>, before calling 
        /// <see cref="SmoothMap"/> as specified by <see cref="smoothLevel"/>.</summary>
        public void GenerateMap()
        {
            // Initialise and fill the map.
            map = new int[width, height];
            RandomFillMap();
            
            // Apply a smooth pass over the map, as many times as specified.
            for (int i = 0; i < smoothLevel; i++)
            {
                SmoothMap();
            }
        }
        
        /// <summary>Passes over the map and applys smoothing, filling and emptying regions in the 
        /// map based off neighbouring regions and <see cref="wallBleed"/>.</summary>
        void SmoothMap()
        {
            // For every x coordinate on the map,
            for (int x = 0; x < width; x++)
            {
                // For every y coordinate on the map,
                for (int y = 0; y < height; y++)
                {
                    // Count the neighbouring walls.
                    int neighbouringWallCount = GetNeighbouringWallCount(x, y);
                    
                    if(neighbouringWallCount > wallBleed)
                    {
                        // If the amount of walls exceeds the specified wallBleed,
                        // Fill in the region.
                        map[x, y] = 1;
                    }
                    else if(neighbouringWallCount < wallBleed)
                    {
                        // Else, if the amount of walls is less than the specified wallBleed,
                        // Empty the region.
                        map[x, y] = 0;
                    }
                }
            }
        }
        
        /// <summary>Counts the number of walls neighbouring a specified region in 
        /// <see cref="map"/>.</summary>
        /// <returns>The number of walls neighbouring the specified region.</returns>
        /// <param name="xPosition">X position of the intended region, in <see cref="map"/>.</param>
        /// <param name="yPosition">Y position of the intended region, in <see cref="map"/>.</param>
        private int GetNeighbouringWallCount(int xPosition, int yPosition)
        {
            // We start off assuming there are no walls.
            int wallCount = 0;
            
            // For the x coordinates to the left, right and center of our xPosition,
            for (int x = (xPosition - 1); x <= (xPosition + 1); x++)
            {
                // For the y coordinates to the left, right and center of our yPosition,
                for (int y = (yPosition - 1); y <= (yPosition + 1); y++)
                {
                    if(x >= 0 && x < width && y >= 0 && y < height)
                    {
                        // If the x,y coordinate is still on the map,
                        // Add the value of that region to our count.
                        // Remember, a wall will return 1, while empty space will return 0.
                        wallCount += map[x, y];
                    }
                    else
                    {
                        // Else, the x,y coordinate has moved off the map,
                        // We must be hitting a wall.
                        wallCount++;
                    }
                }
            }
            
            // Since the above for statements will inevitbly count the intended region as one of 
            // it's own neighbours, we will balance the count by deducting it, before returning it.
            wallCount -= map[xPosition, yPosition];
            return wallCount;
        }
        
        /// <summary>Randomly fills <see cref="map"/> using <see cref="System.Random"/> and 
        /// <see cref="seed"/>. Will attempt to adhere to <see cref="fillPercent"/>.</summary>
        /// <remarks>This method will roughly fill <see cref="map"/> to the percent specified 
        /// by <see cref="fillPercent"/>. That said, there is still room for error, due to the 
        /// nature of integers and the potential for fractions, given a real percentage value of 
        /// the map. Furthermore, we will automatically allocate walls to the perimeter of the map, 
        /// which will not be counted towards the percentage.
        void RandomFillMap()
        {
            if (usingRandomSeed)
            {
                // If we have specified the use of a random seed,
                // Generate a random seed.
                AssignRandomSeed();
            }
            
            // Create a random number generator using the hashcode of our seed. We use a 
            // hashcode to determine the integer value of our seed, which is a string, itself.
            System.Random numberGenerator = new System.Random(seed.GetHashCode());
            
            // For every x coordinate on the map, 
            for (int x = 0; x < width; x++)
            {
                // For every y coordinate on the map, 
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 || x == (width - 1) || y == 0 || y == (height - 1))
                    {
                        // If the current coordinate is on the perimeter of the map, 
                        // Automatically allocate a wall to this coordinate.
                        map[x, y] = 1;
                    }
                    else
                    {
                        // Else, we are inside the walls of the map, 
                        // Generate a random number from our random number generator and 
                        // compare it with our desired fill percent, to randomly allocate 
                        // walls to the rest of the map.
                        map[x, y] = ((numberGenerator.Next (0, 100) < fillPercent) ? 1 : 0);
                    }
                }
            }
        }

        #if UNITY_EDITOR
        /// <summary>This method will be called when we draw gizmos.</summary>
        /// <remarks>This method has been marked for exclusive use with the editor, and thus 
        /// will not be available in the published game.</remarks>
        void OnDrawGizmos()
        {
            if (map != null)
            {
                // If we currently have a map to work with, 
                // For each x coordinate on the map,
                for (int x = 0; x < width; x++)
                {
                    // For each y coordinate on the map,
                    for (int y = 0; y < height; y++)
                    {
                        // Ensure the current gizmo drawing colour matches the intended colour of 
                        // the region we are currently looking at.
                        Gizmos.color = (map [x, y] == 1) ? Color.black : Color.white;
                        
                        //TODO:Clean this up. new Vector each time creates garbage. We should be able to create one vector and increment its x,y values.
                        //TODO:We can probably also add a parameter to increase the size of each region.
                        // Ensure we determine the correct physical position of the region we 
                        // are looking at.
                        Vector3 position 
                            = new Vector3((-width / 2f + x + 0.5f), (-height / 2f + y + 0.5f), 0f);
                        
                        // Draw a cube at the specified position, in the specified colour, to a 
                        // scale of one.
                        Gizmos.DrawCube(position, Vector3.one);
                    }
                }
            }
        }
        #endif
    }
}

namespace LevelGeneration.Utility
{
    /// <summary>This class holds the tooltips for variables serialized to the inspector.</summary>
    public static class Tooltips
    {
        #if UNITY_EDITOR
        public const string seed = "What seed should the generator use to build the map " +
            "from? If usingRandomSeed is enabled, this will be replaced with a random seed, " +
            "anyway.";
        public const string usingRandomSeed = "Is the map using a random seed?";
        public const string width = "How many grid units wide should the map be?";
        public const string height = "How many grid units high should the map be?";
        public const string smoothLevel = "How many times should we apply the smoothing filter?";
        public const string wallBleed = "When we apply the smoothing filter, how should we weigh " +
            "neighbouring tiles? Tiles with less neighbours will be made empty, while tiles with " +
            "more neighbours will be filled in.";
        public const string fillPercent = "Roughly what percent of the grid should be filled in?";
        #endif
    }
    
    /// <summary>This class provides additional functionality to the editor.</summary>
    [CustomEditor(typeof(CaveGenerator))] public class CaveGeneratorEditor : Editor
    {
        #if UNITY_EDITOR
        /// <summary>This method will be called to draw the inspector interface for the target 
        /// class.</summary>
        public override void OnInspectorGUI()
        {
            // Explicitly reference the target class as a CaveGenerator, so we have CaveGenerator 
            // specific access.
            CaveGenerator instance = (CaveGenerator)target;
            
            // Draw the default inspector, so we still have the default interface.
            DrawDefaultInspector();
            
            if(GUILayout.Button("Generate Map"))
            {
                // Add a button called "Generate Map", and when it is pressed, 
                // generate a map from the current settings.
                instance.GenerateMap();
            }  
        }
        #endif
    }
}