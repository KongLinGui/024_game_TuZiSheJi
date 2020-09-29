using UnityEngine;
using System.Collections;

public class BackgroundTree : MonoBehaviour 
{
	private float accumulator = 0.0f;
	
	// The speed (in seconds) at which a new target scale is determined. Bigger scaleSpeed means slower movement.
	public float scaleSpeed = 0.5f;
	// The maximum distance between the transform and the new target scale 
	public float scaleDistance = 0.01f;
	
	// The rotation speed (in seconds). Bigger rotation speed means faster movement.
	public float rotationSpeed = 1f;
	// The rotation amplitude (in degrees).
	public float rotationAmplitude = 3f;
	
	private Vector3 scaleTarget;
	private Quaternion rotationTarget;	
	
	

	void Start () 
	{
		// Initialize the targets
		scaleTarget = WiggleScale( );
		rotationTarget = WiggleRotate();	
	}
	
	void Update () 
	{
		// Every scaleSpeed, a new scale target is determined.
		accumulator += Time.deltaTime;
		if(accumulator >= scaleSpeed)
		{
			scaleTarget = WiggleScale();			
			accumulator -= scaleSpeed;
		}
				
		// the local scale is lerped towards the target scale		
		var norm = Time.deltaTime/scaleSpeed;		
		Vector3 newLocalScale=Vector3.Lerp(transform.localScale, scaleTarget, norm);		
		transform.localScale = newLocalScale;		
		
		// the transform rotation is rotated towards the target rotation
		var normRotation = Time.deltaTime*rotationSpeed;
		transform.rotation = Quaternion.RotateTowards( transform.rotation, rotationTarget , normRotation );
		if(transform.rotation == rotationTarget)
		{			
			rotationTarget = WiggleRotate();
		}
		
	}
	
	private Vector3 WiggleScale()
	{
		// Determines a new scale (only on x and y axis)
		return new Vector3((1 + Random.Range(-scaleDistance,scaleDistance)),(1 + Random.Range(-scaleDistance,scaleDistance)),1);
	}
	
	private Quaternion WiggleRotate()
	{
		// Determines a new angle (only on the z axis)
		return Quaternion.Euler(0f, 0f, Random.Range(-rotationAmplitude,rotationAmplitude));
	}
	
}
