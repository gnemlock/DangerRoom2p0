/* Created by Matthew Francis Keating */

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Electrical
{
    using StringFormats = Utility.ElectricalStringFormats;
    using Labels = Utility.ElectricalLabels;
    using Log = Utility.ElectricalDebug;
    using Tags = Utility.ElectricalTags;

    #if UNITY_EDITOR
    using Tooltips = Utility.ElectricalWireTooltips;
    using Colours = Utility.ElectricalWireColours;
    using Dimensions = Utility.ElectricalWireDimensions;
    #endif
        
    // TODO: Add basic drawing functionality
    // TODO: Create and test functionality for 'electrical spark' travelling down wire.

    /// <summary>Represents an electrical wire, designed to link two <see cref="Electrical.ElectricalComponent"> 
    /// types, to transfer power.</summary>
    /// <remarks>This <see cref="Electrical.ElectricalWire"> is designed using an ideal model; that is, 
    /// it does not take the resistance of the wire into account.</remarks>
    public class ElectricalWire : ElectricalComponent 
    {
        /// <summary>The physical co-ordinates of which the wire runs along, from start to finish.</summary>
        /// <remarks>If the wire exists in a 3D world, these co-ordinates should be the used as co-ordinates 
        /// local to the circuit board.</remarks>
        //TODO: Set up ElectricalWire to use these co-ordinates to draw movable handles, in the inspector, as well as adding or removing points.
        //TODO: Restrict size to atleast 2
        [SerializeField][Tooltip(Tooltips.positions)] private Vector2[] positions;

        /// <summary>Returns the starting position of the wire.</summary>
        /// <remarks>This is the same as calling <c>positions[0]</c>.</remarks>
        public Vector2 start { get { return positions[0]; } }
        /// <summary>Returns the ending position of the wire.</summary>
        /// <remarks>This is the same as calling <c>positions[positions.Length - 1]</c>.</remarks>
        public Vector2 end { get { return positions[segmentCount]; } }
        /// <summary>Returns the count of individual segments, seperated by differant co-ordinates, in the wire.</summary>
        /// <remarks>This is the same as calling <c>positions.Length - 1</c>.</remarks>
        public int segmentCount { get { return positions.Length - 1; } }

        /// <summary>Returns the total length of this <see cref="Electrical.ElectricalWire">.</summary>
        /// <returns>The total length of this <see cref="Electrical.ElectricalWire">.</returns>
        public float TotalLength()
        {
            // Store a float to start summing up the distance between segments
            float totalDistance = 0f;

            for(int i = 1; i < positions.Length; i++)
            {
                // For each wire segment, calculate the Vector2.Distance between the two points, 
                // and add it to our total distance.
                totalDistance += Vector2.Distance(positions[i - 1], positions[i]);
            }

            // Return the final totalDistance.
            return totalDistance;
        }

        /// <summary>Returns the length of an individual segment, in this 
        /// <see cref="Electrical.ElectricalWire">.</summary>
        /// <params name="segmentID">The segment intended to be measured, identified by number.
        /// Consider the first segment to be <c>segmentID = 1</c>.</params>
        /// <returns>The length of the identified segment, or <c>0f</c>, should the <c>segmentID</c>
        /// point to an invalid segment.</returns>
        public float SegmentLength(int segmentID)
        {
            if(segmentID < 1 || segmentID > segmentCount)
            {
                // If the segmentID is less than 1 or greater than the segmentCount, it points to
                // an invalid segment; return 0f.
                return 0f;
            }

            // Return the Vector2.Distance between the two co-ordinates identified by the segmentID,
            // where the segmentID points to the ending position of the segment.
            return Vector2.Distance(positions[segmentID - 1], positions[segmentID]);
        }

        /// <summary>Returns the total length of a set of specified segments, in this 
        /// <see cref="Electrical.ElectricalWire">.</summary>
        /// <params name="startSegmentID">The segment to start measuring from, identified by number.
        /// Consider the first segment to be <c>startSegmentID = 1</c>, where any lower value will 
        /// be reset to <c>1</c>.</params>
        /// <params name="endSegmentID">The segment to finish measuring at, identified by number.
        /// The last segment will be included in the measurement, and any ID outside of the bounds 
        /// will be reset to the final segment.</params>
        /// <returns>The length of the inclusive segments between the provide values.</returns>
        /// <remarks>Should both ID values be equal, the measurement will simply defer to 
        /// <see cref="Electrical.ElectricalWire.SegmentLength(int)">. The start ID should always 
        /// be lower than the end ID; however, if the start ID is actually higher, the distance will 
        /// still be calculated. The values will still have to be swapped, in order to facilitate 
        /// the for loop, so ensuring correct order will still be slightly quicker.</remarks>
        public float SegmentLength(int startSegmentID, int endSegmentID)
        {
            if(startSegmentID > endSegmentID)
            {
                // If the startSegmentID is greater than the endSegmentID, the segmentID values 
                // are in the wrond order; use a placeholder to swap the values around.
                int newStartSegmentID = endSegmentID;
                endSegmentID = startSegmentID;
                startSegmentID = newStartSegmentID;
            }
            
            if(startSegmentID < 1)
            {
                // If the startSegmentID is less than 1, it points to an invalid segment; 
                // reset it back to the first segment.
                startSegmentID = 1;
            }

            if(endSegmentID > segmentCount)
            {
                // If the endSegmentID is greater than the segmentCount, it points to an invalid 
                // segment; reset it back to the last segment.
                endSegmentID = segmentCount;
            }

            if(startSegmentID == endSegmentID)
            {
                // If the two segmentIDs are the same, return the distance of the single segment.
                return SegmentLength(startSegmentID);
            }

            // Store a float to start summing up the distance between segments
            float totalDistance = 0f;

            for(int i = startSegmentID; i <= endSegmentID; i++)
            {
                // For each segment, between and inclusive of the segments pointed to by the startSegmentID 
                // and endSegmentID, add the distance of the segment to the totalDistance.
                totalDistance += Vector2.Distance(positions[i - 1], positions[i]);
            }

            // Return the final distance.
            return totalDistance;
        }
    }
}

namespace Electrical.Utility
{
    [CustomEditor(typeof(ElectricalWire))] public class ElectricalWireEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ElectricalWire electricalWire = target as ElectricalWire;
        }
    }

    #if UNITY_EDITOR
    // Strings used to generate tooltips for the editor.
    public static class ElectricalWireTooltips
    {
        public const string positions = "The 2D co-ordinates of each point being used to draw the wire in 2D space.";
    }

    // Colours for use in displaying custom editor GUI.
    public static class ElectricalWireColours
    {
    }

    // Dimensions for use in displaying custom editor GUI.
    public static class ElectricalWireDimensions
    {
    }
    #endif

    // Strings containing format for string interface display.
    public static partial class ElectricalStringFormats
    {
    }
    
    // Strings used for general labelling.
    public static partial class ElectricalLabels
    {
    }
    
    // Provides debug functionality, including methods and customised string messages.
    public static partial class ElectricalDebug
    {
    }
    
    // Strings used for tag or name comparison
    public static partial class ElectricalTags
    {
    }
}
