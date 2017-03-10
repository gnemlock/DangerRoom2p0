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
    using Tooltips = Utility.CubicBezierCurveTooltips;
    #endif
    
    /// <summary>Represents a cubic Bezier curve; a Bezier curve that takes three coordinates.
    /// </summary>
    public class CubicBezierCurve : QuadraticBezierCurve
    {
        /// <summary>The fourth point in this Bezier curve.</summary>
        [Tooltip(Tooltips.pointFour)][SerializeField] protected Vector3 pointFour;
        
        public bool Loop { get { return false; } }
        
        #if UNITY_EDITOR
        /// <summary>This method is called when we instantiate or reset an instance of 
        /// <see cref="Drawing.CubicBezierCurve"/>  in the editor. THIS METHOD IS EDITOR ONLY.
        /// </summary>
        protected override void Reset()
        {
            // Set the fourth point to the default value, and use the base method to reset the rest.
            pointFour = new Vector3(4.0f, 0f, 0f);
            base.Reset();
        }
        #endif
        
        /// <summary>Gets one of the four points in this <see cref="CubicBezierCurve"/>,
        /// using an index.</summary>
        /// <returns>The first, second, third or fourth point.</returns>
        /// <param name="pointIndex">The index representation of the desired point. <c>0</c> will 
        /// return <see cref="pointOne"/>, <c>1</c> will return <see cref="pointTwo"/>, <c>3</c> will 
        /// return <see cref="pointFour"/> and all other values will return 
        /// <see cref="pointThree"/>.</param>
        public override Vector3 GetPoint(int pointIndex)
        {
            if(pointIndex == 3)
            {
                // If the point index is 3, return the fourth point.
                return pointFour;
            }
            else
            {
                // Else, we can defer to the base method for the correct point.
                return base.GetPoint(pointIndex);
            }
        }
        
        /// <summary>Finds the coordinates of a specified point on this 
        /// <see cref="CubicBezierCurve"/>.</summary>
        /// <returns>The coordinates of the point.</returns>
        /// <param name="t">The specific point, defined as a normalised interpolant.
        /// This value will be clamped to 0 and 1.</param>
        public override Vector3 GetPointOnCurve(float t)
        {
            // Find the point, and convert it to world coordinates before returning.
            return transform.TransformPoint(
                BezierUtility.GetPoint(pointOne, pointTwo, pointThree, pointFour, t));
        }
        
        /// <summary>Finds the velocity of this <see cref="CubicBezierCurve"/>, at a specified point.
        /// </summary>
        /// <returns>The velocity at the specified point.</returns>
        /// <param name="t">The specific point, defined as a normalised interpolant.
        /// This value will be clamped to 0 and 1.</param>
        public override Vector3 GetVelocityOfPointOnCurve(float t)
        {
            // Find the point, and convert it to world coordinates. Velocity is a direction, not 
            // a position, so remove the position offset before returning.
            return transform.TransformPoint(
                BezierUtility.GetFirstDerivative(pointOne, pointTwo, pointThree, pointFour, t))
                - transform.position;
        }
        
        /// <summary>Sets the specified point in this <see cref="QuadraticBezierCurve"/>.</summary>
        /// <param name="pointIndex">The index of the point being set. <see cref="pointOne"/> will 
        /// be set with a value of <c>0</c>, <see cref="pointTwo"/> will be set with a value of 
        /// <c>1</c>, <see cref="pointThree"/> will be set with a value of <c>2</c> and 
        /// <see cref="pointFour"/> will be set with a value of <c>3</c>.</param>
        /// <param name="newPoint">The new value for the desired point.</param>
        public override void SetPoint(int pointIndex, Vector3 newPoint)
        {
            if(pointIndex == 3)
            {
                // If the index is 3, set the fourth point as the value of the new point.
                pointFour = newPoint;
            }
            else
            {
                // Else, we can defer to the base method to set the correct point.
                base.SetPoint(pointIndex, newPoint);
            }
        }
    }
}

namespace Drawing.Utility
{
    /// <summary>This class holds the tooltips for variables serialised to the inspector.</summary>
    public static class CubicBezierCurveTooltips
    {
        #if UNITY_EDITOR
        public const string pointFour = "The fourth point.";
        #endif
    }
    
    /// <summary>This class holds the various labels used in drawing GUI to the inspector.</summary>
    public static class CubicBezierCurveLabels
    {
        #if UNITY_EDITOR
        /// <summary>The description given to moving a point in the history window.</summary>
        public const string movePointDescription = "Move Point - Cubic Bezier Curve";
        #endif
    }
    
