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
    using Tooltips = Utility.CubicBeizerCurveTooltips;
    #endif
    
    /// <summary>Represents a cubic Beizer curve; a Beizer curve that takes three coordinates.
    /// </summary>
    public class CubicBeizerCurve : QuadraticBeizerCurve
    {
        
        /// <summary>The points used to draw the curve.</summary>
        [Tooltip(Tooltips.pointFour)][SerializeField] protected Vector3 pointFour;
        
        #if UNITY_EDITOR
        /// <summary>This method will be called whenever the class is instantiated or reset via the 
        /// inspector. This method is EDITOR ONLY.</summary>
        public override void Reset()
        {
            // Set the fourth point coordinate to default values.
            pointFour = new Vector3(4.0f, 0f, 0f);
            
            // Run the parent Reset() method to set coordinates 1 - 3.
            base.Reset();
        }
        
        /// <summary>Gets a point based off it's "index" value. This method is EDITOR ONLY.
        /// </summary
        /// <returns>The first, second, third or fourth point. An "out of bounds" exception will 
        /// return a value of <see cref="Vector3.zero"/>.</returns>
        /// <param name="index">The desired index value for the point. Should be treated like array
        ///  pointers, with an array length of 4.</param>
        public override Vector3 GetPoint(int index)
        {
            if(index == 3)
            {
                // If the index is 4, return the fourth point.
                return pointFour;
            }
            else
            {
                // If not, check the parent GetPoint() method for the correct value.
                return base.GetPoint(index);
            }
        }
        
        /// <summary>Sets a point based off it's "index" value. This method is EDITOR ONLY.
        /// </summary>
        /// <param name="index">The "index" value of the desired point. Should be treated like 
        /// array pointers, with an array length of 4.</param>
        /// <param name="point">The new value for the desired point.</param>
        public override void SetPoint(int index, Vector3 point)
        {
            if(index == 3)
            {
                // If the index is 3, set the new value to the fourth point.
                pointFour = point;
            }
            else
            {
                // If not, check the parent SetPoint() method to set the new value.
                base.SetPoint(index, point);
            }
        }
        
        /// <summary>Sets a point based off it's "index" value. This method is EDITOR ONLY.
        /// </summary>
        /// <param name="index">The "index" value of the desired point. Should be treated like 
        /// array pointers, with an array length of 4.</param>
        /// <param name="x">The x coordinate for the new point.</param>
        /// <param name="y">The y coordinate for the new point.</param>
        /// <param name="z">The z coordinate for the new point.</param>
        public override void SetPoint(int index, float x = 0f, float y = 0f, float z = 0f)
        {
            // Create a Vector3 using the provided x, y and z coordinates, and send it to the 
            // SetPoint(int, Vector3) method.
            SetPoint(index, new Vector3(z, y, z));
        }
        #endif
        
        //// <summary>Finds the coordinates of a specified point on this <see cref="BeizerCurbe"/>
        /// </summary>
        /// <returns>The coordinates of the point.</returns>
        /// <param name="t">The specific point, defined as a normalised interpolant.
        /// This value will be clamped to 0 and 1.</param>
        public override Vector3 GetPointOnCurve (float t)
        {
            // Find the point, and convert it to world coordinates before returning.
            return transform.TransformPoint(
                BeizerUtility.GetPoint(pointOne, pointTwo, pointThree, pointFour, t));
        }
        
        /// <summary>Finds the velocity of this <see cref="CubicBeizerCurve"/>, at a specified point.
        /// </summary>
        /// <returns>The velocity at the specified point.</returns>
        /// <param name="t">The specific point, defined as a normalised interpolant.
        /// This value will be clamped to 0 and 1.</param>
        public override Vector3 GetVelocity(float t)
        {
            // Find the point, and convert it to world coordinates. Velocity is a direction, not 
            // a position, so remove the position offset before returning.
            return transform.TransformPoint(
                BeizerUtility.GetFirstDerivative(pointOne, pointTwo, pointThree, pointFour, t))
                - transform.position;
        }
    }
}


namespace Drawing.Utility
{
    /// <summary>This class holds the tooltips for variables serialized to the inspector.</summary>
    public static class CubicBeizerCurveTooltips
    {
        #if UNITY_EDITOR
        public const string pointFour = "The fourth main point for this curve.";
        #endif
    }
    
