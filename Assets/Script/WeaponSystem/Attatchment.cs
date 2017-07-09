using UnityEngine;
using System.Collections;

public class Attatchment : MonoBehaviour 
{
	/// <summary>The attatchments point of anchorage to the weapon.</summary>
	public GameObject anchor;

	/// <summary>Attatches the anchor to a specific coordinate and parents attatchment to related weapon.</summary>
	/// <param name="anchorPosition">Anchor position.</param>
	public void Attatch(Vector3 anchorPosition, Transform weapon)
	{
		anchor.transform.position = anchorPosition;

		anchor.transform.SetParent (weapon);
	}
}


