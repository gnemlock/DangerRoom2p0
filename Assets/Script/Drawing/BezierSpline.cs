/* 
 * Created by Matthew F Keating with the help of Catlike Coding Tutorials.
 *  --- http://catlikecoding.com/unity/tutorials/curves-and-splines/ ---
 */

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using Label = Drawing.Utility.BezierSplineLabels;
#endif

namespace Drawing
{
    public class BezierSpline : MonoBehaviour, IBezierInterface
    {
        /// <summary>The points making up this <see cref="BezierSpline"/>.</summary>
        /// <remarks>This array contains points that are used to draw cubic Bezier curves. As such, 
        /// it should contain a minimum of four positions, or groups of three additional positions, 
        /// thereafter. Every curve past the first will use the last position in the previous curve 
        /// as the starting position in the current curve, and if <see cref="loop"/> is set to 
        /// <c>true</c>, the last position will be set to the same value as the first.</remarks>
        [SerializeField] private Vector3[] points;
        /// <summary>The transition modes affecting the points shared between curves in 
        /// <see cref="points"/>.</summary>
        /// <remarks>Transition modes allow us to ensure smoothing between curves. Only the 
        /// beggining and end points require a deliberate <see cref="BezierPointMode"/>, as these 
        /// are the only points where curves should meet. Points in the middle of a curve should 
        /// identify the <see cref="BezierPointMode"/> of the nearest shared point.</remarks>
        [SerializeField] private BezierPointMode[] transitionModes;
        /// <summary>Is this <see cref="BezierSpline"/> a circuit?</summary>
        /// <remarks>If <c>true</c>, this <see cref="BezierSpline"/> will ensure that the last 
        /// position in <see cref="points"/> matches the first, forcing the last cubic Bezier curve 
        /// to return to the start. This variable should only be set via <see cref="Loop"/>, to 
        /// ensure the correct changes are made, internally.</remarks>
        [SerializeField] private bool loop;
        /// <summary>
        /// The end of spline.
        /// </summary>
        [SerializeField] private Vector3 endOfSplinePosition;
        /// <summary>The last <see cref="BezierPointMode"/> </summary>
        [SerializeField] private BezierPointMode endOfSplineTransition;
        
        /// <summary>Provides access to <see cref="loop"/>, ensuring that that <see cref="points"/> 
        /// is correctly adjusted to reflect change.</summary>
        /// <value><c>true</c> if this <see cref="BezierSpline"/> is a circuit; otherwise, <c>false</c>.</value>
        public bool Loop
        {
            get
            {
                // If we are tryin to get Loop, simply return the value of loop.
                return loop;
            }
            set
            {
                // If we are trying to set Loop, set the value to loop.
                loop = value;
                
                if(value == true)
                {
                    // If the user is submitting a value of true, store the last position and 
                    // transition mode, so we can still convert back to a linear spline.
                    endOfSplinePosition = points[points.Length - 1];
                    endOfSplineTransition = transitionModes[transitionModes.Length - 1];
                    
                    // Set the last transition mode to match the first transition mode, as these 
                    // two points are now connecting.
                    transitionModes[transitionModes.Length - 1] = transitionModes[0];
                    
                    // Reset the first point, which will ensure the value is also assigned to the 
                    // last position, and appropriate smoothing is performed.
                    SetPoint(0, points[0]);
                }
                else
                {
                    // Else, the user has submitted a value of false. Reset the last position and 
                    // transition to the original values.
                    points[points.Length - 1] = endOfSplinePosition;
                    transitionModes[transitionModes.Length - 1] = endOfSplineTransition;
                }
            }
        }
        
        /// <summary>Provides access to the length of the <see cref="points"/> array.</summary>
        /// <value>The length of the <see cref="points"/> array.</value>
        public int PointCount
        {
            get
            {
                // If we are trying to get the length, return the length of the points array.
                return points.Length;
            }
        }
        
        /// <summary>Provides access to the length of the <see cref="points"/> array, interpretted 
        /// as individual curves.</summary>
        /// <value>The amount of curves that would be drawn from the current <see cref="points"/> 
        /// array.</value>
        public int CurveCount
        {
            get
            {
                // If we are trying to get the count, return the number of curves.
                // There will be a curve for every three points, not including the first.
                return ((points.Length - 1) / 3);
            }
        }
        