    /// <summary>This class provides additional functionality to the editor.</summary>
    [CustomEditor(typeof(CubicBezierCurve))] public class CubicBezierCurveEditor : Editor
    {
        #if UNITY_EDITOR
        /// <summary>Cached reference to the target <see cref="CubicBezierCurve"/>.</summary>
        private CubicBezierCurve cubicBezierCurve;
        /// <summary>Cached reference to the target <see cref="Transform"/>.</summary>
        private Transform transform;
        /// <summary>Cached reference to the intended handle rotation.</summary>
        private Quaternion handleRotation;
        
        /// <summary>This method will be called to draw the <see cref="CubicBezierCurve"/>
        /// in to the scene view.</summary>
        private void OnSceneGUI()
        {
            // Explicitly reference the target class as a CubicBezierCurve, so we have CubicBezierCurve 
            // specific access, and create a local reference to the transform.
            cubicBezierCurve = target as CubicBezierCurve;
            transform = cubicBezierCurve.transform;

            // Cache a reference to the current handle rotation. If we are using local rotation, 
            // this will be the transform rotation. If we are using world rotation, this will 
            // be the default identity value.
            handleRotation = (Tools.pivotRotation == PivotRotation.Local) ? 
                transform.rotation : Quaternion.identity;

            // Create a vector array to represent the position of each handle.
            Vector3[] handlePosition = new Vector3[4];
            
            for(int i = 0; i < 4; i++)
            {
                // process each handle, and load the resulting positions into the array.
                handlePosition[i] = ShowHandle(i);
            }

            // Draw the step lines between the two pointsdd. This will represent the initial line 
            // created by directly connecting the three pointsdd.
            Handles.color = QuadraticBezierCurveColours.stepColour;
            Handles.DrawLine(handlePosition[0], handlePosition[1]);
            Handles.DrawLine(handlePosition[1], handlePosition[2]);
            Handles.DrawLine(handlePosition[2], handlePosition[3]);
            
            ShowDirections();
            
            Handles.DrawBezier(handlePosition[0], handlePosition[3], handlePosition[1], 
                handlePosition[2], QuadraticBezierCurveColours.lineColour, null, 2f);
        }
        
        /// <summary>Manages the display of directional tangents off of the curve.</summary>
        private void ShowDirections()
        {
            // Find the starting position of the curve.
            Vector3 lineStart = cubicBezierCurve.GetPointOnCurve(0f);
            
            // Draw the first tangent in the Bezier curve.
            Handles.color = QuadraticBezierCurveColours.tangentColour;
            Handles.DrawLine(lineStart, lineStart 
                + (cubicBezierCurve.GetDirectionOfPointOnCurve(0f) 
                * QuadraticBezierCurveDimensions.tangentLength));
            
            // Cache a local version of our line steps integer
            int lineSteps = QuadraticBezierCurveDimensions.lineSteps;
            
            for(int i = 1; i <= lineSteps; i++)
            {
                // For each increment in our curve, 
                // Cache a local reference to our current normalised increment.
                float t = i / (float)lineSteps;
                
                // Get the next point, on the curve, to draw our tangent from.
                lineStart = cubicBezierCurve.GetPointOnCurve(t);
                
                // Draw out tangent.
                Handles.DrawLine(lineStart, lineStart 
                    + (cubicBezierCurve.GetDirectionOfPointOnCurve(t) 
                    * QuadraticBezierCurveDimensions.tangentLength));
            }
        }
        
        /// <summary>Draws a handle at the specified point, and manages user translation.</summary>
        /// <returns>The position of the handle, updated to reflect user translation.</returns>
        /// <param name="pointIndex">The index of the point in <see cref="CubicBezierCurve.pointsdd"/> 
        /// to which we are to draw a handle for.</param>
        private Vector3 ShowHandle (int pointIndex)
        {
            // Create a local position to contain the position pointed to by the requested index, 
            // converted to world coordinates.
            Vector3 point = transform.TransformPoint(cubicBezierCurve.GetPoint(pointIndex));

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
                this.PrepareChange(cubicBezierCurve, CubicBezierCurveLabels.movePointDescription);
                cubicBezierCurve.SetPoint(pointIndex, transform.InverseTransformPoint(point));
            }

            // Return the updated position.
            return point;
        }
        #endif
    }
}