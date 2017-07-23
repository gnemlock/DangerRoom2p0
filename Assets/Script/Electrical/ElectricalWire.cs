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
        
    // TODO: Create and test functionality for 'electrical spark' travelling down wire.
    // TODO: Create functionality to determine exact position given normalised value (0 - 1)

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

        /// <summary>Gets the specified position co-ordinates for this 
        /// <see cref="Electrical.ElectricalWire">.</summary>
        /// <params name="positionID">The ID number of the position to be retrieved.</params>
        /// <returns>The position co-ordinates specified by the ID number</returns>
        public Vector3 GetPosition(int positionID)
        {
            try
            {
                // Try to return the positions element pointed to by the positionID;
                return positions[positionID];
            }
            catch(System.IndexOutOfRangeException exception)
            {
                // if this causes an IndexOutOfRangeException, log an error, and return the
                // default position.
                Log.AttemptingToGetInvalidPosition(positionID);
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

        /// <summary>Sets the position of a specified positions coordinate in this 
        /// <see cref="Electrical.ElectricalWire">.</summary>
        /// <params name="positionID">The ID number of the position to be set.</params>
        /// <params name="position">The new position value</params>
        public void SetPosition(int positionID, Vector3 position)
        {
            try
            {
                // Try to set the new position of the positions element referenced 
                // by the positionID;
                positions[positionID] = position;
            }
            catch(System.IndexOutOfRangeException exception)
            {
                // if this causes an IndexOutOfRangeException, log the error.
                Log.AttemptingToSetInvalidPosition(positionID);
            }
        }

        /// <summary>Returns the total length of this 
        /// <see cref="Electrical.ElectricalWire">.</summary>
        /// <returns>The total length of this <see cref="Electrical.ElectricalWire">.</returns>
        public override float TotalLength()
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
    /// <summary>This class provides additional functionality to the editor.</summary>
    [CustomEditor(typeof(ElectricalWire))] public class ElectricalWireEditor : Editor
    {
        /// <summary>Cached reference to the target <see cref="Electrical.ElectricalWire"/>.</summary>
        private ElectricalWire electricalWire;
        /// <summary>Cached reference to the target <see cref="Transform"/>.</summary>
        private Transform transform;
        /// <summary>Cached reference to the intended handle rotation.</summary>
        private Quaternion handleRotation;
                
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            electricalWire = target as ElectricalWire;
        }

        /// <summary>This method will be called to draw the <see cref="Electrical.ElectricalWire"/>
        /// in to the scene view.</summary>
        private void OnSceneGUI()
        {
            // Explicitly reference the target class as an ElectricalWire, so we have ElectricalWire 
            // specific access, and reference the target transform.
            electricalWire = target as ElectricalWire;
            transform = electricalWire.transform;

            // Cache a reference to the current handle rotation. If we are using local rotation, 
            // this will be the transform rotation. If we are using world rotation, this will 
            // be the default identity value.
            handleRotation = (Tools.pivotRotation == PivotRotation.Local) ? 
                transform.rotation : Quaternion.identity;

            // Create a vector array to represent the position of each handle.
            Vector3[] handlePositions = new Vector3[electricalWire.positionCount];

            for(int i = 0; i < electricalWire.positionCount; i++)
            {
                // for each handle position, process the handle, and update the handlePosition.
                handlePositions[i] = ShowHandle(i);
            }

            // Set the Handles.color to the wireColour, in preparation for drawing the wire.
            Handles.color = ElectricalWireColours.wireColour;

            for(int i = 0; i < electricalWire.positionCount - 1; i++)
            {
                // For each wire position, draw a line between the two positions.
                Handles.DrawLine(handlePositions[i], handlePositions[i + 1]);
            }
        }

        /// <summary>Draws a handle at the specified point, and manages user translation.</summary>
        /// <returns>The position of the handle, updated to reflect user translation.</returns>
        /// <param name="pointIndex">The index of the position in <see cref="Electrical.ElectricalWire"/> 
        /// to which we are to draw a handle for.</param>
        private Vector3 ShowHandle(int positionIndex)
        {
            // Create a local position to contain the position pointed to by the requested index, 
            // converted to world coordinates.
            Vector3 position = transform.TransformPoint(electricalWire.GetPosition(positionIndex));

            // Perform a BeginChangeCheck so we can tell if the position of the handle changes, 
            // through user translation.
            EditorGUI.BeginChangeCheck();
    
            // Create a handle at the determined point, using the current rotation, and update 
            // point to reflect any new changes caused by user translation.
            position = Handles.DoPositionHandle(position, handleRotation);

            // If the editor detected change, i.e. the user translated the handle via scene view, 
            // Record a change to the inspector and update the original position reference in 
            // the actual curve to reflect the new position in local coordinates.
            if(EditorGUI.EndChangeCheck())
            {
                this.PrepareChange(electricalWire, 
                    ElectricalLabels.moveElectricalWirePositionDescription);
                electricalWire
                    .SetPosition(positionIndex, transform.InverseTransformPoint(position));
            }

            // Return the final position.
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
        /// <summary>The colour to use when drawing wires into the Unity Editor.</summary>
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
        private const string attemptingToGetInvalidPosition = "Attempting to get an invalid " 
            + "position index of {0}.";
        private const string attemptingToSetInvalidPosition = "Attempting to set an invalid " 
            + "position index of {0}.";
        
        public static void AttemptingToGetInvalidPosition(int positionID)
        {
            Debug.LogError(string.Format(attemptingToGetInvalidPosition, positionID));
        }
        
        public static void AttemptingToSetInvalidPosition(int positionID)
        {
            Debug.LogError(string.Format(attemptingToSetInvalidPosition, positionID));
        }
    }
    
    // Strings used for tag or name comparison
    public static partial class ElectricalTags
    {
    }
}
