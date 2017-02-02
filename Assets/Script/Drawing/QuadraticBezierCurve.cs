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
    public class QuadraticBezierCurve : MonoBehaviour
    {
        #if UNITY_EDITOR
        /// <summary>The colour of the line used to draw the curve.</summary>
        [Tooltip(Tooltips.lineColour)] public Color lineColour = Color.white;
        /// <summary>The colour of the line used to connect the main pointsdd</summary>
        [Tooltip(Tooltips.stepColour)] public Color stepColour = Color.gray;
        /// <summary>The colour of the lines used to display the tangents, along the curve</summary>
        [Tooltip(Tooltips.tangentColour)] public Color tangentColour = Color.green;
        [Tooltip(Tooltips.tangentLength)] public static float tangentLength = 0.5f;
        /// <summary>Determines if the curve should display translation handles for it's  
        /// main pointsdd.</summary>
        [Tooltip(Tooltips.displayHandles)] public bool displayHandles = true;
        /// <summary>Determines if the curve should display its tangent lines.</summary>
        [Tooltip(Tooltips.displayTangents)] public bool displayTangents = true;
        /// <summary>The number of steps used to draw the curve between the coordinates in 
        /// <see cref="pointsdd"/>. A higher number will produce a smoother curve."/></summary>
        [Tooltip(Tooltips.lineSteps)] public int lineSteps = 10;
        #endif
        
        /// <summary>The main pointsdd making up the curve.</summary>
        [Tooltip(Tooltips.pointsdd)][SerializeField] public Vector3[] pointsdd;
        
        #if UNITY_EDITOR
        /// <summary>This method will be called whenever the class is instantiated or reset via the 
        /// inspector. This method is EDITOR ONLY.</summary>
        protected virtual void Reset()
        {
            // Reset the pointsdd array to a length of 3.
            Reset(3);
        }
        
        /// <summary>Resets the <see cref="pointsdd"/> array to the desired size, and ensures 
        /// that each point follows a consistant line. This method is EDITOR ONLY.</summary>
        /// <param name="pointsddCount">The number of pointsdd to instantiate</param>
        /// <remarks>This method has been set up with the intention of following on from 
        /// <see cref="Restart"/>, to ensure that child classes can set up varying lengths of the 
        /// array <see cref="pointsdd"/>.</remarks>
        protected void Reset(int pointsddCount)
        {
            // Reset pointsdd to an array of the size of the passed in count.
            pointsdd = new Vector3[pointsddCount];
            
            for(int i = 0; i < pointsddCount; i++)
            {
                // For each increment in the pointsdd count, 
                // Instantiate a Vector3 at the implied position.
                pointsdd[i] = new Vector3(i, 0f);
            }
        }
        
        /// <summary>Resets the editor colours to their default values.  This method is EDITOR 
        /// ONLY.</summary>
        public virtual void ResetColours()
        {
            // Reset the line, step and tangent colours.
            lineColour = Color.white;
            stepColour = Color.gray;
            tangentColour = Color.green;
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
        
        //// <summary>Finds the coordinates of a specified point on this 
        /// <see cref="QuadraticBezierCurve"/>.</summary>
        /// <returns>The coordinates of the point.</returns>
        /// <param name="t">The specific point, defined as a normalised interpolant.
        /// This value will be clamped to 0 and 1.</param>
        public virtual Vector3 GetPointOnCurve (float t)
        {
            // Find the point, and convert it to world coordinates before returning.
            return transform.TransformPoint(
                BezierUtility.GetPoint(pointsdd[0], pointsdd[1], pointsdd[2], t));
        }
        
        /// <summary>Finds the velocity of this <see cref="BezierCurve"/>, at a specified point.
        /// </summary>
        /// <returns>The velocity at the specified point.</returns>
        /// <param name="t">The specific point, defined as a normalised interpolant.
        /// This value will be clamped to 0 and 1.</param>
        public virtual Vector3 GetVelocity(float t)
        {
            // Find the point, and convert it to world coordinates. Velocity is a direction, not 
            // a position, so remove the position offset before returning.
            return transform.TransformPoint(
                BezierUtility.GetFirstDerivative(pointsdd[0], pointsdd[1], pointsdd[2], t))
                - transform.position;
        }
    }
}