        #if UNITY_EDITOR
        /// <summary>Reset is called by the editor when we instantiate or reset the instance. THIS 
        /// METHOD IS EDITOR ONLY.</summary>
        void Reset()
        {
            // points should be set to an initial Vector3 array of 4, representing the first curve.
            points = new Vector3[] {
                new Vector3(1.0f, 0, 0),
                new Vector3(2.0f, 0, 0),
                new Vector3(3.0f, 0, 0),
                new Vector3(4.0f, 0, 0)
            };
            
            // transitionModes should be set to an initial BezierPointMode array of 2, representing 
            // the beggining and end points of the first curve with a free transition.
            transitionModes = new BezierPointMode[]
            {
                BezierPointMode.Free,
                BezierPointMode.Free
            };
        }
        #endif
        
        /// <summary>Adds an additional curve to this <see cref="BezierSpline"/> , in the form of 
        /// three additional points. Transition mode will be maintained from the previous curve.
        /// </summary>
        public void AddCurve()
        {
            // Grab the value of the current last point, to determine default values.
            Vector3 point = points[points.Length - 1];
            
            // Each additional curve is represented by three additonal points and an additional 
            // transition. Resize the points and transitionModes arrays accordingly.
            System.Array.Resize(ref points, points.Length + 3);
            System.Array.Resize(ref transitionModes, transitionModes.Length + 1);
            
            for(int i = 0; i < 3; i++)
            {
                // For each of the new points we are adding to the points array, move the temporary 
                // point position along the X axis, and set it's values as the next point.
                point.x += 1.0f;
                points[points.Length - (3 - i)] = point;
            }
            
            // Set the new transition as the value of the previous transition.
            transitionModes[transitionModes.Length - 1] 
                = transitionModes[transitionModes.Length - 2];
            
            // EnforceMode on the first point in the new curve.
            EnforceMode(points.Length - 4);
            
            if(loop)
            {
                // If this Bezier spline currently represents a loop, ensure that the last position 
                // matches the first position, that the first transition matches the last 
                // transition, and that we EnforceMode on the first point in the spline to ensure 
                // that the new points are correctly implemented in the circuit.
                points[points.Length - 1] = points[0];
                transitionModes[transitionModes.Length - 1] = transitionModes[0];
                EnforceMode(0);
            }
        }
        
        /// <summary>Enforces the <see cref="BezierPointMode"/> affecting a specified point, 
        /// applying relative smoothing to the neighboring points.</summary>
        /// <param name="pointIndex">The index for the target point in <see cref="points"/>.</param>
        private void EnforceMode(int pointIndex)
        {
            // Convert the point index to a transition index, and store the BezierPointMode 
            // pointed to by this index, locally.
            int modeIndex = ((pointIndex + 1) / 3);
            BezierPointMode mode = transitionModes[modeIndex];
            
            if(mode == BezierPointMode.Free || !loop && (modeIndex == 0 
                || modeIndex == (transitionModes.Length - 1)))
            {
                // If the current mode is set to Free, or if we are looking at the start or end 
                // index while NOT in a looping BezierSpline, we do not need to perform logic.
                return;
            }
            
            // Convert the transition index back to a point index, pointing to the joining point 
            // affected by the transition. We will also store the indices to the left and right 
            // of the middle index, and a boolean to determine wether we are moving the right or 
            // left index. We should not move the selected index, when smoothing. If we have the 
            // middle point selected, we will move the right index.
            int middleIndex = modeIndex * 3;
            int leftIndex = middleIndex - 1;
            int rightIndex = middleIndex + 1;
            bool movingRightIndex = (pointIndex <= middleIndex);
            
            if(leftIndex < 0)
            {
                // If the left index falls out of the array, reset it to the last index 
                // pointing to a non-joining point.
                leftIndex = points.Length - 2;
            }
            
            if(rightIndex >= points.Length)
            {
                // If the right index falls out of the array, reset it to the first index 
                // pointing to a non-joining point.
                rightIndex = 1;
            }
            
            // Store the point referenced by the middle index. We also need to store the differance 
            // between the middle point and the fixed point, so we can use it to apply smoothing 
            // to the other side.
            Vector3 middlePoint = points[middleIndex];
            Vector3 enforcedDelta = movingRightIndex ? 
                (middlePoint - points[leftIndex]) : (middlePoint - points[rightIndex]);
            
            if(mode == BezierPointMode.Aligned)
            {
                // If we are aligning the vectors, we need to work out the distance between the 
                // middle point and the point we are moving.
                float vectorDistance = movingRightIndex ? 
                    Vector3.Distance(middlePoint, points[rightIndex]) :
                    Vector3.Distance(middlePoint, points[leftIndex]);
                
                // As we are only aligning the vectors, instead of mirroring them, we need to 
                // use the current tangent as a direction, and multiply it by our determined 
                // distance.
                enforcedDelta = enforcedDelta.normalized * vectorDistance;
            }
            
            if(movingRightIndex)
            {
                // If we are moving the right index, move the right index to the position 
                // determined by the middle point and the derived tangent.
                points[rightIndex] = middlePoint + enforcedDelta;
            }
            else
            {
                // Else, we are moving the left index; move the left index to the position 
                // determined by the middle point and the derived tangent.
                points[leftIndex] = middlePoint + enforcedDelta;
            }
        }
        
