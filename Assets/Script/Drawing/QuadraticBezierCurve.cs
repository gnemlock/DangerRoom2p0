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
    using Tooltips = Utility.QuadraticBezierCurveTooltips;
    #endif
    
    /// <summary>Represents a quadratic Bezier curve; a Bezier curve that takes four coordinates.
    /// </summary>
    public class QuadraticBezierCurve : MonoBehaviour, IBezierInterface
    {
        /// <summary>The first point in this Bezier curve.</summary>
        [Tooltip(Tooltips.pointOne)][SerializeField] protected Vector3 pointOne;
        /// <summary>The second point in this Bezier curve.</summary>
        [Tooltip(Tooltips.pointTwo)][SerializeField] protected Vector3 pointTwo;
        /// <summary>The third point in this Bezier curve.</summary>
        [Tooltip(Tooltips.pointThree)][SerializeField] protected Vector3 pointThree;
        
        public bool Loop { get { return false; } }
        
        #if UNITY_EDITOR
        /// <summary>This method will be called whenever the class is instantiated or reset via the 
        /// inspector. This method is EDITOR ONLY.</summary>
        protected virtual void Reset()
        {
            // Reset the three points to default consecutive positions.
            Vector3 pointOne = new Vector3(1.0f, 0f, 0f);
            Vector3 pointTwo = new Vector3(2.0f, 0f, 0f); 
            Vector3 pointThree = new Vector3(3.0f, 0f, 0f);
        }
        #endif
        
        /// <summary>Finds the normalised velocity of this curve, at a specified point.</summary>
        /// <remarks>This is the same as calling 
        /// <see cref="GetVelocityOfPointOnCurve().normalized"/>
        /// <returns>The velocity at the specified point, normalised.</returns>
        /// <param name="t">The specific point, defined as a normalised interpolant.
        /// This value will be clamped to 0 and 1.</param>
        public Vector3 GetDirectionOfPointOnCurve(float t)
        {
            // Return the velocity, normalized.
            return GetVelocityOfPointOnCurve(0f).normalized;
        }
        
        /// <summary>Gets one of the three points in this <see cref="QuadraticBezierCurve"/>,
        /// using an index.</summary>
        /// <returns>The first, second or third point.</returns>
        /// <param name="pointIndex">The index representation of the desired point. <c>0</c> will 
        /// return <see cref="pointOne"/>, <c>1</c> will return <see cref="pointTwo"/> and all 
        /// other values will return <see cref="pointThree"/>.</param>
        public virtual Vector3 GetPoint(int pointIndex)
        {
            // Based off the provided index, return the corresponding point.
            switch(pointIndex)
            {
                case 0:
                    return pointOne;
                case 1:
                    return pointTwo;
                default:
                    return pointThree;
            }
        }
        
        //// <summary>Finds the coordinates of a specified point on this 
        /// <see cref="QuadraticBezierCurve"/>.</summary>
        /// <returns>The coordinates of the point.</returns>
        /// <param name="t">The specific point, defined as a normalised interpolant.
        /// This value will be clamped to 0 and 1.</param>
        public virtual Vector3 GetPointOnCurve (float t)
        {
            // Find the point, and convert it to world coordinates before returning.
            return transform.TransformPoint(
                BezierUtility.GetPoint(pointOne, pointTwo, pointThree, t));
        }
        
        /// <summary>Finds the velocity of this <see cref="BezierCurve"/>, at a specified point.
        /// </summary>
        /// <returns>The velocity at the specified point.</returns>
        /// <param name="t">The specific point, defined as a normalised interpolant.
        /// This value will be clamped to 0 and 1.</param>
        public virtual Vector3 GetVelocityOfPointOnCurve(float t)
        {
            // Find the point, and convert it to world coordinates. Velocity is a direction, not 
            // a position, so remove the position offset before returning.
            return transform.TransformPoint(
                BezierUtility.GetFirstDerivative(pointOne, pointTwo, pointThree, t))
                - transform.position;
        }
        
        /// <summary>Sets the specified point in this <see cref="QuadraticBezierCurve"/>.</summary>
        /// <param name="pointIndex">The index of the point being set. <see cref="pointOne"/> will 
        /// be set with a value of <c>0</c>, <see cref="pointTwo"/> will be set with a value of 
        /// <c>1</c> and <see cref="pointThree"/> will be set with a value of <c>2</c>.</param>
        /// <param name="newPoint">The new value for the desired point.</param>
        public virtual void SetPoint(int pointIndex, Vector3 newPoint)
        {
            // Based off the provided index, set the value of the new point to the intended point.
            switch(pointIndex)
            {
                case 0:
                    pointOne = newPoint;
                    break;
                case 1:
                    pointTwo = newPoint;
                    break;
                case 2:
                    pointThree = newPoint;
                    break;
            }
        }
    }
}


namespace Drawing.Utility
{
    /// <summary>This class holds the tooltips for variables serialised to the inspector.</summary>
    public static class QuadraticBezierCurveTooltips
    {
        #if UNITY_EDITOR
        public const string pointOne = "The first point.";
        public const string pointTwo = "The second point.";
        public const string pointThree = "The third point.";
        #endif
    }
    
    /// <summary>This class holds the various labels used in drawing GUI to the inspector.</summary>
    public static class QuadraticBezierCurveLabels
    {
        #if UNITY_EDITOR
        /// <summary>The description given to moving a point in the history window.</summary>
        public const string movePointDescription = "Move Point - Quadratic Bezier Curve";
        #endif
    }
    