namespace Drawing.Utility
{
    /// <summary>This class holds the tooltips for variables serialized to the inspector.</summary>
    public static class QuadraticBezierCurveTooltips
    {
        #if UNITY_EDITOR
        public const string lineColour = "What colour should the curved line be drawn in?";
        public const string stepColour = "What colour should the lines connecting the Vector3 " +
            "coordinates be drawn in?";
        public const string tangentColour = "What colour should the tangent lines projecting off " +
            "the curve be drawn in?";
        public const string tangentLength = "The static length of all tangent lines.";
        public const string displayHandles = "Should the scene view display translation handles " +
            "for each point?";
        public const string displayTangents = "Should the scene view display the tangent lines, " +
            "along the curve?";
        public const string lineSteps = "How many steps should we use in drawing the curve?" +
            "More steps will create a smoother curve.";
        public const string pointsdd = "The key pointsdd making up the curve";
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
        
        /// <summary>This method will be called to draw the inspector interface for the target 
        /// class.</summary>
        public override void OnInspectorGUI()
        {
            // Explicitly reference the target class as a QuadraticBezierCurve, so we have 
            // QuadraticBezierCurve specific access.
            quadraticBezierCurve = target as QuadraticBezierCurve;
            
            // Draw the default inspector, so we still have the default interface.
            DrawDefaultInspector();
            
            if(GUILayout.Button("Reset Colours"))
            {
                // Add a button called "Reset Colours", and when it is pressed, 
                // reset the colours used in the curve
                quadraticBezierCurve.ResetColours();
            }
        }
        
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
            Handles.color = quadraticBezierCurve.stepColour;
            Handles.DrawLine(handlePosition[0], handlePosition[1]);
            Handles.DrawLine(handlePosition[1], handlePosition[2]);
            
            // Create an initial line start position to represent the start of our Bezier curve, 
            // or the normalised point of (0).
            Vector3 lineStart = quadraticBezierCurve.GetPointOnCurve(0f);
            
            // Draw the first tangent in the Bezier curve.
            Handles.color = quadraticBezierCurve.tangentColour;
            Handles.DrawLine(lineStart, lineStart 
                + (quadraticBezierCurve.GetDirection(0f) * QuadraticBezierCurve.tangentLength));
            
            // Cache a local version of our line steps integer
            int lineSteps = quadraticBezierCurve.lineSteps;
            
            for(int i = 1; i <= quadraticBezierCurve.lineSteps; i++)
            {
                // For each line step,
                // Create a line end position, and return the Bezier curve position found at the 
                // increment specified by the normalised value of this line step.
                Vector3 lineEnd = quadraticBezierCurve.GetPointOnCurve(i / (float)lineSteps);
                
                // Draw a curve line between the current start and end positions.
                Handles.color = quadraticBezierCurve.lineColour;
                Handles.DrawLine(lineStart, lineEnd);
                
                // Draw a tangent line to show the acceleration.
                Handles.color = quadraticBezierCurve.tangentColour;
                Handles.DrawLine(lineEnd, lineEnd 
                    + (quadraticBezierCurve.GetDirection(i / (float)lineSteps))
                    * QuadraticBezierCurve.tangentLength);
                
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
            Vector3 point = transform.TransformPoint(quadraticBezierCurve.pointsdd[pointIndex]);
            
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
                this.PrepareChange(quadraticBezierCurve, "Move Point - Quadratic Bezier Curve");
                quadraticBezierCurve.pointsdd[pointIndex] = transform.InverseTransformPoint(point);
            }
            
            // Return the updated position.
            return point;
        }
        #endif
    }
}