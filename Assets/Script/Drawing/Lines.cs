/* 
 * Created by Matthew F Keating
 */

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Drawing
{
    #if UNITY_EDITOR
    using Tooltips = Utility.LinesTooltips;
    #endif

    //TODO:Get Lines working
    //TODO:Implement OpenGL-style draw options
    public class Lines : MonoBehaviour
    {
        #if UNITY_EDITOR
        public Color color = Color.white;
        public bool circuit = false;
        #endif

        public Vector3[] points;
        
        public Lines(Vector3[] points)
        {
            this.points = points;
        }
    }
}


namespace Drawing.Utility
{
    /// <summary>This class holds the tooltips for variables serialized to the inspector.</summary>
    public static class LinesTooltips
    {
        #if UNITY_EDITOR

        #endif
    }

    /// <summary>This class provides additional functionality to the editor.</summary>
    [CustomEditor(typeof(Lines))] public class LinesInspector : Editor
    {
        #if UNITY_EDITOR
        /// <summary>This method will be called to draw the inspector interface for the target 
        /// class.</summary>
        public override void OnInspectorGUI()
        {
            // Explicitly reference the target class as a CaveGenerator, so we have CaveGenerator 
            // specific access.
            Lines instance = (Lines)target;

            // Draw the default inspector, so we still have the default interface.
            DrawDefaultInspector();
        }

        private void OnSceneGUI()
        {
            // Explicitly reference the target class as a CaveGenerator, so we have CaveGenerator 
            // specific access.
            Lines instance = (Lines)target;

            Handles.color = instance.color;
            //Handles.DrawLines(instance.points);
            
            if(instance.circuit)
            {
                Handles.DrawLine(instance.points[0], instance.points[instance.points.Length - 1]);
            }
        }
        #endif
    }
}