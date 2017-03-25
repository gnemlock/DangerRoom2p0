using UnityEngine;

//TODO:The SecurityCamera should allow for multiple movement variations
//TODO:Implement still camera variation
//TODO:Implement basic point to point panning
//TODO:Test trigger events
//TODO:Enable ability to track player
//TODO:Ensure camera can return to previous activity when loosing tracked player
//TODO:Set up constructors
//TODO:Set up deconstructors
//TODO:Set up room reference
//TODO:Set up source reference
//TODO:Set up hijack ability
//TODO:Set up main control for quick access

public class SecurityCamera : MonoBehaviour 
{
    /// <summary>Keeps static reference of the next available id to ensure unique id values.</summary>
    private static int nextId = 0;
    /// <summary>The id reference for this <see cref="SecurityCamera"/>.</summary>
    public int id { get; protected set;}
    public bool active;
    public Transform pivotPoint;
    public Vector3 start, finish;
    public float speed;
    public float increment;
    
    void Awake()
    {
        // Allocate the next available id to this instance, and increment nextID.
        id = nextId++;
        start = pivotPoint.transform.localEulerAngles;
    
    #if UNITY_EDITOR
        if(pivotPoint == null)
        {
            // If we are in the Unity Editor, ensure that our pivot point is linked up.
            // If it isn't, output a warning to the debug log.
            Debug.Log("Warning: Pivot point not set up");
        }
    #endif
    
    }
    
    void Update()
    {
        if (active)
        {
            pivotPoint.transform.Rotate((finish - start) * speed * Time.deltaTime);
        }
    }
}