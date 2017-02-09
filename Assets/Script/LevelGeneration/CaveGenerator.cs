/* 
 * Created by Matthew F Keating with help from tutorials created by Sebastian Lague
 *  ---------- https://www.youtube.com/channel/UCmtyQOKKmrMVaKuRXz02jbQ ----------
 */

using UnityEngine;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LevelGeneration
{
    #if UNITY_EDITOR
    using Tooltips = Utility.CaveGeneratorTooltips;
    #endif
    
    //[RequireComponent(typeof(MeshGenerator))]
    public class CaveGenerator : MonoBehaviour
    {
        /// <summary>The current seed this <see cref="CaveGenerator"/> is using for 
        /// <see cref="System.Random"/> generation. This value will be changed using 
        /// <see cref="AssignRandomSeed"/> if <see cref="usingRandomSeed"/> is set to 
        /// <c>true.</c></summary>
        [Tooltip(Tooltips.seed)] public string seed;
        
        /// <summary>If <c>true</c>, this <see cref="CaveGenerator"/> will create a seed using 
        /// <see cref="AssignRandomSeed"/>.</summary>
        [Tooltip(Tooltips.usingRandomSeed)][SerializeField] private bool usingRandomSeed = true;
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
        
        public int wallThreshold = 50;
        public int roomThreshold = 50;
        
        #if UNITY_EDITOR
        [SerializeField] private bool showGizmos = true;
        #endif
        
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
            //TODO:Ammend summary with inclusion of mesh generation
            // Initialise and fill the map.
            map = new int[width, height];
            FillMap();
            
            // Apply a smooth pass over the map, as many times as specified.
            for (int i = 0; i < smoothLevel; i++)
            {
                SmoothMap();
            }
            
            ProcessMap();
            
            int borderSize = 5;
            int[,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2];
            
            for(int x = 0; x < borderedMap.GetLength(0); x++)
            {
                // For every y coordinate on the map,
                for(int y = 0; y < borderedMap.GetLength(1); y++)
                {
                    if(x >= borderSize && x < (width + borderSize)
                       && y >= borderSize && y < (height + borderSize))
                    {
                        borderedMap[x, y] = map[x - borderSize, y - borderSize];
                    }
                    else
                    {
                        borderedMap[x, y] = 1;
                    }
                }
            }
            
            MeshGenerator meshGenerator = GetComponent<MeshGenerator>();
                
            if(meshGenerator != null)
            {
                meshGenerator.GenerateMesh(borderedMap, 1);
            }
        }
        
        List<Coordinate> GetRegionTiles(int startX, int startY)
        {
            List<Coordinate> tiles = new List<Coordinate>();
            
            int[,] mapFlags = new int[width, height];
            int tileType = map[startX, startY];
            
            Queue<Coordinate> queue = new Queue<Coordinate>();
            
            queue.Enqueue(new Coordinate(startX, startY));
            mapFlags[startX, startY] = 1;
            
            while(queue.Count > 0)
            {
                Coordinate tile = queue.Dequeue();
                
                tiles.Add(tile);
                
                for(int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                {
                    for(int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                    {
                        if(IsInMapRange(x, y) && (y == tile.tileY || x == tile.tileX))
                        {
                            if(mapFlags[x, y] == 0 && map[x, y] == tileType)
                            {
                                mapFlags[x, y] = 1;
                                queue.Enqueue(new Coordinate(x, y));
                            }
                        }
                    }
                }
            }
            
            return tiles;
        }
        
        List<List<Coordinate>> GetRegions(int tileType)
        {
            List<List<Coordinate>> regions = new List<List<Coordinate>>();
            int[,] mapFlags = new int[width, height];
            
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    if(mapFlags[x, y] == 0 && map[x, y] == tileType)
                    {
                        List<Coordinate> newRegion = GetRegionTiles(x, y);
                        regions.Add(newRegion);
                        
                        foreach(Coordinate tile in newRegion)
                        {
                            mapFlags[tile.tileX, tile.tileY] = 1;
                        }
                    }
                }
            }
            
            return regions;
        }
        
        void ProcessMap()
        {
            List<List<Coordinate>> wallRegions = GetRegions(1);
            
            foreach(List<Coordinate> wallRegion in wallRegions)
            {
                if(wallRegion.Count < wallThreshold)
                {
                    foreach(Coordinate tile in wallRegion)
                    {
                        map[tile.tileX, tile.tileY] = 0;
                    }
                }
            }
            
            List<List<Coordinate>> roomRegions = GetRegions(0);
            List<Room> remainingRooms = new List<Room>();
            
            foreach(List<Coordinate> roomRegion in roomRegions)
            {
                if(roomRegion.Count < roomThreshold)
                {
                    foreach(Coordinate tile in roomRegion)
                    {
                        map[tile.tileX, tile.tileY] = 1;
                    }
                }
                else
                {
                    remainingRooms.Add(new Room(roomRegion, map));
                }
            }
            
            remainingRooms.Sort();
            
            remainingRooms[0].isMainRoom = true;
            remainingRooms[0].accessableFromMainRoom = true;
            
            ConnectClosestRooms(remainingRooms);
        }
                        
        void ConnectClosestRooms(List<Room> allRooms, bool forceConnectionToMainRoom = false)
        {
            List<Room> roomListA = new List<Room>();
            List<Room> roomListB = new List<Room>();
            
            if(forceConnectionToMainRoom)
            {
                foreach(Room room in allRooms)
                {
                    if(room.accessableFromMainRoom)
                    {
                        roomListB.Add(room);
                    }
                    else
                    {
                        roomListA.Add(room);
                    }
                }
            }
            else
            {
                roomListA = allRooms;
                roomListB = allRooms;
            }
            
            int bestDistance = 0;
            Coordinate bestTileA = new Coordinate();
            Coordinate bestTileB = new Coordinate();
            Room bestRoomA = new Room();
            Room bestRoomB = new Room();
            bool possibleConnectionFound = false;
            
            foreach(Room roomA in roomListA)
            {
                if(!forceConnectionToMainRoom)
                {
                    possibleConnectionFound = false;
                    
                    if(roomA.connectedRooms.Count > 0)
                    {
                        continue;
                    }
                }
                
                foreach(Room roomB in roomListB)
                {
                    if(roomA == roomB || roomA.IsConnected(roomB))
                    {
                        continue;
                    }
                    
                    
                    for(int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
                    {
                        for(int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
                        {
                            Coordinate tileA = roomA.edgeTiles[tileIndexA];
                            Coordinate tileB = roomB.edgeTiles[tileIndexB];
                            
                            int distanceBetweenRooms 
                                = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2)
                                + Mathf.Pow(tileA.tileY - tileB.tileY, 2));
                            
                            if(distanceBetweenRooms < bestDistance || !possibleConnectionFound)
                            {
                                bestDistance = distanceBetweenRooms;
                                possibleConnectionFound = true;
                                bestTileA = tileA;
                                bestTileB = tileB;
                                bestRoomA = roomA;
                                bestRoomB = roomB;
                            }
                        }
                    }
                }
                
                if(possibleConnectionFound && !forceConnectionToMainRoom)
                {
                    CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
                }
            }
            
            if(possibleConnectionFound && forceConnectionToMainRoom)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
                ConnectClosestRooms(allRooms, true);
            }
            
            if(!forceConnectionToMainRoom)
            {
                ConnectClosestRooms(allRooms, true);
            }
        }
        
        void CreatePassage(Room roomA, Room roomB, Coordinate tileA, Coordinate tileB)
        {
            Room.ConnectRooms(roomA, roomB);
            //TODO: We do not appear to be drawing lines between rooms, yet.
            Debug.DrawLine(CoordinateToWorldPoint(tileA), CoordinateToWorldPoint(tileB), 
                Color.green, 100.0f);
            List<Coordinate> line = GetLine(tileA, tileB);
            
            foreach(Coordinate coordinate in line)
            {
                DrawCircle(coordinate, 1);
            }
        }
        
        void DrawCircle(Coordinate coordinate, int radius)
        {
            for(int x = -radius; x <= radius; x++)
            {
                for(int y = -radius; y <= radius; y++)
                {
                    if(x * x + y * y <= radius * radius)
                    {
                        int realX = coordinate.tileX + x;
                        int realY = coordinate.tileY + y;
                        
                        if(IsInMapRange(realX, realY))
                        {
                            map[realX, realY] = 0;
                        }
                    }
                }
            }
        }
        
        List<Coordinate> GetLine(Coordinate fromCoordinate, Coordinate toCoordinate)
        {
            List<Coordinate> line = new List<Coordinate>();
            
            bool inverted = false;
            
            int x = fromCoordinate.tileX;
            int y = fromCoordinate.tileY;
            
            int dx = toCoordinate.tileX - fromCoordinate.tileX;
            int dy = toCoordinate.tileY - fromCoordinate.tileY;
            
            int step = Math.Sign(dx);
            int gradientStep = Math.Sign(dy);
            
            int longest = Mathf.Abs(dx);
            int shortest = Mathf.Abs(dy);
            
            if(longest < shortest)
            {
                inverted = true;
                longest = Mathf.Abs(dy);
                shortest = Mathf.Abs(dx);
                
                step = Math.Sign(dy);
                gradientStep = Math.Sign(dx);
            }
            
            int gradientAccumulation = longest / 2;
            
            for(int i = 0; i < longest; i++)
            {
                line.Add(new Coordinate(x, y));
                
                if(inverted)
                {
                    y += step;
                }
                else
                {
                    x += step;
                }
                
                gradientAccumulation += shortest;
                
                if(gradientAccumulation >= longest)
                {
                    if(inverted)
                    {
                        x += gradientStep;
                    }
                    else
                    {
                        y += gradientStep;
                    }
                    
                    gradientAccumulation -= longest;
                }
            }
            
            return line;
        }
        
        Vector3 CoordinateToWorldPoint(Coordinate tile)
        {
            return new Vector3(-width / 2f + 0.5f + tile.tileX, -height / 2f + 0.5f + tile.tileY, -6.0f);
        }
        
        bool IsInMapRange(int x, int y)
        {
            return (x >= 0 && x < width && y >= 0 && y < height);
        }
        
        struct Coordinate
        {
            public int tileX;
            public int tileY;

            public Coordinate(int tileX, int tileY)
            {
                this.tileX = tileX;
                this.tileY = tileY;
            }
        }
        
        class Room : IComparable<Room>
        {
            public List<Coordinate> tiles;
            public List<Coordinate> edgeTiles;
            public List<Room> connectedRooms;
            public int roomSize;
            public bool accessableFromMainRoom;
            public bool isMainRoom;
            
            public Room()
            {
            }
            
            public Room(List<Coordinate> tiles, int[,] map)
            {
                this.tiles = tiles;
                roomSize = tiles.Count;
                connectedRooms = new List<Room>();
                edgeTiles = new List<Coordinate>();
                
                foreach(Coordinate tile in tiles)
                {
                    for(int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                    {
                        for(int y = tile.tileY - 1; y <= tile.tileY; y++)
                        {
                            if(x == tile.tileX || y == tile.tileY)
                            {
                                if(map[x,y] == 1)
                                {
                                    edgeTiles.Add(tile);
                                }
                            }
                        }
                    }
                }
            }
            
            public static void ConnectRooms(Room roomA, Room roomB)
            {
                if(roomA.accessableFromMainRoom)
                {
                    roomB.SetAccessibleFromMainRoom();
                }
                else if(roomB.accessableFromMainRoom)
                {
                    roomA.SetAccessibleFromMainRoom();
                }
                roomA.connectedRooms.Add(roomB);
                roomB.connectedRooms.Add(roomA);
            }

            public bool IsConnected(Room otherRoom)
            {
                return connectedRooms.Contains(otherRoom);
            }
            
            public int CompareTo(Room otherRoom)
            {
                return otherRoom.roomSize.CompareTo(roomSize);
            }
            
            public void SetAccessibleFromMainRoom()
            {
                if(accessableFromMainRoom)
                {
                    accessableFromMainRoom = true;
                    
                    foreach(Room room in connectedRooms)
                    {
                        room.accessableFromMainRoom = true;
                    }
                }
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
                    if(IsInMapRange(x, y))
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
        void FillMap()
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
        
        //TODO:We are commenting out the below OnDrawGizmos method so it does not clash with the MeshGenerator.OnDrawGizmos method. Find a more suitable way to allow the user to swap between "gizmo stages" via the inspector. May need a custom property drawer.
        #if UNITY_EDITOR
        /// <summary>This method will be called when we draw gizmos.</summary>
        /// <remarks>This method has been marked for exclusive use with the editor, and thus 
        /// will not be available in the published game.</remarks>
        void OnDrawGizmos()
        {
            if (showGizmos && map != null)
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
    public static class CaveGeneratorTooltips
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
    [CustomEditor(typeof(CaveGenerator))] public class CaveGeneratorInspector : Editor
    {
        #if UNITY_EDITOR
        /// <summary>This method will be called to draw the inspector interface for the target 
        /// class.</summary>
        public override void OnInspectorGUI()
        {
            // Explicitly reference the target class as a CaveGenerator, so we have CaveGenerator 
            // specific access.
            CaveGenerator caveGenerator = target as CaveGenerator;
            
            // Draw the default inspector, so we still have the default interface.
            DrawDefaultInspector();
            
            if(GUILayout.Button("Generate Map"))
            {
                // Add a button called "Generate Map", and when it is pressed, 
                // generate a map from the current settings.
                caveGenerator.GenerateMap();
            }  
        }
        #endif
    }
}