        /// <summary>Finds the starting index of a specific curve in the <see cref="points"/> 
        /// array, based off an increment of the entire spline.</summary>
        /// <returns>The index of the determined curve's starting point in the <see cref="points"/> 
        /// array</returns>
        /// <param name="t">The normalised increment of the position for the desired curve, in this 
        /// <see cref="BezierSpline"/>. This value is passed by reference, so it can be internally 
        /// editted to reflect the position as an increment in the actual curve. 0 would represent 
        /// the start of the spline, while 1 would represent the end of the spline.</param>
        private int FindCurveOnSpline(ref float t)
        {
            // Create an integer to store the derived index for the start of our determined curve.
            int i;
            
            if(t >= 1.0f)
            {
                // If the normalised increment is greater or equal to the max value of 1, clamp the 
                // t value to 1, and set our curve index to the starting index of the last curve.
                t = 1.0f;
                i = points.Length - 4;
            }
            else
            {
                // Else, the normaised increment is not greater or equal to the max value. Clamp 
                // the value to between 0 and 1, and multiply it by the number of curves, to 
                // determine where the increment lands in relation to the individual curves.
                t = Mathf.Clamp01(t) * CurveCount;
                
                // The value of t now reflects the curve index, as a whole number, and the 
                // increment within the curve, as the decimal value. We can cast the value to our 
                // index integer, as this will only carry the whole number. We can also remove this 
                // whole number from t, in order to leave the normalised increment inside the curve.
                i = (int)t;
                t -= i;
                
                // Multiple the current curve index by 3, in order to represent the starting index 
                // in the points array.
                i *= 3;
            }
            
            // Return the determined starting index.
            return i;
        }
        
        /// <summary>Finds the specified direction of this <see cref="BezierSpline"/> based off a 
        /// provided increment.</summary>
        /// <returns>The direction of this <see cref="BezierSpline"/> at the specified 
        /// increment, as a normalised vector.</returns>
        /// <param name="t">The normalised increment of the position for the desired direction. 0 
        /// would represent the start of the spline, while 1 would represent the end of the spline.
        /// </param>
        /// <remarks>Calling this method is the same as calling 
        /// <c>GetVelocityOfPointOnCurve(t).normalized</c>.</remarks>
        public Vector3 GetDirectionOfPointOnCurve(float t)
        {
            // Find the velocity at the specified increment, and return the normalised value.
            return GetVelocityOfPointOnCurve(t).normalized;
        }
        
        /// <summary>Gets a point in the <see cref="points"/> array.</summary>
        /// <returns>The position pointed to by <c>index</c>, in the <see cref="points"/> 
        /// array.</returns>
        /// <param name="pointIndex">The index of the required point in the <see cref="points"/> 
        /// array.</param>
        public Vector3 GetPoint(int pointIndex)
        {
            // Return the Vector3 found at the specified index in points.
            return points[pointIndex];
        }
        
