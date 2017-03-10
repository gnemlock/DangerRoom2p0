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
    
    public class Line : MonoBehaviour
    {
        #if UNITY_EDITOR
        public Color color = Color.white;
        public bool displayHandles = true;
        #endif
        
        public Vector3 start;
        public Vector3 end;
    }
}


namespace Drawing.Utility
{
    /// <summary>This class holds the tooltips for variables serialized to the inspector.</summary>
    public static class LineTooltips
    {
        #if UNITY_EDITOR

        #endif
    }

    /// <summary>This class provides additional functionality to the editor.</summary>
    [CustomEditor(typeof(Line))] public class LineEditor : Editor
    {
        #if UNITY_EDITOR
        /// <summary>This method will be called to draw the inspector interface for the target 
        /// class.</summary>
        public override void OnInspectorGUI()
        {
            // Explicitly reference the target class as a CaveGenerator, so we have CaveGenerator 
            // specific access.
            Line line = (Line)target;

            // Draw the default inspector, so we still have the default interface.
            DrawDefaultInspector();
        }
        #endif
        
        private void OnSceneGUI()
        {
            // Explicitly reference the target class as a CaveGenerator, so we have CaveGenerator 
            // specific access.
            Line line = target as Line;
            Transform transform = line.transform;
            
            Quaternion rotation = (Tools.pivotRotation == PivotRotation.Local) ? 
                transform.rotation : Quaternion.identity;
            
            Vector3 start = transform.TransformPoint(line.start);
            Vector3 end = transform.TransformPoint(line.end);
            
            Handles.color = line.color;
            Handles.DrawLine(start, end);
            
            if(line.displayHandles)
            {
                EditorGUI.BeginChangeCheck();
            
                Vector3 startHandle = Handles.DoPositionHandle(start, rotation);
            
                if(EditorGUI.EndChangeCheck())
                {
                    PrepareChange(line, "Move Point");
                    line.start = transform.InverseTransformPoint(startHandle);
                }
            
                EditorGUI.BeginChangeCheck();
            
                Vector3 endHandle = Handles.DoPositionHandle(end, rotation);
            
                if(EditorGUI.EndChangeCheck())
                {
                    PrepareChange(line, "Move Point");
                    line.end = transform.InverseTransformPoint(endHandle);
                }
            }
        }
        
        private static void PrepareChange(Line line, string description)
        {
            Undo.RecordObject(line, description = "line change");
            EditorUtility.SetDirty(line);
        }
    }
}