/* 
 * Created by Matthew F Keating
 */

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using Label = LevelGeneration.Utility.CircuitGeneratorLabels;
#endif

namespace LevelGeneration
{
    
    public class CircuitGenerator : MonoBehaviour
    {
        [SerializeField] private int waypointCount = 10;
        [SerializeField] private int maxDistance = 100;
        [SerializeField] private int minDistance = 10;
        [SerializeField] private string seed;
        
        public bool usingRandomSeed = true;
        
        #if UNITY_EDITOR
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private float gizmoRadius = 0.8f;
        
        [HideInInspector] public bool showSpread = false;
        #endif
        
        public Vector3[] waypoints;
        
        /// <summary>Assigns a value to <see cref="seed"/> based off the current 
        /// <see cref="Time.time"/> value.</summary>
        private void AssignRandomSeed()
        {
            // Set seed to the string value of the current time since level load, in seconds.
            // This will change so quickly that it imitates a random number to the extent we need.
            seed = Time.time.ToString();
        }
        
        void GenerateCircuit()
        {
            if(usingRandomSeed)
            {
                AssignRandomSeed();
            }
            
            waypoints = new Vector3[waypointCount];
            
            System.Random numberGenerator = new System.Random(seed.GetHashCode());
            
            for(int i = 0; i < waypointCount; i++)
            {
                transform.Rotate(new Vector3(0f, 360.0f/ (float)(waypointCount + 1)));
                float distance = (float)numberGenerator.Next(minDistance, maxDistance);
                
                waypoints[i] = (transform.position + (transform.forward * distance));
            }
            
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            SendToSpline();
        }
        
        #if UNITY_EDITOR
        /// <summary>This method will be called when we draw gizmos.</summary>
        /// <remarks>This method has been marked for exclusive use with the editor, and thus 
        /// will not be available in the published game.</remarks>
        void OnDrawGizmos()
        {
            if(showGizmos && waypoints != null)
            {
                Gizmos.color = Color.red;
                
                for(int i = 0; i < waypoints.Length; i++)
                {
                    
                    if(i != (waypoints.Length - 1))
                    {
                        Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
                    }
                    else
                    {
                        Gizmos.DrawLine(waypoints[i], waypoints[0]);
                    }
                    
                    if(showSpread && (transform.position != waypoints[i]))
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawLine(transform.position, waypoints[i]);
                    }
                }
                
                Gizmos.color = Color.blue;
                
                for(int i = 0; i < waypoints.Length; i++)
                {
                    Gizmos.DrawSphere(waypoints[i], gizmoRadius);
                }
            }
        }
        
        public void EditorGenerateCircuit()
        {
            GenerateCircuit();
        }
        
        public void SendToSpline()
        {
            Drawing.BezierSpline bezierSpline = GetComponent<Drawing.BezierSpline>();
        }
        #endif
    }
}


namespace LevelGeneration.Utility
{
    /// <summary>This class holds the tooltips for variables serialized to the inspector.</summary>
    public static class CircuitGeneratorLabels
    {
        #if UNITY_EDITOR
        #endif
    }

    /// <summary>This class provides additional functionality to the editor.</summary>
    [CustomEditor(typeof(CircuitGenerator))] public class CircuitGeneratorInspector : Editor
    {
        #if UNITY_EDITOR
        /// <summary>This method will be called to draw the inspector interface for the target 
        /// class.</summary>
        public override void OnInspectorGUI()
        {
            // Explicitly reference the target class as a CaveGenerator, so we have CaveGenerator 
            // specific access.
            CircuitGenerator circuitGenerator = target as CircuitGenerator;

            // Draw the default inspector, so we still have the default interface.
            DrawDefaultInspector();
            
            if(GUILayout.Button("Generate Circuit"))
            {
                circuitGenerator.EditorGenerateCircuit();
            }
            
            if(GUILayout.Button("Spline"))
            {
                circuitGenerator.SendToSpline();
            }
        }   
        
        /*private void OnSceneGUI()
        {
            // Explicitly reference the target class as a CubicBeizerCurve, so we have CubicBeizerCurve 
            // specific access, and create a local reference to the transform.
            CircuitGenerator circuitGenerator = (CircuitGenerator)target;

            int handleCount = circuitGenerator.waypoints.Length;
            Vector3[] handlePosition = new Vector3[handleCount];

            for(int i = 0; i < handleCount; i++)
            {
                
                Vector3 point = circuitGenerator.waypoints[i];
                
                // Perform a BeginChangeCheck so we can tell if the position of the handle changes, 
                // through user translation.
                EditorGUI.BeginChangeCheck();

                // Create a handle at the determined point, using the current rotation, and update 
                // point to reflect any new changes caused by user translation.
                point = Handles.DoPositionHandle(point, Quaternion.identity);
                
                if(EditorGUI.EndChangeCheck())
                {
                    // If the editor detected change, i.e. the user translated the handle via scene view, 
                    // Record a change to the inspector and update the original position reference in 
                    // the actual curve to reflect the new position in local coordinates.
                    PrepareChange(circuitGenerator, "Moving Waypoint" + (i + 1));
                    circuitGenerator.waypoints[i] = point;
                }
            }
        }*/
        
        public void PrepareChange(CircuitGenerator cq, string name)
        {
            // Record the current object, using the desired description, and mark it as dirty.
            Undo.RecordObject(cq, name);
            EditorUtility.SetDirty(cq);
        }
        #endif
    }
}