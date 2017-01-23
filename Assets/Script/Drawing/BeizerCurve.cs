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
    using Tooltips = Utility.LineTooltips;
    #endif

    public class BeizerCurve : MonoBehaviour
    {
        #if UNITY_EDITOR
        public Color lineColor = Color.white;
        public Color stepColor = Color.gray;
        public Color tangentColor = Color.green;
        public bool displayHandles = true;
        public int lineSteps = 10;
        #endif

        public Vector3[] points;
        
        public void Reset()
        {
            points = new Vector3[] {
                new Vector3(1.0f, 0f, 0f), 
                new Vector3(2.0f, 0f, 0f), 
                new Vector3(3.0f, 0f, 0f) 
            };
        }
        
        public Vector3 GetPoint (float normalisedDistance = 0f)
        {
            return transform.TransformPoint(Beizer.GetPoint(points[0], points[1], points[2], normalisedDistance));
        }
        
        public Vector3 GetVelocity(float normalisedDistance = 0f)
        {
            return transform.TransformPoint(Beizer.GetFirstDerivative(points[0], points[1], points[2], normalisedDistance)) - transform.position;
        }
        
        public Vector3 GetDirection(float normalisedDistance = 0f)
        {
            return GetVelocity(normalisedDistance).normalized;
        }
    }
}


namespace Drawing.Utility
{
    /// <summary>This class holds the tooltips for variables serialized to the inspector.</summary>
    public static class BeizerCurveTooltips
    {
        #if UNITY_EDITOR

        #endif
    }

    /// <summary>This class provides additional functionality to the editor.</summary>
    [CustomEditor(typeof(BeizerCurve))] public class BeizerCurveInspector : Editor
    {
        #if UNITY_EDITOR
        private BeizerCurve beizerCurve;
        private Transform transform;
        private Quaternion rotation;
        
        /// <summary>This method will be called to draw the inspector interface for the target 
        /// class.</summary>
        public override void OnInspectorGUI()
        {
            // Explicitly reference the target class as a CaveGenerator, so we have CaveGenerator 
            // specific access.
            beizerCurve = (BeizerCurve)target;

            // Draw the default inspector, so we still have the default interface.
            DrawDefaultInspector();
            
            if(GUILayout.Button("Reset Colours"))
            {
                beizerCurve.lineColor = Color.white;
                beizerCurve.stepColor = Color.gray;
                beizerCurve.tangentColor = Color.green;
            }
        }
        #endif

        private void OnSceneGUI()
        {
            // Explicitly reference the target class as a CaveGenerator, so we have CaveGenerator 
            // specific access.
            beizerCurve = (BeizerCurve)target;
            transform = beizerCurve.transform;

            rotation = (Tools.pivotRotation == PivotRotation.Local) ? 
                transform.rotation : Quaternion.identity;
            
            Vector3[] point = new Vector3[3];
            point[0] = ShowHandle(0);
            point[1] = ShowHandle(1);
            point[2] = ShowHandle(2);
            
            Handles.color = beizerCurve.stepColor;
            Handles.DrawLine(point[0], point[1]);
            Handles.DrawLine(point[1], point[2]);
            
            Vector3 lineStart = beizerCurve.GetPoint();
            
            Handles.color = beizerCurve.tangentColor;
            Handles.DrawLine(lineStart, lineStart + beizerCurve.GetDirection());
            
            int lineSteps = beizerCurve.lineSteps;
            
            for(int i = 1; i <= lineSteps; i++)
            {
                Vector3 lineEnd = beizerCurve.GetPoint(i / (float)lineSteps);
                
                Handles.color = beizerCurve.lineColor;
                Handles.DrawLine(lineStart, lineEnd);
                
                Handles.color = beizerCurve.tangentColor;
                Handles.DrawLine(lineEnd, lineEnd + beizerCurve.GetDirection(i / (float)lineSteps));
                lineStart = lineEnd;
            }
        }
        
        private Vector3 ShowHandle (int pointIndex)
        {
            Vector3 point = transform.TransformPoint(beizerCurve.points[pointIndex]);
            
            EditorGUI.BeginChangeCheck();
            
            point = Handles.DoPositionHandle(point, rotation);
            
            if(EditorGUI.EndChangeCheck())
            {
                PrepareChange(beizerCurve, "Move Point");
                beizerCurve.points[pointIndex] = transform.InverseTransformPoint(point);
            }
            
            return point;
        }

        private static void PrepareChange(BeizerCurve beizerCurve, string description)
        {
            Undo.RecordObject(beizerCurve, description = "beizer curve change");
            EditorUtility.SetDirty(beizerCurve);
        }
    }
}