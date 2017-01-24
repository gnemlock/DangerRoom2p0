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

    public class CubicBeizerCurve : MonoBehaviour
    {
        #if UNITY_EDITOR
        // <summary>The colour of the line used to draw the curve.</summary>
        [Tooltip(Tooltips.lineColour)] public Color lineColour = Color.white;
        /// <summary>The colour of the line used to connect the four points, along the curve.
        /// </summary>
        [Tooltip(Tooltips.stepColour)] public Color stepColour = Color.gray;
        /// <summary>The colour of the lines used to display the tangents, along the curve</summary>
        [Tooltip(Tooltips.tangentColour)] public Color tangentColour = Color.green;
        /// <summary>Determines if the curve should display translation handles for the 
        /// four points.</summary>
        [Tooltip(Tooltips.displayHandles)] public bool displayHandles = true;
        /// <summary>Determines if the curve should display its tangent lines.</summary>
        [Tooltip(Tooltips.displayTangents)] public bool displayTangents = true;
        /// <summary>The number of steps used to draw the beizer curve between coordinates in 
        /// <see cref="points"/>. A higher number will produce a smoother curve."/></summary>
        [Tooltip(Tooltips.lineSteps)] public int lineSteps = 10;
        #endif

        /// <summary>The points used to draw the curve.</summary>
        [Tooltip(Tooltips.points)] public Vector3[] points;

        #if UNITY_EDITOR
        /// <summary>This method will be called whenever the class is instantiated or reset via the 
        /// inspector. This method is EDITOR ONLY.</summary>
        public void Reset()
        {
            points = new Vector3[] {
                new Vector3(1.0f, 0f, 0f), 
                new Vector3(2.0f, 0f, 0f), 
                new Vector3(3.0f, 0f, 0f),
                new Vector3(4.0f, 0f, 0f)
            };
        }
        #endif

        //// <summary>Finds the coordinates of a specified point on this <see cref="BeizerCurbe"/>
        /// </summary>
        /// <returns>The coordinates of the point.</returns>
        /// <param name="t">The specific point, defined as a normalised interpolant.
        /// This value will be clamped to 0 and 1.</param>
        public Vector3 GetPoint (float t)
        {
            // Find the point, and convert it to world coordinates before returning.
            return transform.TransformPoint(
                BeizerUtility.GetPoint(points[0], points[1], points[2], points[3], t));
        }

        /// <summary>Finds the velocity of this <see cref="CubicBeizerCurve"/>, at a specified point.
        /// </summary>
        /// <returns>The velocity at the specified point.</returns>
        /// <param name="t">The specific point, defined as a normalised interpolant.
        /// This value will be clamped to 0 and 1.</param>
        public Vector3 GetVelocity(float t)
        {
            // Find the point, and convert it to world coordinates. Velocity is a direction, not 
            // a position, so remove the position offset before returning.
            return transform.TransformPoint(
                BeizerUtility.GetFirstDerivative(points[0], points[1], points[2], points[3], t))
                - transform.position;
        }

        /// <summary>Finds the normalised velocity of this <see cref="CubicBeizerCurve"/>, at a 
        /// specified point.</summary>
        /// <remarks>This is the same as calling <see cref="GetVelocity().normalized"/>
        /// <returns>The velocity at the specified point, normalised.</returns>
        /// <param name="t">The specific point, defined as a normalised interpolant.
        /// This value will be clamped to 0 and 1.</param>
        public Vector3 GetDirection(float t)
        {
            // Return the velocity, normalized.
            return GetVelocity(0f).normalized;
        }
    }
}


