/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/curves-and-splines/ ---
 */

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Drawing
{
    #if UNITY_EDITOR
    using Tooltips = Utility.QuadraticBeizerCurveTooltips;
    #endif
    
    /// <summary>Represents a quadratic Beizer curve; a Beizer curve that takes four coordinates.
    /// </summary>
    public class QuadraticBeizerCurve : MonoBehaviour
    {
        #if UNITY_EDITOR
        /// <summary>The colour of the line used to draw the curve.</summary>
        [Tooltip(Tooltips.lineColour)] public Color lineColour = Color.white;
        /// <summary>The colour of the line used to connect the main points</summary>
        [Tooltip(Tooltips.stepColour)] public Color stepColour = Color.gray;
        /// <summary>The colour of the lines used to display the tangents, along the curve</summary>
        [Tooltip(Tooltips.tangentColour)] public Color tangentColour = Color.green;
        /// <summary>Determines if the curve should display translation handles for it's  
        /// main points.</summary>
        [Tooltip(Tooltips.displayHandles)] public bool displayHandles = true;
        /// <summary>Determines if the curve should display its tangent lines.</summary>
        [Tooltip(Tooltips.displayTangents)] public bool displayTangents = true;
        /// <summary>The number of steps used to draw the curve between the coordinates in 
        /// <see cref="points"/>. A higher number will produce a smoother curve."/></summary>
        [Tooltip(Tooltips.lineSteps)] public int lineSteps = 10;
        #endif
        
        [Tooltip(Tooltips.pointOne)][SerializeField] protected Vector3 pointOne;
        [Tooltip(Tooltips.pointTwo)][SerializeField] protected Vector3 pointTwo;
        [Tooltip(Tooltips.pointThree)][SerializeField] protected Vector3 pointThree;
        
        #if UNITY_EDITOR
        /// <summary>This method will be called whenever the class is instantiated or reset via the 
        /// inspector. This method is EDITOR ONLY.</summary>
        public virtual void Reset()
        {
            // Set the first, second and third coordinate to default values.
            pointOne = new Vector3(1.0f, 0f, 0f);
            pointTwo = new Vector3(2.0f, 0f, 0f);
            pointThree = new Vector3(3.0f, 0f, 0f);
        }
        
        /// <summary>Resets the editor colours to their default values.  This method is EDITOR 
        /// ONLY.</summary>
        public void ResetColours()
        {
            lineColour = Color.white;
            stepColour = Color.gray;
            tangentColour = Color.green;
        }
        
        /// <summary>Gets a point based off it's "index" value. This method is EDITOR ONLY.
        /// </summary>
        /// <returns>The first, second or third point. An "out of bounds" exception will return 
        /// a value of <see cref="Vector3.zero"/>.</returns>
        /// <param name="index">The desired index value for the point. Should be treated like array
        ///  pointers, with an array length of 3.</param>
        public virtual Vector3 GetPoint(int index)
        {
            // Based off the provided index, 
            switch(index)
            {
                case 0:
                    // If the index is 0, return the first point.
                    return pointOne;
                case 1:
                    // If the index is 1, return the second point.
                    return pointTwo;
                case 2:
                    // If the index is 2, return the third point.
                    return pointThree;
                default:
                    // If the provided index is outside of the logical bounds, return "0".
                    return Vector3.zero;
            }
        }
        
        /// <summary>Sets a point based off it's "index" value. This method is EDITOR ONLY.
        /// </summary>
        /// <param name="index">The "index" value of the desired point. Should be treated like 
        /// array pointers, with an array length of 3.</param>
        /// <param name="point">The new value for the desired point.</param>
        public virtual void SetPoint(int index, Vector3 point)
        {
            // Based off the provided index, 
            switch(index)
            {
                case 0:
                    // If the index is 0, set the new value to the first point.
                    pointOne = point;
                    break;
                case 1:
                    // If the index is 1, set the new value the second point.
                    pointTwo = point;
                    break;
                case 2:
                    // If the index is 2, set the new value the third point.
                    pointThree = point;
                    break;
                default:
                    // If the provided index is outside of the logical bounds, set nothing.
                    return;
            }
        }
        
        /// <summary>Sets a point based off it's "index" value. This method is EDITOR ONLY.
        /// </summary>
        /// <param name="index">The "index" value of the desired point. Should be treated like 
        /// array pointers, with an array length of 3.</param>
        /// <param name="x">The x coordinate for the new point.</param>
        /// <param name="y">The y coordinate for the new point.</param>
        /// <param name="z">The z coordinate for the new point.</param>
        public virtual void SetPoint(int index, float x = 0f, float y = 0f, float z = 0f)
        {
            // Create a Vector3 using the provided x, y and z coordinates, and send it to the 
            // SetPoint(int, Vector3) method.
            SetPoint(index, new Vector3(z, y, z));
        }
        #endif
        
        /// <summary>Finds the normalised velocity of this curve, at a specified point.</summary>
        /// <remarks>This is the same as calling <see cref="GetVelocity().normalized"/>
        /// <returns>The velocity at the specified point, normalised.</returns>
        /// <param name="t">The specific point, defined as a normalised interpolant.
        /// This value will be clamped to 0 and 1.</param>
        public Vector3 GetDirection(float t)
        {
            // Return the velocity, normalized.
            return GetVelocity(0f).normalized;
        }
        
        //// <summary>Finds the coordinates of a specified point on this <see cref="BeizerCurbe"/>
        /// </summary>
        /// <returns>The coordinates of the point.</returns>
        /// <param name="t">The specific point, defined as a normalised interpolant.
        /// This value will be clamped to 0 and 1.</param>
        public virtual Vector3 GetPointOnCurve (float t)
        {
            // Find the point, and convert it to world coordinates before returning.
            return transform.TransformPoint(
                BeizerUtility.GetPoint(pointOne, pointTwo, pointThree, t));
        }
        
        /// <summary>Finds the velocity of this <see cref="BeizerCurve"/>, at a specified point.
        /// </summary>
        /// <returns>The velocity at the specified point.</returns>
        /// <param name="t">The specific point, defined as a normalised interpolant.
        /// This value will be clamped to 0 and 1.</param>
        public virtual Vector3 GetVelocity(float t)
        {
            // Find the point, and convert it to world coordinates. Velocity is a direction, not 
            // a position, so remove the position offset before returning.
            return transform.TransformPoint(
                BeizerUtility.GetFirstDerivative(pointOne, pointTwo, pointThree, t))
                - transform.position;
        }
    }
}


