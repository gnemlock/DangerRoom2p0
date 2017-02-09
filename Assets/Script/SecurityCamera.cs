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
		 /// <summary>Indicates wether this <see cref="SecurityCamera"/> is currently feeding 
		 /// information back to its source.</summary>
		 public bool feeding { get; protected set;}
		 /// <summary>Indicates wether this <see cref="SecurityCamera"/> is currently turned on.</summary>
		 public bool active { get; protected set;}
		 /// <summary>The pivot point used for rotation.</summary>
		 [SerializeField] private Transform pivotPoint;
		 /// <summary>The angle at which the camera can detect activity.</summary>
		 public float viewAngle;
		 /// <summary>The distance at which the camera can detect activity.</summary>
		 public float viewDistance;

		 /// <summary>Delegate for events that fire when the camera detects activity (or inactivity).</summary>
		 public delegate void DetectActivityEvent(int cameraId);
		 /// <summary>Occurs when the camera detects any movement and is 
		 /// both <see cref="active"/> and <see cref="feeding"/>.</summary>
		 /// <remarks>This event should fire whenever the kinematic collider representing this 
		 /// <see cref="SecurityCamera"/>s field of vision is triggered.</remarks>
		 public static event DetectActivityEvent DetectActivity;
		 /// <summary>Occurs when the camera detects movement by units hostile to the player and is 
		 /// both <see cref="active"/> and <see cref="feeding"/>.</summary>
		 /// <remarks>This event should fire whenever a <see cref="DetectActivity"/> event fires from a 
		 /// <see cref="GameObject"/> tagged as "Enemy".</remarks>
		 public static event DetectActivityEvent DetectEnemyActivity;
		 /// <summary>Occurs when the camera detects movement by units friendly to the player and is 
		 /// both <see cref="active"/> and <see cref="feeding"/>.</summary>
		 /// <remarks>This event should fire whenever a <see cref="DetectActivity"/> event fires from a 
		 /// <see cref="GameObject"/> that is not tagged as "Player" or "Enemy".</remarks>
		 public static event DetectActivityEvent DetectFriendlyActivity;
		 /// <summary>Occurs when the camera detects movement by the player and is 
		 /// both <see cref="active"/> and <see cref="feeding"/>.</summary>
		 /// <remarks>This event should fire whenever a <see cref="DetectActivity"/> event fires from a 
		 /// <see cref="GameObject"/> tagged as "Player".</remarks>
		 public static event DetectActivityEvent DetectPlayer;
		 /// <summary>Occurs when the camera loses detection of the player and is 
		 /// both <see cref="active"/> and <see cref="feeding"/>.</summary>
		 /// <remarks>This event should fire whenever the kinematic collider representing 
		 /// this <see cref="SecurityCamera"/>s field of vision detects losing a 
		 /// <see cref="GameObject"/> tagged as "Player".</remarks>
		 public static event DetectActivityEvent LosePlayer;

		 void Awake()
		 {
					// Allocate the next available id to this instance, and increment nextID.
					id = nextId++;

		 #if UNITY_EDITOR
					// If we are in the Unity Editor, ensure that our pivot point is linked up.
					if(pivotPoint == null)
					{
							 // If it isn't, output a warning to the debug log.
							 Debug.Log("Warning: Pivot point not set up");
					}
		 #endif

		 }

		 void Update()
		 {
					if (active)
					{
					}
		 }
    
    void OnDestroy()
    {
        Debug.Log("destroy sub");
    }
    

		 void OnTriggerEnter(Collider otherCollider)
		 {
					if (active && feeding)
					{
							 if (DetectActivity != null)
							 {
										DetectActivity (id);
							 }

							 if (otherCollider.CompareTag ("Player"))
							 {
										if (DetectPlayer != null)
										{
												 DetectPlayer (id);
										}
							 } 
							 else if (otherCollider.CompareTag ("Enemy"))
							 {
										if (DetectEnemyActivity != null)
										{
												 DetectEnemyActivity (id);
										}
							 } 
							 else
							 {
										if (DetectFriendlyActivity != null)
										{
												 DetectFriendlyActivity (id);
										}
							 }
					}
		 }

		 void OnTriggerExit(Collider otherCollider)
		 {
					if (active && feeding)
					{
							 if (otherCollider.CompareTag ("Player") && LosePlayer != null)
							 {
										LosePlayer (id);
							 }
					}
		 }

		 /// <summary>Base class for managing camera rotation</summary>
		 public abstract class CameraMovement
		 {
					protected abstract bool Move ();
		 }

		 /// <summary>Represents a camera that does not allow movement.</summary>
		 public class NoCameraMovement : CameraMovement
		 {
					protected override bool Move()
					{
							 return false;
					}
		 }

		 /// <summary>Represents a camera that pans back and fourth between two points.</summary>
		 public class PanningCameraMovement : CameraMovement
		 {
					protected override bool Move()
					{

							 return true;
					}
		 }
}
