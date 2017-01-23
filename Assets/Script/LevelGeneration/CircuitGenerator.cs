/* 
 * Created by Matthew F Keating
 */

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LevelGeneration
{
    #if UNITY_EDITOR
    using Tooltips = Utility.CircuitGeneratorTooltips;
    #endif
    
    public class CircuitGenerator : MonoBehaviour
    {
        [SerializeField] private int waypointCount = 10;
        [SerializeField] private int maxDistance = 100;
        [SerializeField] private int minDistance = 10;
        [SerializeField] private string seed;
        
        #if UNITY_EDITOR
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private float gizmoRadius = 0.8f;
        
        [HideInInspector] public bool showSpread = false;
        #endif
        
        public Vector3[] waypoints;
        
        void GenerateCircuit()
        {
            waypoints = new Vector3[waypointCount];
            
            System.Random numberGenerator = new System.Random(seed.GetHashCode());
            
            for(int i = 0; i < waypointCount; i++)
            {
                transform.Rotate(new Vector3(0f, 360.0f/ (float)(waypointCount + 1)));
                float distance = (float)numberGenerator.Next(minDistance, maxDistance);
                
                waypoints[i] = (transform.position + (transform.forward * distance));
            }
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
        #endif
    }
}


namespace LevelGeneration.Utility
{
    /// <summary>This class holds the tooltips for variables serialized to the inspector.</summary>
    public static class CircuitGeneratorTooltips
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
            CircuitGenerator circuitGenerator = (CircuitGenerator)target;

            // Draw the default inspector, so we still have the default interface.
            DrawDefaultInspector();
            
            if(GUILayout.Button("Generate Circuit"))
            {
                circuitGenerator.EditorGenerateCircuit();
            }
        }
        #endif
    }
}