    /// <summary>This class holds dimensions for drawing elements to the inspector.</summary>
    public static class QuadraticBezierCurveDimensions
    {
        #if UNITY_EDITOR
        /// <summary>The length of the lines representing velocity along the curve.</summary>
        public const float tangentLength = 0.5f;
        /// <summary>The number of lines involved in drawing the curve. The more line steps, the 
        /// smoother the curve.</summary>
        public const int lineSteps = 10;
        #endif
    }
    
    /// <summary>This class holds colours for drawing elements to the inspector.</summary>
    public static class QuadraticBezierCurveColours
    {
        #if UNITY_EDITOR
        /// <summary>The colour of the lines connecting points in the curve.</summary>
        public static Color stepColour = Color.white;
        /// <summary>The colour of the lines representing velocity along the curve.</summary>
        public static Color tangentColour = Color.green;
        /// <summary>The colour of the curve.</summary>
        public static Color lineColour = Color.red;
        #endif
    }

    /// <summary>This class provides additional functionality to the editor.</summary>
    [CustomEditor(typeof(QuadraticBezierCurve))] public class QuadraticBezierCurveInspector : Editor
    {
        #if UNITY_EDITOR
        /// <summary>Cached reference to the target <see cref="QuadraticBezierCurve"/>.</summary>
        private QuadraticBezierCurve quadraticBezierCurve;
        /// <summary>Cached reference to the target <see cref="Transform"/>.</summary>
        private Transform transform;
        /// <summary>Cached reference to the intended handle rotation.</summary>
        private Quaternion handleRotation;
        
        /// <summary>This method will be called to draw the <see cref="QuadraticBezierCurve"/>
        /// in to the scene view.</summary>
        private void OnSceneGUI()
        {
            // Explicitly reference the target class as a QuadraticBezierCurve, so we have 
            // QuadraticBezierCurve specific access, and create a local reference to the transform.
            quadraticBezierCurve = target as QuadraticBezierCurve;
            transform = quadraticBezierCurve.transform;
            
            // Cache a reference to the current handle rotation. If we are using local rotation, 
            // this will be the transform rotation. If we are using world rotation, this will 
            // be the default identity value.
            handleRotation = (Tools.pivotRotation == PivotRotation.Local) ? 
                transform.rotation : Quaternion.identity;
            
            // Create a vector array to represent the position of each handle.
            Vector3[] handlePosition = new Vector3[3];

            // For each of the handle positions, 
            for(int i = 0; i < 3; i++)
            {
                // process each handle, and load the resulting positions into the array.
                handlePosition[i] = ShowHandle(i);
            }
            
            // Draw the step lines between the two pointsdd. This will represent the initial line 
            // created by directly connecting the three pointsdd.
            Handles.color = QuadraticBezierCurveColours.stepColour;
            Handles.DrawLine(handlePosition[0], handlePosition[1]);
            Handles.DrawLine(handlePosition[1], handlePosition[2]);
            
            // Create an initial line start position to represent the start of our Bezier curve, 
            // or the normalised point of (0).
            Vector3 lineStart = quadraticBezierCurve.GetPointOnCurve(0f);
            
            // Draw the first tangent in the Bezier curve.
            Handles.color = QuadraticBezierCurveColours.tangentColour;
            Handles.DrawLine(lineStart, lineStart 
                + (quadraticBezierCurve.GetDirectionOfPointOnCurve(0f) 
                * QuadraticBezierCurveDimensions.tangentLength));
            
            // Cache a local version of our line steps integer
            int lineSteps = QuadraticBezierCurveDimensions.lineSteps;
            
            for(int i = 1; i <= lineSteps; i++)
            {
                // For each line step,
                // Create a line end position, and return the Bezier curve position found at the 
                // increment specified by the normalised value of this line step.
                Vector3 lineEnd = quadraticBezierCurve.GetPointOnCurve(i / (float)lineSteps);
                
                // Draw a curve line between the current start and end positions.
                Handles.color = QuadraticBezierCurveColours.lineColour;
                Handles.DrawLine(lineStart, lineEnd);
                
                // Draw a tangent line to show the acceleration.
                Handles.color = QuadraticBezierCurveColours.tangentColour;
                Handles.DrawLine(lineEnd, lineEnd 
                    + (quadraticBezierCurve.GetDirectionOfPointOnCurve(i / (float)lineSteps))
                    * QuadraticBezierCurveDimensions.tangentLength);
                
                // The end position of this line is now the start position of the next line.
                lineStart = lineEnd;
            }
        }
        
        /// <summary>Draws a handle at the specified point, and manages user translation.</summary>
        /// <returns>The position of the handle, updated to reflect user translation.</returns>
        /// <param name="pointIndex">The index of the point in <see cref="BezierCurve.pointsdd"/> 
        /// to which we are to draw a handle for.</param>
        private Vector3 ShowHandle (int pointIndex)
        {
            // Create a local position to contain the position pointed to by the requested index, 
            // converted to world coordinates.
            Vector3 point = transform.TransformPoint(quadraticBezierCurve.GetPoint(pointIndex));
            
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
                this.PrepareChange(quadraticBezierCurve, 
                    QuadraticBezierCurveLabels.movePointDescription);
                quadraticBezierCurve.SetPoint(pointIndex, transform.InverseTransformPoint(point));
            }
            
            // Return the updated position.
            return point;
        }
        #endif
    }
}