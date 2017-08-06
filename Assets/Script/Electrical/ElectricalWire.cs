/* Created by Matthew Francis Keating */

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

//TODO: Fix issue with positions returning null array, even when visually created in inspector
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
	// TODO: Create reverse lerping

    /// <summary>Represents an electrical wire, designed to link two 
    /// <see cref="Electrical.ElectricalComponent"> types, to transfer power.</summary>
    /// <remarks>This <see cref="Electrical.ElectricalWire"> is designed using an ideal model; that 
    /// is, it does not take the resistance of the wire into account.</remarks>
    public class ElectricalWire : ElectricalComponent 
    {
        /// <summary>The physical co-ordinates of which the wire runs along, 
        /// from start to finish.</summary>
        //TODO: adding or removing points.
        //TODO: Restrict size to atleast 2
        [SerializeField][Tooltip(Tooltips.positions)] private Vector3[] positions;
        //TODO: float[] distances Comment
        private float[] distances;

        /// <summary>Returns the starting position of the wire, in world coordinates.</summary>
        public Vector3 startPosition { get { return transform.TransformPoint(positions[0]); } }
        /// <summary>Returns the ending position of the wire in world coordinates.</summary>
        public Vector3 endPosition
        {
            get
            {
                return transform.TransformPoint(positions[segmentCount]);
            }
        }
        /// <summary>Returns the count of individual segments, seperated by differant co-ordinates, 
        /// in the wire.</summary>
        /// <remarks>This is the same as calling <c>positions.Length - 1</c>.</remarks>
        public int segmentCount { get { return positions.Length - 1; } }
        /// <summary>Returns the count of individual positions.</summary>
        public int positionCount { get { return positions.Length; } }

        //TODO: Start() Comment
        protected override void Start()
        {
            // Initialise the distance array by calculating the distances along each wire segment.
            CalculateDistances();
        }

        //TODO: CalculateDistances() Comments
        private void CalculateDistances()
        {
            distances = new float[positions.Length];
            distances[(distances.Length - 1)] = 0f;

            for(int i = 0; i < (distances.Length - 1); i++)
            {
                distances[i] = Vector3.Distance(positions[i], positions[i + 1]);
                distances[distances.Length - 1] += distances[i];
            }
        }

        /// <summary>
        /// Calculates the distances.
        /// </summary>
        /// <param name="positionIndex">The index of the position that has been changed.</param>
        //TODO:CalculateDistances(int) Comments
        private void CalculateDistances(int positionIndex)
        {
            float distanceDifference;

            if(positionIndex <= 1)
            {
                distanceDifference = distances[0];
                distances[0] = Vector3.Distance(positions[0], positions[1]);
                distanceDifference -= distances[0];
            }
            else if(positionIndex >= segmentCount)
            {
                distanceDifference = distances[segmentCount];
                distances[segmentCount - 1] 
                    = Vector3.Distance(positions[segmentCount], positions[segmentCount - 1]);
                distanceDifference -= distances[segmentCount - 1];
            }
            else
            {
                distanceDifference = distances[positionIndex] + distances[positionIndex - 1];
                distances[positionIndex - 1] 
                    = Vector3.Distance(positions[positionIndex - 1], positions[positionIndex]);
                distances[positionIndex] 
                    = Vector3.Distance(positions[positionIndex], positions[positionIndex + 1]);
                distanceDifference -= (distances[positionIndex] + distances[positionIndex - 1]);
            }

            distances[segmentCount] -= distanceDifference;
        }

        //TODO: GetNormalisedPosition(float) Comments
        //TODO: Test and clean GetNormalisedPosition(float)
        public Vector3 GetNormalisedPosition(float positionIncrement)
        {
            if(positionIncrement <= 0f)
            {
                return startPosition;
            }
            else if(positionIncrement >= 1.0f)
            {
                return endPosition;
            }
            else
            {
                //TODO:This is currently outputting the correct key positions at the right increment of time; but not positions between the key positions
                float currentDistance = 0f;
                float maxDistance = distances[segmentCount];
                float currentPositionIncrement = 0f;
                float lastPositionIncrement = 0f;
                float incrementDifference = 0f;
                float segmentIncrement = 0f;

                for(int i = 0; i < segmentCount; i++)
                {
                    currentDistance += distances[i];
                    currentPositionIncrement = currentDistance / maxDistance;

                    if(currentPositionIncrement >= positionIncrement)
                    {
                        incrementDifference = currentPositionIncrement - lastPositionIncrement;
                        segmentIncrement = segmentIncrement - lastPositionIncrement;

                        return transform.TransformPoint(Vector3
                            .Lerp(positions[i], positions[i + 1], segmentIncrement));
                    }
                    else
                    {
                        lastPositionIncrement = currentPositionIncrement;
                    }
                }
                //TODO: If we get to here, something went wrong; handle error
                return Vector3.zero;
            }
        }

        /// <summary>Gets the specified position coordinates for this 
        /// <see cref="Electrical.ElectricalWire">, in local coordinates.</summary>
        /// <params name="positionIndex">The index of the position to be retrieved.</params>
        /// <returns>The position coordinates specified by the index.</returns>
        public Vector3 GetLocalPosition(int positionIndex)
        {
            try
            {
                // Try to return the positions element pointed to by the positionID;
                return positions[positionIndex];
            }
            catch(System.IndexOutOfRangeException exception)
            {
                // if this causes an IndexOutOfRangeException, log an error, and return the
                // default position.
                Log.AttemptingToGetInvalidPosition(positionIndex);
                return Vector3.zero;
            }
        }

        //TODO: GetWorldPosition(int) comments
        public Vector3 GetWorldPosition(int positionIndex)
        {
            return transform.TransformPoint(GetLocalPosition(positionIndex));
        }

        /// <summary>Returns the length of an individual segment, in this 
        /// <see cref="Electrical.ElectricalWire">.</summary>
        /// <params name="segmentIndex">The segment intended to be measured, identified by index. 
        /// Consider the first segment to be <c>segmentIndex = 1</c>.</params>
        /// <returns>The length of the identified segment, or <c>0f</c>, should the 
        /// <c>segmentIndex</c> point to an invalid segment.</returns>
        public float SegmentLength(int segmentIndex)
        {
            if(segmentIndex < 1 || segmentIndex > segmentCount)
            {
                // If the segmentIndex is less than 1 or greater than the segmentCount, it points 
                // to an invalid segment; return 0f.
                return 0f;
            }

            // Return the Vector3.Distance between the two co-ordinates identified by the 
            // segmentIndex, where the segmentIndex points to the ending position of the segment.
            return Vector3.Distance(positions[segmentIndex - 1], positions[segmentIndex]);
        }

        /// <summary>Returns the total length of a set of specified segments, in this 
        /// <see cref="Electrical.ElectricalWire">.</summary>
        /// <params name="startSegmentIndex">The segment to start measuring from, identified by 
        /// number. Consider the first segment to be <c>startSegmentIndex = 1</c>, where any lower 
        /// value will be reset to <c>1</c>.</params>
        /// <params name="endSegmentIndex">The segment to finish measuring at, identified by index. 
        /// The last segment will be included in the measurement, and any index outside of the 
        /// bounds will be reset to the final segment.</params>
        /// <returns>The length of the inclusive segments between the provide values.</returns>
        /// <remarks>Should both indices be equal, the measurement will simply defer to 
        /// <see cref="Electrical.ElectricalWire.SegmentLength(int)">. The start index should 
        /// always be lower than the end index; however, if the start index is actually higher, the 
        /// distance will still be calculated. The values will still have to be swapped, in order 
        /// to facilitate the for loop, so ensuring correct order will still be slightly quicker.
        /// </remarks>
        public float SegmentLength(int startSegmentIndex, int endSegmentIndex)
        {
            if(startSegmentIndex > endSegmentIndex)
            {
                // If the startSegmentIndex is greater than the endSegmentIndex, the segmentIndices 
                // are in the wrong order; use a placeholder to swap the values around.
                int newStartSegmentID = endSegmentIndex;
                endSegmentIndex = startSegmentIndex;
                startSegmentIndex = newStartSegmentID;
            }
            
            if(startSegmentIndex < 1)
            {
                // If the startSegmentIndex is less than 1, it points to an invalid segment; 
                // reset it back to the first segment.
                startSegmentIndex = 1;
            }

            if(endSegmentIndex > segmentCount)
            {
                // If the endSegmentIndex is greater than the segmentCount, it points to an invalid 
                // segment; reset it back to the last segment.
                endSegmentIndex = segmentCount;
            }

            if(startSegmentIndex == endSegmentIndex)
            {
                // If the two indices are the same, return the distance of the single segment.
                return SegmentLength(startSegmentIndex);
            }

            // Store a float to start summing up the distance between segments
            float totalDistance = 0f;

            for(int i = startSegmentIndex; i <= endSegmentIndex; i++)
            {
                // For each segment, between and inclusive of the segments pointed to by the 
                // startSegmentIndex and endSegmentIndex, add the Vector3.Distance of the segment 
                // to the totalDistance.
                totalDistance += Vector3.Distance(positions[i - 1], positions[i]);
            }

            // Return the final distance.
            return totalDistance;
        }

        /// <summary>Sets the position of a specified positions coordinate in this 
        /// <see cref="Electrical.ElectricalWire">.</summary>
        /// <params name="positionIndex">The index of the position to be set.</params>
        /// <params name="position">The new position value</params>
        public void SetPosition(int positionIndex, Vector3 position)
        {
            try
            {
                // Try to set the new position of the positions element referenced 
                // by the positionIndex;
                positions[positionIndex] = position;

                // If setting the position was successful, update the distances array, by 
                // calculating the distances of the wires attached to the positionIndex.
                CalculateDistances(positionIndex);
            }
            catch(System.IndexOutOfRangeException exception)
            {
                // if this causes an IndexOutOfRangeException, log the error.
                Log.AttemptingToSetInvalidPosition(positionIndex);
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
            Vector3 position 
                = transform.TransformPoint(electricalWire.GetLocalPosition(positionIndex));

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
