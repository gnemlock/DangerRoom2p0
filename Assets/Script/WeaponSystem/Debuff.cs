using UnityEngine;
using System.Collections;

public class Debuff : MonoBehaviour 
{

	float timer;
	float frequancy;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.fixedDeltaTime;
	}
}
