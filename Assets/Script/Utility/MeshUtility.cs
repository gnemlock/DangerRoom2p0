using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// This is a test script intended to allow 3D editing in Unity. USE AT YOUR OWN RISK. THIS SCRIPT IS CURRENTLY UNTESTED.
/// </summary>
public class MeshUtility : MonoBehaviour 
{
    public Mesh mesh;
    
    public void AttachLocalMesh()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }
    
    public bool CheckIndex(int index)
    {
        if(mesh != null)
        {
            return (index > 0 && index < mesh.vertices.Length);
        }
        else
        {
            return false;
        }
    }
    
    public Vector3 GetPoint(int index)
    {
        if(CheckIndex(index))
        {
            return mesh.vertices[index];
        }
        else
        {
            return Vector3.zero;
        }
    }
    
    public Vector3 SetPoint(int index, Vector3 newPoint)
    {
        if(CheckIndex(index))
        {
            mesh.vertices[index] = newPoint;
        }
    }
}

[CustomEditor(typeof(MeshUtility))]
public class MeshUtilityEditor : Editor
{
    int selectedIndex = -1;
    MeshUtility meshUtility;
    Transform transform;
    Quaternion rotation;
    private float handleSize = 0.04f;
    private float triggerSize = 0.06f;
    
    public override void OnInspectorGUI()
    {
        meshUtility = target as MeshUtility;
        
        if(meshUtility.CheckIndex(selectedIndex))
        {
            DrawSelectedPointToInspector(meshUtility);
        }
    }
    
    
    private void DrawSelectedPointToInspector()
    {
        EditorGUI.BeginChangeCheck();

        Vector3 point = EditorGUILayout
            .Vector3Field("Selected Point", meshUtility.GetPoint(selectedIndex));

        if(EditorGUI.EndChangeCheck())
        {
            this.PrepareChange(meshUtility.mesh, "Moving Point In Mesh");
            meshUtility.SetPoint(selectedIndex, point);
        }
    }
    
    private void OnSceneGUI()
    {
        // Explicitly reference the target class as a BezierSpline, so we have BezierSpline 
        // specific access, and create a local reference to the transform.
        meshUtility = target as MeshUtility;
        transform = meshUtility.transform;

        // Cache a reference to the current handle rotation. If we are using local rotation, 
        // this will be the transform rotation. If we are using world rotation, this will 
        // be the default identity value.
        rotation = (Tools.pivotRotation == PivotRotation.Local) ? 
            transform.rotation : Quaternion.identity;

        // Create a vector array to represent the position of each handle.
        // We only need 4 positions, as we only iterate through 4 handles at a time.
        // Set the first position, so we can iterate through the rest more easily.
        Vector3[] handlePosition = new Vector3[4];
        handlePosition[0] = ShowHandle(0);

        // Set the handle colour in preparation for drawing lines between main points
        Handles.color = Color.white;
        
        int pointCount = meshUtility.mesh.vertices.Length;

        // For each curve set in the points array, 
        for(int i = 0; i < pointCount; i ++)
        {
            ShowHandle(i);
        }
    }
    
    private Vector3 ShowHandle (int pointIndex)
    {
        // Create a local position to contain the position pointed to by the requested index, 
        // converted to world coordinates, and prepare the Handle gizmo to draw point markers.
        Vector3 point = transform.TransformPoint(meshUtility.GetPoint(pointIndex));
        Handles.color = Color.yellow;

        // Determine the current handle size, to work out the correct screen scale to use on 
        // our handle button.
        float trueHandleSize = HandleUtility.GetHandleSize(point);

        if(Handles.Button(point, rotation, handleSize * trueHandleSize, 
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
            point = Handles.DoPositionHandle(point, rotation);

            if(EditorGUI.EndChangeCheck())
            {
                // If the editor detected change, i.e. the user translated the handle via scene view, 
                // Record a change to the inspector and update the original position reference in 
                // the actual curve to reflect the new position in local coordinates.
                this.PrepareChange(Mesh, "Move Point - Cubic Bezier Curve");
                meshUtility.SetPoint(pointIndex, transform.InverseTransformPoint(point));
            }
        }

        // Return the updated position.
        return point;
    }
}
