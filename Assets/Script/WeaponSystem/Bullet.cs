using UnityEngine;
using System.Collections;

public class Bullet : Projectile 
{
	/// <summary>Bullet damage.</summary>
	public double DAMAGE;
	/// <summary>Prefab link for bullet.</summary>
	public GameObject GAMEOBJECT;
	/// <summary>Bullet maximum range.</summary>
	public float RANGE;


	void Update()
	{
		if(Input.GetKeyDown (KeyCode.Space))
		{
			Fire ();
			Debug.Log ("Fire");
		}
	}

	bool CheckHit(Vector3 position, Vector3 forward)
	{
		return Physics.Raycast (position, forward, RANGE);
	}
	
	public void Fire()
	{
		Debug.Log (CheckHit (transform.position, transform.forward));
	}
}