namespace Drawing.Utility
{
    /// <summary>This class holds the tooltips for variables serialized to the inspector.</summary>
    public static class QuadraticBeizerCurveTooltips
    {
        #if UNITY_EDITOR
        public const string lineColour = "What colour should the curved line be drawn in?";
        public const string stepColour = "What colour should the lines connecting the Vector3 " +
            "coordinates be drawn in?";
        public const string tangentColour = "What colour should the tangent lines projecting off " +
            "the curve be drawn in?";
        public const string displayHandles = "Should the scene view display translation handles " +
            "for each point?";
        public const string displayTangents = "Should the scene view display the tangent lines, " +
            "along the curve?";
        public const string lineSteps = "How many steps should we use in drawing the curve?" +
            "More steps will create a smoother curve.";
        public const string pointOne = "The first main point for this curve.";
        public const string pointTwo = "The second main point for this curve.";
        public const string pointThree = "The third main point for this curve.";
        #endif
    }

    /// <summary>This class provides additional functionality to the editor.</summary>
    [CustomEditor(typeof(QuadraticBeizerCurve))] public class QuadraticBeizerCurveInspector : Editor
    {
        #if UNITY_EDITOR
        /// <summary>Cached reference to the target <see cref="QuadraticBeizerCurve"/>.</summary>
        private QuadraticBeizerCurve quadraticBeizerCurve;
        /// <summary>Cached reference to the target <see cref="Transform"/>.</summary>
        private Transform transform;
        /// <summary>Cached reference to the intended handle rotation.</summary>
        private Quaternion handleRotation;
        
        /// <summary>This method will be called to draw the inspector interface for the target 
        /// class.</summary>
        public override void OnInspectorGUI()
        {
            // Explicitly reference the target class as a QuadraticBeizerCurve, so we have 
            // QuadraticBeizerCurve specific access.
            quadraticBeizerCurve = target as QuadraticBeizerCurve;
            
            // Draw the default inspector, so we still have the default interface.
            DrawDefaultInspector();
            
            if(GUILayout.Button("Reset Colours"))
            {
                // Add a button called "Reset Colours", and when it is pressed, 
                // reset the colours used in the curve
                quadraticBeizerCurve.ResetColours();
            }
        }
        
        /// <summary>This method will be called to draw the <see cref="QuadraticBeizerCurve"/>
        /// in to the scene view.</summary>
        private void OnSceneGUI()
        {
            // Explicitly reference the target class as a QuadraticBeizerCurve, so we have 
            // QuadraticBeizerCurve specific access, and create a local reference to the transform.
            quadraticBeizerCurve = target as QuadraticBeizerCurve;
            transform = quadraticBeizerCurve.transform;
            
            // Cache a reference to the current handle rotation. If we are using local rotation, 
            // this will be the transform rotation. If we are using world rotation, this will 
            // be the default identity value.
            handleRotation = (Tools.pivotRotation == PivotRotation.Local) ? 
                transform.rotation : Quaternion.identity;
            
            // Create a vector array, process the handles, and load the resulting positions into 
            // the array.
            Vector3[] handlePosition = new Vector3[3];
            handlePosition[0] = ShowHandle(0);
            handlePosition[1] = ShowHandle(1);
            handlePosition[2] = ShowHandle(2);
            
            // Draw the step lines between the two points. This will represent the initial line 
            // created by directly connecting the three points.
            Handles.color = quadraticBeizerCurve.stepColour;
            Handles.DrawLine(handlePosition[0], handlePosition[1]);
            Handles.DrawLine(handlePosition[1], handlePosition[2]);
            
            // Create an initial line start position to represent the start of our Beizer curve, 
            // or the normalised point of (0).
            Vector3 lineStart = quadraticBeizerCurve.GetPointOnCurve(0f);
            
            // Draw the first increment in the Beizer curve.
            Handles.color = quadraticBeizerCurve.tangentColour;
            Handles.DrawLine(lineStart, lineStart + quadraticBeizerCurve.GetDirection(0f));
            
            // Cache a local version of our line steps integer
            int lineSteps = quadraticBeizerCurve.lineSteps;
            
            for(int i = 1; i <= quadraticBeizerCurve.lineSteps; i++)
            {
                // For each line step,
                // Create a line end position, and return the Beizer curve position found at the 
                // increment specified by the normalised value of this line step.
                Vector3 lineEnd = quadraticBeizerCurve.GetPointOnCurve(i / (float)lineSteps);
                
                // Draw a curve line between the current start and end positions.
                Handles.color = quadraticBeizerCurve.lineColour;
                Handles.DrawLine(lineStart, lineEnd);
                
                // Draw a tangent line to show the acceleration.
                Handles.color = quadraticBeizerCurve.tangentColour;
                Handles.DrawLine(lineEnd, lineEnd 
                    + quadraticBeizerCurve.GetDirection(i / (float)lineSteps));
                
                // The end position of this line is now the start position of the next line.
                lineStart = lineEnd;
            }
        }
        
        /// <summary>Draws a handle at the specified point, and manages user translation.</summary>
        /// <returns>The position of the handle, updated to reflect user translation.</returns>
        /// <param name="pointIndex">The index of the point in <see cref="BeizerCurve.points"/> 
        /// to which we are to draw a handle for.</param>
        private Vector3 ShowHandle (int pointIndex)
        {
            // Create a local position to contain the position pointed to by the requested index, 
            // converted to world coordinates.
            Vector3 point = transform.TransformPoint(quadraticBeizerCurve.GetPoint(pointIndex));
            
            // Perform a BeginChangeCheck so we can tell if the position of the handle changes, 
            // through user translation.
            EditorGUI.BeginChangeCheck();
            
            // Create a handle at the determined point, using the current rotation, and update 
            // point to reflect any new changes caused by user translation.
            point = Handles.DoPositionHandle(point, handleRotation);
            
            if(EditorGUI.EndChangeCheck())
            {
                // If the editor detected change, i.e. the user translated the handle via scene view, 
                // Record a change to the inspector and update the original position reference in 
                // the actual curve to reflect the new position in local coordinates.
                this.PrepareChange(quadraticBeizerCurve, "Move Point - Quadratic Beizer Curve");
                quadraticBeizerCurve.SetPoint(pointIndex, transform.InverseTransformPoint(point));
            }
            
            // Return the updated position.
            return point;
        }
        #endif
    }
}