        /// <summary>Finds the specified position on this <see cref="BezierSpline"/> based off a 
        /// provided increment.</summary>
        /// <returns>The position on this <see cref="BezierSpline"/> at the specified increment.
        /// </returns>
        /// <param name="t">The normalised increment of the desired position. 0 would represent the 
        /// start of the spline, while 1 would represent the end of the spline.</param>
        public Vector3 GetPointOnCurve(float t)
        {
            // Find the index of the specific curve we should be on, providing direct reference to 
            // our t value so that it can be edited to reflect the normalised increment of the 
            // internal curve.
            int i = FindCurveOnSpline(ref t);
            
            // Return the position specified by a cubic Bezier function, passing in the determined 
            // points and derived normalised increment.
            return transform.TransformPoint(BezierUtility
                .GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
        }
        
        /// <summary>Finds the specified velocity of this <see cref="BezierSpline"/> based off a 
        /// provided increment.</summary>
        /// <returns>The velocity of this <see cref="BezierSpline"/> at the specified 
        /// increment.</returns>
        /// <param name="t">The normalised increment of the position for the desired velocity. 0 
        /// would represent the start of the spline, while 1 would represent the end of the spline.
        /// </param>
        public Vector3 GetVelocityOfPointOnCurve(float t)
        {
            // Find the index of the specific curve we should be on, providing direct reference to 
            // our t value so that it can be edited to reflect the normalised increment of the 
            // internal curve.
            int i = FindCurveOnSpline(ref t);
            
            // Return the velocity specified by the first derivative of a cubic Bezier function, 
            // passing in the determined points and derived normalised increment.
            return transform.TransformPoint(BezierUtility
                .GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t));
        }
        
        /// <summary>Gets the <see cref="BezierPointMode"/> set to the specified point.</summary>
        /// <returns>The <see cref="BezierPointMode"/> setting set to the specified point.</returns>
        /// <param name="pointIndex">The index for the target point in the <see cref="points"/> 
        /// array.</param>
        /// <remarks>Only the first and last point in each curve has a corresponding mode. Points 
        /// that represent the middle of a curve will retrieve the mode of the nearest start or end 
        /// point, representing the mode that affects the specified point.</remarks>
        public BezierPointMode GetPointMode(int pointIndex)
        {
            // We move to the next transition mode after the first two points, and the next three, 
            // thereafter. We can use this to convert the point index to a transition index, 
            // knowing that all values will automatically round down, in an integer.
            // Using this, return the transition mode, given the specified index in points.
            return transitionModes[(pointIndex + 1) / 3];
        }
        
        public void SetPoint(int pointIndex, Vector3 newPoint)
        {
            if(pointIndex % 3 == 0)
            {
                Vector3 pointDelta = newPoint - points[pointIndex];
                
                if(loop)
                {
                    if(pointIndex == 0)
                    {
                        points[1] += pointDelta;
                        points[points.Length - 2] += pointDelta;
                        points[points.Length - 1] = newPoint;
                    }
                    else if(pointIndex == (points.Length - 1))
                    {
                        points[0] = newPoint;
                        points[1] += pointDelta;
                        points[pointIndex - 1] += pointDelta;
                    }
                    else
                    {
                        points[pointIndex - 1] += pointDelta;
                        points[pointIndex + 1] += pointDelta;
                    }
                }
                else
                {
                    if(pointIndex > 0)
                    {
                        points[pointIndex - 1] += pointDelta;
                    }
                
                    if((pointIndex + 1) < points.Length)
                    {
                        points[pointIndex + 1] += pointDelta;
                    }
                }
            }
            
            points[pointIndex] = newPoint;
            EnforceMode(pointIndex);
        }
        
        /// <summary>Sets the <see cref="BezierPointMode"/> affecting a specified point in the 
        /// points array. This will have an effect on the nearest joining point, and the point on 
        /// either side.</summary>
        /// <param name="pointIndex">The index for the specific point ,in the <see cref="points"/> 
        /// array, of which we are changing the affecting <see cref="BezierPointMode"/>.</param>
        /// <param name="newMode">The new <see cref="BezierPointMode"/>.</param>
        public void SetPointMode(int pointIndex, BezierPointMode newMode)
        {
            // Convert the index to point to the corresponding position in the transitionsModes
            // array and set that position to the new mode.
            int modeIndex = (pointIndex + 1) / 3;
            transitionModes[modeIndex] = newMode;
            
            if(loop)
            {
                // If this BezierSpline is a loop,
                if(modeIndex == 0)
                {
                    // If we are changing the first transition index, also change the last index.
                    transitionModes[transitionModes.Length - 1] = newMode;
                }
                else if(modeIndex == (transitionModes.Length - 1))
                {
                    // Else, if we are changing the last index, also change the first index.
                    transitionModes[0] = newMode;
                }
            }
            
            // EnforceMode at the changed index, to ensure the changes are reflected.
            EnforceMode(pointIndex);
        }
    }
}

