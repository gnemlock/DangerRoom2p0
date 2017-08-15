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

        /// <summary>The cached distance between each point in the 
        /// <see cref="Electrical.ElectricalWire.positions"/> array, with the final value being a 
        /// total sum.</summary>
        /// <remarks>Each index represents the distance from the same index in the 
        /// <see cref="Electrical.ElectricalWire.positions"/> array to the next index. The very 
        /// last distance contains the total distance between all positions.</remarks>
        private float[] distances;

        /// <summary>Returns the starting position of the wire, in world coordinates.</summary>
        public Vector3 startPosition
        {
            get
            {
                // Return the first position, transformed to world space.
                return transform.TransformPoint(positions[0]);
            }
        }
        /// <summary>Returns the ending position of the wire in world coordinates.</summary>
        public Vector3 endPosition
        {
            get
            {
                // Return the last position, transformed to world space.
                return transform.TransformPoint(positions[segmentCount]);
            }
        }
        /// <summary>Returns the count of individual segments, seperated by differant co-ordinates, 
        /// in the wire.</summary>
        /// <remarks>This is the same as calling <c>positions.Length - 1</c>.</remarks>
        public int segmentCount { get { return positions.Length - 1; } }
        /// <summary>Returns the count of individual positions.</summary>
        public int positionCount { get { return positions.Length; } }

        /// <summary>This method will be called just before the first Update call.</summary>
        protected override void Start()
        {
            // Initialise the distance array by calculating the distances along each wire segment.
            CalculateDistances();
        }

        /// <summary>Calculates all of the distances in the 
        /// <see cref="Electrical.ElectricalWire.positions"/> array, and stores them in the 
        /// <see cref="Electrical.ElectricalWire.distances"/> array.</summary>
        private void CalculateDistances()
        {
            // Create a new distances array, of the same length as the positions array, and set its 
            // last value to 0 so that we can add values to it. The last value will be our total.
            distances = new float[positions.Length];
            distances[(distances.Length - 1)] = 0f;

            for(int i = 0; i < (distances.Length - 1); i++)
            {
                // For each element in the distances array, minus the last, set the value as the 
                // distance between the current corresponding positions value, and the next 
                // positions value. Add this value to the last element in the distances away, so 
                // we are left with a total distance.
                distances[i] = Vector3.Distance(positions[i], positions[i + 1]);
                distances[distances.Length - 1] += distances[i];
            }
        }

        //TODO: I am not sure CalculateDistances(int) is correct; test, test, test.
        /// <summary>Calculates the distances of segments connected to a particular position in the 
        /// <see cref="Electrical.ElectricalWire.positions"/> array.</summary>
        /// <param name="positionIndex">The index of the position to start calculating distance 
        /// from.</param>
        /// <remarks>This method will correct the <see cref="Electrical.ElectricalWire.distances"/>
        /// array when an individual <c>positionIndex</c> has been changed.</remarks>
        private void CalculateDistances(int positionIndex)
        {
            // Create a placeholder to store the distance change for updating the total distance.
            // For further logical ease, this value will be inverted; a negative number will add to 
            // the final distance, while a positive number will be subtracted from it.
            float distanceDifference;

            if(positionIndex <= 1)
            {
                // If we are working with a positionIndex of 1 or less, we have changed the first 
                // position. Keep a record of our old segment distance, and calculate the new 
                // distance of the segment between positions 0 and 1; finally, subtract the new 
                // distance from the distanceDifference to give the final distanceDifference.
                distanceDifference = distances[0];
                distances[0] = Vector3.Distance(positions[0], positions[1]);
                distanceDifference -= distances[0];
            }
            else if(positionIndex >= segmentCount)
            {
                // If we are working with a positionIndex greater to or equal the value of our 
                // segmentCount, we have changed the last position. Keep a record of our old 
                // segment distance, and calculate the new distance of the segment between the 
                // last and second last positions; finally, subtract the new distance from the 
                // distanceDifference to give the final distanceDifference.
                distanceDifference = distances[segmentCount - 1];
                distances[segmentCount - 1] 
                    = Vector3.Distance(positions[segmentCount], positions[segmentCount - 1]);
                distanceDifference -= distances[segmentCount - 1];
            }
            else
            {
                // If we are not working with the first or last positionIndex, we need to account 
                // for the segments on either side of the position. Store the sum of the two 
                // segment distances, and calculate the new segment distances; finally, subtract 
                // the new distances from the distanceDifference to give the final 
                // distanceDifference.
                distanceDifference = distances[positionIndex] + distances[positionIndex - 1];
                distances[positionIndex - 1] 
                    = Vector3.Distance(positions[positionIndex - 1], positions[positionIndex]);
                distances[positionIndex] 
                    = Vector3.Distance(positions[positionIndex], positions[positionIndex + 1]);
                distanceDifference -= (distances[positionIndex] + distances[positionIndex - 1]);
            }

            // Subtract the final distanceDifference from the final distance, to update the total 
            // distance sum with all distance changes.
            distances[segmentCount] -= distanceDifference;
        }

        /// <summary>Gets a position along this <see cref="Electrical.ElectricalWire"/>, 
        /// using a normalised increment. Behaves in a similar fashion to 
        /// <see cref="UnityEngine.Vector3.Lerp(Vector3, Vector3, float)"/>.</summary>
        /// <returns>The position along this <see cref="Electrical.ElectricalWire"/>, as defined 
        /// by the provided increment value.</returns>
        /// <param name="positionIncrement">The required position increment, where <c>0</c> 
        /// refers to the <see cref="Electrical.ElectricalWire.startPosition"/>, <c>1</c> refers 
        /// to the <see cref="Electrical.ElectricalWire.endPosition"/> and <c>0.5</c> refers to a 
        /// position exactly in the middle.</param>
        /// <param name="allowRevolution">If set to <c>true</c>, interprets the 
        /// <c>positionIncrement</c> as revolutionary; that is, <c>1.25f</c> will become 
        /// <c>0.25f</c>, and <c>4.89f</c> will become <c>0.89f</c>.</param>
        /// <remarks>Positions are given along the wire; for instance, <c>0.5f</c> will point to 
        /// the exact middle distance when travelling along the wire specified by the 
        /// <see cref="Electrical.ElectricalWire.positions"/> coordinates, as opposed to a position 
        /// in the middle of <see cref="Electrical.ElectricalWire.startPosition"/> and 
        /// <see cref="Electrical.ElectricalWire.endPosition"/>.</remarks>
        public Vector3 GetNormalisedPosition(float positionIncrement, bool allowRevolution = false)
        {
            if(allowRevolution)
            {
                // If we are permitting revolution, reset the positionIncrement of the modulus of 
                // 1.0f, to determine the final incremental value.
                positionIncrement %= 1.0f;
            }

            if(positionIncrement <= 0f)
            {
                // If the positionIncrement is the minimum value, we are looking for the 
                // startPosition.
                return startPosition;
            }
            else if(positionIncrement >= 1.0f)
            {
                // If the positionIncrement is the maximum value, we are looking for the 
                // endPosition.
                return endPosition;
            }
            else
            {
                // If we are within the bounds to perform a Lerp, create a plaeholder to store 
                // the currentIncrement, the previousIncrement and accumulatedDistance, so we 
                // can determine the correct segment, and the increment within that segment.
                float currentIncrement = 0f;
                float previousIncrement = 0f;
                float accumulatedDistance = 0f;

                for(int i = 0; i < segmentCount; i++)
                {
                    // For each segment, store the currentIncrement as the previousIncrement, as 
                    // this is now our minimum bounds. Add the distance of this segment to our 
                    // accumulated distance, and divide it by the total overall distance to 
                    // determine the new currentIncrement, defining this segments maximum bounds.
                    previousIncrement = currentIncrement;
                    accumulatedDistance += distances[i];
                    currentIncrement = accumulatedDistance / distances[segmentCount];

                    if(currentIncrement > positionIncrement)
                    {
                        // If the currentIncrement is now greater than the originally passed in 
                        // positionIncrement, we are at the intended segment. Subtract the value 
                        // of our previousIncrement from both the passed in positionIncrement and 
                        // currentIncrement; this gives us the remaining positionIncrement that 
                        // we need to adjust for, and the normalised distance of the current 
                        // segment in relation to the overall total distance. Finally, divide the 
                        // passed in positionIncrement by the currentIncrement, to determine the 
                        // final positionIncrement as an increment of the current segment.
                        positionIncrement -= previousIncrement;
                        currentIncrement -= previousIncrement;
                        positionIncrement /= currentIncrement;

                        // Now we have calculated our final increment, we can use it to find the 
                        // Lerped distance between the positions bounding this segment. Find and 
                        // return the final position.
                        return Vector3.Lerp(GetWorldPosition(i), GetWorldPosition(i + 1), positionIncrement);
                    }
                }

                // We should never get to here, as we should already have returned a value; in case 
                // it does happen, throw an error, and return the default Vector3 value.
                Log.EndOfGetNormalisedPositionsReached(currentIncrement);
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
                // Try to return the positions element pointed to by the positionIndex;
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

        /// <summary>Gets the specified position coordinates for this 
        /// <see cref="Electrical.ElectricalWire">, in world coordinates.</summary>
        /// <params name="positionIndex">The index of the position to be retrieved.</params>
        /// <returns>The position coordinates specified by the index.</returns>
        public Vector3 GetWorldPosition(int positionIndex)
        {
            // Return the positions element pointed to by the positionIndex, in world 
            // coordinates. Note that we use the GetLocalPosition(int) method to retrieve the 
            // initial value, which also takes care of error handling.
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
        private const string endOfGetNormalisedPositionsReached = "This is weird; the end of the " 
            + "GetNormalisedPosition(int, bool) method has been reached, where it should have " 
            + "returned a value, by now. Returning a position of {0} with a passed in increment " 
            + "of {1}.";
        
        public static void AttemptingToGetInvalidPosition(int positionID)
        {
            Debug.LogError(string.Format(attemptingToGetInvalidPosition, positionID));
        }
        
        public static void AttemptingToSetInvalidPosition(int positionID)
        {
            Debug.LogError(string.Format(attemptingToSetInvalidPosition, positionID));
        }

        public static void EndOfGetNormalisedPositionsReached(float increment)
        {
            Debug.LogWarning(string
                .Format(endOfGetNormalisedPositionsReached, Vector3.zero, increment));
        }
    }
    
    // Strings used for tag or name comparison
    public static partial class ElectricalTags
    {
    }
}
