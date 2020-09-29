using UnityEngine;
using System.Collections;

public class Buzzsaw : MonoBehaviour 
{
	private float rotationSpeed = 100f;

	void Start () 
	{
	
	}
	
	void Update () 
	{
		transform.Rotate(rotationSpeed*Vector3.forward*Time.deltaTime);
	}
}