namespace Drawing.Utility
{
    /// <summary>This class holds the tooltips for variables serialized to the inspector.</summary>
    public static class BezierSplineLabels
    {
        #if UNITY_EDITOR
        public const string pointCount = "Point Count: ";
        public const string selectedPosition = "Selected Position:";
        public const string movePoint = "Move Point";
        public const string moveBezierCurve = movePoint + " - Cubic Bezier Curve";
        public const string addCurve = "Add Curve";
        public const string addCurveDescription = "Adding Curve to Spline";
        public const string mode = "Mode";
        public const string changeModeDescription = "Changing Point Mode in Spline";
        public const string loop = "Loop";
        public const string toggleLoopDescription = "Toggled Loop in Spline";
        #endif
    }
    
    /// <summary>This class provides additional functionality to the editor.</summary>
    [CustomEditor(typeof(BezierSpline))] public class BezierSplineEditor : Editor
    {
        #if UNITY_EDITOR
        /// <summary>Represents the size of each handle marker, used to mark handles for unselected 
        /// curves.</summary>
        private const float handleSize = 0.04f;
        /// <summary>Represents the size of the collider over each handle marker, used to determine 
        /// interaction for selecting unselected curbes.</summary>
        private const float triggerSize = 0.06f;
        private const float tangentLength = 0.5f;
        private const bool showTangents = true;
        private const int lineSteps = 10;
        
        /// <summary>Represents the currently selected index in the target points array.</summary>
        private int selectedIndex = -1;
        /// <summary>Cached reference to the target <see cref="BezierSpline"/>.</summary>
        private BezierSpline bezierSpline;
        /// <summary>Cached reference to the target <see cref="Transform"/>.</summary>
        private Transform transform;
        /// <summary>Cached reference to the intended handle rotation.</summary>
        private Quaternion handleRotation;
        
        private static Color[] modeColours =
        {
            Color.white,
            Color.yellow,
            Color.cyan
        };

        /// <summary>This method will be called to draw the inspector interface for the target 
        /// class.</summary>
        public override void OnInspectorGUI()
        {
            // Explicitly reference the target class as a BezierSpline, so we have 
            // BezierSpline specific access.
            bezierSpline = target as BezierSpline;
            
            EditorGUI.BeginChangeCheck();
            
            bool loop = EditorGUILayout.Toggle(Label.loop, bezierSpline.Loop);
            GUILayout.Label(Label.pointCount + bezierSpline.PointCount);
            
            if(EditorGUI.EndChangeCheck())
            {
                this.PrepareChange(bezierSpline, Label.toggleLoopDescription);
                bezierSpline.Loop = loop;
            }

            if(selectedIndex >= 0 && selectedIndex < bezierSpline.PointCount)
            {
                DrawSelectedPointToInspector();
            }
            
            if(GUILayout.Button(Label.addCurve))
            {
                // Add a button called "Add Curve", and when it is pressed, 
                // Prepare the target Bezier spline for change, and add a curve to it.
                this.PrepareChange(bezierSpline, Label.addCurveDescription);
                bezierSpline.AddCurve();
            }
        }

        /// <summary>This method will be called to draw the <see cref="BezierSpline"/>
        /// in to the scene view.</summary>
        private void OnSceneGUI()
        {
            // Explicitly reference the target class as a BezierSpline, so we have BezierSpline 
            // specific access, and create a local reference to the transform.
            bezierSpline = target as BezierSpline;
            transform = bezierSpline.transform;

            // Cache a reference to the current handle rotation. If we are using local rotation, 
            // this will be the transform rotation. If we are using world rotation, this will 
            // be the default identity value.
            handleRotation = (Tools.pivotRotation == PivotRotation.Local) ? 
                transform.rotation : Quaternion.identity;
            
            // Create a vector array to represent the position of each handle.
            // We only need 4 positions, as we only iterate through 4 handles at a time.
            // Set the first position, so we can iterate through the rest more easily.
            Vector3[] handlePosition = new Vector3[4];
            handlePosition[0] = ShowHandle(0);
            
            // Set the handle colour in preparation for drawing lines between main points
            Handles.color = Color.white;
            
            // For each curve set in the points array, 
            for(int i = 1; i < bezierSpline.PointCount; i += 3)
            {
                // For each point in the current curve, 
                for(int j = 0; j < 3; j++)
                {
                    // process each handle, and load the resulting positions into the array.
                    handlePosition[j + 1] = ShowHandle(i + j);
                    
                    // Draw a line between the two handle positions.
                    Handles.DrawLine(handlePosition[j], handlePosition[j + 1]);
                }
                
                // Draw a bezier curve between the current 4 positions.
                Handles.DrawBezier(handlePosition[0], handlePosition[3], handlePosition[1], 
                    handlePosition[2], Color.red, null, 2.0f);
                
                // Set the first position of the next curve as the last position of this curve, 
                // so the curves connect.
                handlePosition[0] = handlePosition[3];
            }
            
            if(true)
            {
                // If we are to display tangents, display direction tangents.
                ShowDirections();
            }
        }
        
        private void DrawSelectedPointToInspector()
        {
            EditorGUI.BeginChangeCheck();
            
            Vector3 point = EditorGUILayout
                .Vector3Field(Label.selectedPosition, bezierSpline.GetPoint(selectedIndex));
            
            if(EditorGUI.EndChangeCheck())
            {
                this.PrepareChange(bezierSpline, Label.movePoint);
                bezierSpline.SetPoint(selectedIndex, point);
            }
            
            EditorGUI.BeginChangeCheck();
            
            BezierPointMode mode = (BezierPointMode)EditorGUILayout
                .EnumPopup(Label.mode, bezierSpline.GetPointMode(selectedIndex));
            
            if(EditorGUI.EndChangeCheck())
            {
                this.PrepareChange(bezierSpline, Label.changeModeDescription);
                bezierSpline.SetPointMode(selectedIndex, mode);
            }
        }

        /// <summary>Manages the display of directional tangents off of the curve.</summary>
        private void ShowDirections()
        {
            // Find the starting position of the curve.
            Vector3 lineStart = bezierSpline.GetPointOnCurve(0f);

            // Draw the first tangent in the Bezier curve.
            Handles.color = Color.green;
            Handles.DrawLine(lineStart, lineStart 
                + (bezierSpline.GetDirectionOfPointOnCurve(0f) * tangentLength));

            // Cache a local version of our line steps integer, multiplied by our curve count. This 
            // now represents the total line count across the entire spline.
            int trueLineSteps = lineSteps * bezierSpline.CurveCount;

            for(int i = 1; i <= trueLineSteps; i++)
            {
                // For each increment in our curve, 
                // Cache a local reference to our current normalised increment.
                float t = i / (float)lineSteps;

                // Get the next point, on the curve, to draw our tangent from.
                lineStart = bezierSpline.GetPointOnCurve(t / (float)trueLineSteps);

                // Draw out the tangent from the derived position.
                Handles.DrawLine(lineStart, lineStart + (bezierSpline
                    .GetDirectionOfPointOnCurve(t / (float)trueLineSteps) * tangentLength));
            }
        }
        
        /// <summary>Draws a handle at the specified point, and manages user translation.</summary>
        /// <returns>The position of the handle, updated to reflect user translation.</returns>
        /// <param name="pointIndex">The index of the point in <see cref="bezierSpline.points"/> 
        /// to which we are to draw a handle for.</param>
        private Vector3 ShowHandle (int pointIndex)
        {
            // Create a local position to contain the position pointed to by the requested index, 
            // converted to world coordinates, and prepare the Handle gizmo to draw point markers.
            Vector3 point = transform.TransformPoint(bezierSpline.GetPoint(pointIndex));
            Handles.color = modeColours[(int)bezierSpline.GetPointMode(pointIndex)];
            
            // Determine the current handle size, to work out the correct screen scale to use on 
            // our handle button.
            float trueHandleSize = HandleUtility.GetHandleSize(point);
            
            if(pointIndex == 0)
            {
                trueHandleSize *= 2.0f;
            }
            
            if(Handles.Button(point, handleRotation, handleSize * trueHandleSize, 
                triggerSize * trueHandleSize, Handles.DotCap))
            {
                // We are currently selecting the target point index, in the array, and it's curve.
                selectedIndex = pointIndex;
                
                // Re-draw the inspector.
                Repaint();
            }
            
            if(selectedIndex == pointIndex)
            {
                // If the current index is the selected index, we should draw the curve.
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
                    this.PrepareChange(bezierSpline, BezierSplineLabels.moveBezierCurve);
                    bezierSpline.SetPoint(pointIndex, transform.InverseTransformPoint(point));
                }
            }

            // Return the updated position.
            return point;
        }
        #endif
    }
}
