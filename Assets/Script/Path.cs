using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debugging;


/// <summary></summary>
/// TODO: Convert to standalone script for implementation in core character class.
public class Path : MonoBehaviour 
{
		 public Vector3[] waypoint;
		 public float[] distance;
		 public float totalDistance;

		 public virtual void Awake()
		 {
					distance = new float[(waypoint.Length - 2)];
					SetDistances ();
		 }

		 protected virtual void SetDistances()
		 {
					int currentIndex = 0;
					int endIndex = (waypoint.Length - 1);
					totalDistance = 0f;

					while (currentIndex < endIndex)
					{
							 distance[currentIndex] = Vector3.Distance (waypoint[currentIndex], waypoint[currentIndex + 1]);
							 totalDistance += distance [currentIndex++];
					}
		 }
}

public class Circuit : Path
{

		 public override void Awake()
		 {
					distance = new float[(waypoint.Length - 1)];
					SetDistances ();
		 }

		 protected override void SetDistances()
		 {
					int currentIndex = 0;
					int endIndex = (waypoint.Length - 1);
					totalDistance = 0f;

					while (currentIndex < endIndex)
					{
							 distance [currentIndex] = Vector3.Distance (waypoint[currentIndex], waypoint[currentIndex + 1]);
							 totalDistance += distance [currentIndex++];
					}

					distance [endIndex] = Vector3.Distance (waypoint [endIndex], waypoint [0]);
					totalDistance += distance [endIndex];
		 }
}
