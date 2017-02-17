/* 
 * Created by Matthew F Keating
 */

using UnityEngine;

namespace LevelGeneration
{
    using Log = Utility.LevelGenerationDebug;
    using StringFormat = Utility.LevelGenerationStringFormats;
    
    /// <summary>Represents a grid coordinate in 2D space.</summary>
    public struct Coordinate2D
    {
        /// <summary>The x coordinate.</summary>
        public int x;
        /// <summary>The y coordinate.</summary>
        public int y;
        
        /// <summary>Initializes a new instance of the <see cref="LevelGeneration.Coordinate2D"/> 
        /// struct.</summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public Coordinate2D(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        
        /// <summary>Converts this <see cref="Coordinate2D"/> to a <see cref="Vector2"/> position.
        /// </summary>
        /// <returns>The <see cref="Vector2"/> position pointed to by this 
        /// <see cref="Coordinate2D"/>.</returns>
        /// <param name="scale">The scale between grid coordinates and real world position.</param>
        /// <remarks>This method allows you to convert 2D coordinates to real world positions. 
        /// You can allow for a scale change between coordinates and real world coordinates by 
        /// altering scale. For example, </remarks>
        public Vector2 ToVector2(float scale = 1.0f)
        {
            // Cast the (x, y) values to floats, multiply them by the map scale, and return them in 
            // a new Vector2.
            return new Vector2((float)x * scale, (float)y * scale);
        }
        
        public int this[int i]
        {
            get
            {
                switch(i)
                {
                    case 0:
                        return x;
                        break;
                    case 1:
                        return y;
                        break;
                    default:
                        Log.CoordinateIndexOutOfBounds(i);
                        return 0;
                }
            }
            set
            {
                switch(i)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        Log.CoordinateIndexOutOfBounds(i);
                        break;
                }
            }
        }
        
        public override string ToString()
        {
            return string.Format(StringFormat.Coordinate2D, x, y);
        }
    }
    
    /// <summary>Represents a grid coordinate in 3D space.</summary>
    public struct Coordinate3D
    {
        /// <summary>The x coordinate.</summary>
        public int x;
        /// <summary>The y coordinate.</summary>
        public int y;
        /// <summary>The z coordinate.</summary>
        public int z;
        
        /// <summary>Initializes a new instance of the <see cref="LevelGeneration.Coordinate3D"/> 
        /// struct.</summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        public Coordinate3D(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        
        public Vector3 ToVector3(float scale = 1.0f)
        {
            // Cast the (x, y, z) values to floats, multiply them by the map scale, and return them 
            // in a new Vector3.
            return new Vector3((float)x * scale, (float)y * scale, (float)z * scale);
        }
        
        public int this[int i]
        {
            get
            {
                switch(i)
                {
                    case 0:
                        return x;
                        break;
                    case 1:
                        return y;
                        break;
                    case 2:
                        return z;
                        break;
                    default:
                        Log.CoordinateIndexOutOfBounds(i);
                        return 0;
                }
            }
            set
            {
                switch(i)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default:
                        Log.CoordinateIndexOutOfBounds(i);
                        break;
                }
            }
        }
        
        public override string ToString()
        {
            return string.Format(StringFormat.Coordinate3D, x, y, z);
        }
    }
}

namespace LevelGeneration.Utility
{
    public static partial class LevelGenerationDebug
    {
        private const string coordinateIndexOutOfBoundsError = "Error: Trying to access " 
            + "coordinate with index outside of array bounds: ";
        
        public static void CoordinateIndexOutOfBounds(int index)
        {
            Debug.Log(coordinateIndexOutOfBoundsError + index);
        }
    }
    
    public static partial class LevelGenerationStringFormats
    {
        public const string Coordinate2D = "Coordinate2D ({0}, {1})";
        public const string Coordinate3D = "Coordinate3D ({0}, {1}, {2})";
    }
}