namespace Drawing.Utility
{
    /// <summary>This class holds the tooltips for variables serialized to the inspector.</summary>
    public static class CubicBeizerCurveTooltips
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
        public const string points = "The four coordinates used for this curve.";
        #endif
    }

    /// <summary>This class provides additional functionality to the editor.</summary>
    [CustomEditor(typeof(CubicBeizerCurve))] public class CubicBeizerCurveInspector : Editor
    {
        #if UNITY_EDITOR
        private CubicBeizerCurve CubicBeizerCurve;
        private Transform transform;
        private Quaternion handleRotation;

        /// <summary>This method will be called to draw the inspector interface for the target 
        /// class.</summary>
        public override void OnInspectorGUI()
        {
            // Explicitly reference the target class as a CaveGenerator, so we have CaveGenerator 
            // specific access.
            CubicBeizerCurve = (CubicBeizerCurve)target;

            // Draw the default inspector, so we still have the default interface.
            DrawDefaultInspector();

            if(GUILayout.Button("Reset Colours"))
            {
                CubicBeizerCurve.lineColour = Color.white;
                CubicBeizerCurve.stepColour = Color.gray;
                CubicBeizerCurve.tangentColour = Color.green;
            }
        }

        private void OnSceneGUI()
        {
            // Explicitly reference the target class as a CubicBeizerCurve, so we have CubicBeizerCurve 
            // specific access, and create a local reference to the transform.
            CubicBeizerCurve = target as CubicBeizerCurve;
            transform = CubicBeizerCurve.transform;

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
            Handles.color = CubicBeizerCurve.stepColour;
            Handles.DrawLine(handlePosition[0], handlePosition[1]);
            Handles.DrawLine(handlePosition[1], handlePosition[2]);
            Handles.DrawLine(handlePosition[2], handlePosition[3]);

            // Create an initial line start position to represent the start of our Beizer curve, 
            // or the normalised point of (0).
            Vector3 lineStart = CubicBeizerCurve.GetPoint(0f);

            Handles.color = CubicBeizerCurve.tangentColour;
            Handles.DrawLine(lineStart, lineStart + CubicBeizerCurve.GetDirection(0f));

            // Cache a local version of our line steps integer
            int lineSteps = CubicBeizerCurve.lineSteps;

            for(int i = 1; i <= CubicBeizerCurve.lineSteps; i++)
            {
                // For each line step,
                // Create a line end position, and return the Beizer curve position found at the 
                // increment specified by the normalised value of this line step.
                Vector3 lineEnd = CubicBeizerCurve.GetPoint(i / (float)lineSteps);

                // Draw a curve line between the current start and end positions.
                Handles.color = CubicBeizerCurve.lineColour;
                Handles.DrawLine(lineStart, lineEnd);

                Handles.color = CubicBeizerCurve.tangentColour;
                Handles.DrawLine(lineEnd, lineEnd + CubicBeizerCurve.GetDirection(i / (float)lineSteps));

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
            Vector3 point = transform.TransformPoint(CubicBeizerCurve.points[pointIndex]);

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
                PrepareChange(CubicBeizerCurve, "Move Point");
                CubicBeizerCurve.points[pointIndex] = transform.InverseTransformPoint(point);
            }

            // Return the updated position.
            return point;
        }

        /// <summary>Prepares the editor for a change. This method records the current state of a 
        /// <see cref="CubicBeizerCurve"/>, so further changes can be reverted with an undo command, 
        /// and marks it as dirty so the inspector will know to ask the user if they want to save 
        /// changes, if Unity is closed.</summary>
        /// <param name="CubicBeizerCurve">The <see cref="CubicBeizerCurve"/> being recorded.</param>
        /// <param name="description">The title of the action we intend to undo if we revert back 
        /// to the recorded state. This will also be displayed in the undo menu.</param>
        private static void PrepareChange(
            CubicBeizerCurve CubicBeizerCurve, string description = "Beizer curve change")
        {
            // Record the current object, using the desired description, and mark it as dirty.
            Undo.RecordObject(CubicBeizerCurve, description = "beizer curve change");
            EditorUtility.SetDirty(CubicBeizerCurve);
        }
        #endif
    }
}