    /// <summary>This class provides additional functionality to the editor.</summary>
    [CustomEditor(typeof(CubicBeizerCurve))] public class CubicBeizerCurveInspector : Editor
    {
        #if UNITY_EDITOR
        /// <summary>Cached reference to the target <see cref="CubicBeizerCurve"/>.</summary>
        private CubicBeizerCurve cubicBeizerCurve;
        /// <summary>Cached reference to the target <see cref="Transform"/>.</summary>
        private Transform transform;
        /// <summary>Cached reference to the intended handle rotation.</summary>
        private Quaternion handleRotation;
        
        /// <summary>This method will be called to draw the inspector interface for the target 
        /// class.</summary>
        public override void OnInspectorGUI()
        {
            // Explicitly reference the target class as a CubicBeizerCurve, so we have 
            // CubicBeizerCurve specific access.
            cubicBeizerCurve = target as CubicBeizerCurve;
            
            // Draw the default inspector, so we still have the default interface.
            DrawDefaultInspector();
            
            if(GUILayout.Button("Reset Colours"))
            {
                // Add a button called "Reset Colours", and when it is pressed, 
                // reset the colours used in the curve
                cubicBeizerCurve.ResetColours();
            }
        }
        
        /// <summary>This method will be called to draw the <see cref="CubicBeizerCurve"/>
        /// in to the scene view.</summary>
        private void OnSceneGUI()
        {
            // Explicitly reference the target class as a CubicBeizerCurve, so we have CubicBeizerCurve 
            // specific access, and create a local reference to the transform.
            cubicBeizerCurve = target as CubicBeizerCurve;
            transform = cubicBeizerCurve.transform;

            // Cache a reference to the current handle rotation. If we are using local rotation, 
            // this will be the transform rotation. If we are using world rotation, this will 
            // be the default identity value.
            handleRotation = (Tools.pivotRotation == PivotRotation.Local) ? 
                transform.rotation : Quaternion.identity;

            // Create a vector array, process the handles, and load the resulting positions into 
            // the array.
            Vector3[] handlePosition = new Vector3[4];
            handlePosition[0] = ShowHandle(0);
            handlePosition[1] = ShowHandle(1);
            handlePosition[2] = ShowHandle(2);
            handlePosition[3] = ShowHandle(3);

            // Draw the step lines between the two points. This will represent the initial line 
            // created by directly connecting the three points.
            Handles.color = cubicBeizerCurve.stepColour;
            Handles.DrawLine(handlePosition[0], handlePosition[1]);
            Handles.DrawLine(handlePosition[1], handlePosition[2]);
            Handles.DrawLine(handlePosition[2], handlePosition[3]);

            // Create an initial line start position to represent the start of our Beizer curve, 
            // or the normalised point of (0).
            Vector3 lineStart = cubicBeizerCurve.GetPointOnCurve(0f);

            // Draw the first increment in the Beizer curve.
            Handles.color = cubicBeizerCurve.tangentColour;
            Handles.DrawLine(lineStart, lineStart + cubicBeizerCurve.GetDirection(0f));

            // Cache a local version of our line steps integer
            int lineSteps = cubicBeizerCurve.lineSteps;

            for(int i = 1; i <= cubicBeizerCurve.lineSteps; i++)
            {
                // For each line step,
                // Create a line end position, and return the Beizer curve position found at the 
                // increment specified by the normalised value of this line step.
                Vector3 lineEnd = cubicBeizerCurve.GetPointOnCurve(i / (float)lineSteps);

                // Draw a curve line between the current start and end positions.
                Handles.color = cubicBeizerCurve.lineColour;
                Handles.DrawLine(lineStart, lineEnd);

                // Draw a tangent line to show the acceleration.
                Handles.color = cubicBeizerCurve.tangentColour;
                Handles.DrawLine(lineEnd, lineEnd 
                    + cubicBeizerCurve.GetDirection(i / (float)lineSteps));

                // The end position of this line is now the start position of the next line.
                lineStart = lineEnd;
            }
            
        }

        /// <summary>Draws a handle at the specified point, and manages user translation.</summary>
        /// <returns>The position of the handle, updated to reflect user translation.</returns>
        /// <param name="pointIndex">The index of the point in <see cref="CubicBeizerCurve.points"/> 
        /// to which we are to draw a handle for.</param>
        private Vector3 ShowHandle (int pointIndex)
        {
            // Create a local position to contain the position pointed to by the requested index, 
            // converted to world coordinates.
            Vector3 point = transform.TransformPoint(cubicBeizerCurve.GetPoint(pointIndex));

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
                this.PrepareChange(cubicBeizerCurve, "Move Point - Cubic Beizer Curve");
                cubicBeizerCurve.SetPoint(pointIndex, transform.InverseTransformPoint(point));
            }

            // Return the updated position.
            return point;
        }
        #endif
    }
}