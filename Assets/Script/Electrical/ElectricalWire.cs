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

    /// <summary>Represents an electrical wire, designed to link two 
    /// <see cref="Electrical.ElectricalComponent"> types, to transfer power.</summary>
    /// <remarks>This <see cref="Electrical.ElectricalWire"> is designed using an ideal model; that 
    /// is, it does not take the resistance of the wire into account.</remarks>
    public class ElectricalWire : ElectricalComponent 
    {
        /// <summary>The physical co-ordinates of which the wire runs along, 
        /// from start to finish.</summary>
        //TODO: Set up ElectricalWire to use these co-ordinates to draw movable handles, in the inspector, as well as adding or removing points.
        //TODO: Restrict size to atleast 2
        [SerializeField][Tooltip(Tooltips.positions)] private Vector3[] positions;

        /// <summary>Returns the starting position of the wire.</summary>
        /// <remarks>This is the same as calling <c>positions[0]</c>.</remarks>
        public Vector3 start { get { return positions[0]; } }
        /// <summary>Returns the ending position of the wire.</summary>
        /// <remarks>This is the same as calling <c>positions[positions.Length - 1]</c>.</remarks>
        public Vector3 end { get { return positions[segmentCount]; } }
        /// <summary>Returns the count of individual segments, seperated by differant co-ordinates, 
        /// in the wire.</summary>
        /// <remarks>This is the same as calling <c>positions.Length - 1</c>.</remarks>
        public int segmentCount { get { return positions.Length - 1; } }
        /// <summary>Returns the count of individual positions.</summary>
        public int positionCount { get { return positions.Length; } }

        public Vector3 GetPosition(int positionID)
        {
            try
            {
                return positions[positionID];
            }
            catch(System.IndexOutOfRangeException exception)
            {
                //TODO: Throw Error
                return Vector3.zero;
            }
        }

        /// <summary>Returns the length of an individual segment, in this 
        /// <see cref="Electrical.ElectricalWire">.</summary>
        /// <params name="segmentID">The segment intended to be measured, identified by number.
        /// Consider the first segment to be <c>segmentID = 1</c>.</params>
        /// <returns>The length of the identified segment, or <c>0f</c>, should the 
        /// <c>segmentID</c> point to an invalid segment.</returns>
        public float SegmentLength(int segmentID)
        {
            if(segmentID < 1 || segmentID > segmentCount)
            {
                // If the segmentID is less than 1 or greater than the segmentCount, it points to
                // an invalid segment; return 0f.
                return 0f;
            }

            // Return the Vector3.Distance between the two co-ordinates identified by the 
            // segmentID, where the segmentID points to the ending position of the segment.
            return Vector3.Distance(positions[segmentID - 1], positions[segmentID]);
        }

        /// <summary>Returns the total length of a set of specified segments, in this 
        /// <see cref="Electrical.ElectricalWire">.</summary>
        /// <params name="startSegmentID">The segment to start measuring from, identified by 
        /// number. Consider the first segment to be <c>startSegmentID = 1</c>, where any lower 
        /// value will be reset to <c>1</c>.</params>
        /// <params name="endSegmentID">The segment to finish measuring at, identified by number.
        /// The last segment will be included in the measurement, and any ID outside of the bounds 
        /// will be reset to the final segment.</params>
        /// <returns>The length of the inclusive segments between the provide values.</returns>
        /// <remarks>Should both ID values be equal, the measurement will simply defer to 
        /// <see cref="Electrical.ElectricalWire.SegmentLength(int)">. The start ID should always 
        /// be lower than the end ID; however, if the start ID is actually higher, the distance 
        /// will still be calculated. The values will still have to be swapped, in order to 
        /// facilitate the for loop, so ensuring correct order will still be slightly quicker.
        /// </remarks>
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
                // For each segment, between and inclusive of the segments pointed to by the 
                // startSegmentID and endSegmentID, add the Vector3.Distance of the segment to the 
                // totalDistance.
                totalDistance += Vector3.Distance(positions[i - 1], positions[i]);
            }

            // Return the final distance.
            return totalDistance;
        }

        public void SetPosition(int positionID, Vector3 position)
        {
            try
            {
                positions[positionID] = position;
            }
            catch(System.IndexOutOfRangeException exception)
            {
                //TODO: Throw Error
            }
        }

        /// <summary>Returns the total length of this 
        /// <see cref="Electrical.ElectricalWire">.</summary>
        /// <returns>The total length of this <see cref="Electrical.ElectricalWire">.</returns>
        public float TotalLength()
        {
            // Store a float to start summing up the distance between segments
            float totalDistance = 0f;

            for(int i = 1; i < positions.Length; i++)
            {
                // For each wire segment, calculate the Vector3.Distance between the two points, 
                // and add it to our total distance.
                totalDistance += Vector3.Distance(positions[i - 1], positions[i]);
            }

            // Return the final totalDistance.
            return totalDistance;
        }
    }
}

namespace Electrical.Utility
{
    #if UNITY_EDITOR
    [CustomEditor(typeof(ElectricalWire))] public class ElectricalWireEditor : Editor
    {
        private ElectricalWire electricalWire;
        private Transform transform;
        private Quaternion handleRotation;
                
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            electricalWire = target as ElectricalWire;
        }

        private void OnSceneGUI()
        {
            electricalWire = target as ElectricalWire;
            transform = electricalWire.transform;

            handleRotation = (Tools.pivotRotation == PivotRotation.Local) ? 
                transform.rotation : Quaternion.identity;

            Vector3[] handlePositions = new Vector3[electricalWire.positionCount];

            for(int i = 0; i < electricalWire.positionCount; i++)
            {
                handlePositions[i] = ShowHandle(i);
            }

            Handles.color = ElectricalWireColours.wireColour;

            for(int i = 0; i < electricalWire.positionCount - 1; i++)
            {
                Handles.DrawLine(handlePositions[i], handlePositions[i + 1]);
            }
        }

        private Vector3 ShowHandle(int positionIndex)
        {
            Vector3 position = transform.TransformPoint(electricalWire.GetPosition(positionIndex));

            EditorGUI.BeginChangeCheck();

            position = Handles.DoPositionHandle(position, handleRotation);

            if(EditorGUI.EndChangeCheck())
            {
                this.PrepareChange(electricalWire, 
                    ElectricalLabels.moveElectricalWirePositionDescription);
                electricalWire
                    .SetPosition(positionIndex, transform.InverseTransformPoint(position));
            }

            return position;
        }
    }

    // Strings used to generate tooltips for the editor.
    public static class ElectricalWireTooltips
    {
        public const string positions = "The 2D co-ordinates of each point being used to draw the " 
            + "wire in 2D space.";
    }

    // Colours for use in displaying custom editor GUI.
    public static class ElectricalWireColours
    {
        public static Color wireColour = Color.red;
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
        public const string moveElectricalWirePositionDescription = "Moving ElectricalWire "
            + "position";
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
