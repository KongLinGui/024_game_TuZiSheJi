using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	
	Transform target;
	LevelBounds levelBounds;
	
	public float cameraAheadFactor = 3;
	public float cameraReturnSpeed = 0.5f;
	public float cameraThreshold = 0.1f;
	public float cameraSlowFactor = 0.3f;
	public bool cameraFollowsPlayer{get;set;}
	
	private float xMin;
	private float xMax;
	private float yMin;
	private float yMax;	
	
	private float offsetZ;
	private Vector3 lastTargetPosition;
	private Vector3 currentVelocity;
	private Vector3 lookAheadPos;
	
	private float shakeIntensity;
	private float shakeDecay;
	private float shakeDuration;
	
	void Start ()
	{		
		cameraFollowsPlayer=true;
		
		// player and level bounds initialization
		target = GameObject.FindGameObjectWithTag("Player").transform;
		levelBounds = GameObject.FindGameObjectWithTag("LevelBounds").GetComponent<LevelBounds>();		
		
		lastTargetPosition = target.position;
		offsetZ = (transform.position - target.position).z;
		transform.parent = null;
		
		// camera size calculation (orthographicSize is half the height of what the camera sees.
		float cameraHeight = Camera.main.orthographicSize * 2f;		
		float cameraWidth = cameraHeight * Camera.main.aspect;
			
		// we get the levelbounds coordinates to lock the camera into the level
		xMin = levelBounds.boundsXMin+(cameraWidth/2);
		xMax = levelBounds.boundsXMax-(cameraWidth/2); 
		yMin = levelBounds.boundsYMin+(cameraHeight/2); 
		yMax = levelBounds.boundsYMax-(cameraHeight/2);		
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		// if the camera is not supposed to follow the player, we do nothing
		if (!cameraFollowsPlayer)
			return;
			
		// if the player has moved since last update
		float xMoveDelta = (target.position - lastTargetPosition).x;
		Vector3 shakeFactorPosition = new Vector3(0,0,0);
		
		bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > cameraThreshold;
		
		if (updateLookAheadTarget) 
		{
			lookAheadPos = cameraAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
		} 
		else 
		{
			lookAheadPos = Vector3.MoveTowards(lookAheadPos, Vector3.zero, Time.deltaTime * cameraReturnSpeed);	
		}
		
		Vector3 aheadTargetPos = target.position + lookAheadPos + Vector3.forward * offsetZ;
		Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref currentVelocity, cameraSlowFactor);
		
		// If shakeDuration is still running.
		if (shakeDuration>0)
		{
			shakeFactorPosition= Random.insideUnitSphere * shakeIntensity * shakeDuration;
			shakeDuration-=shakeDecay*Time.deltaTime ;
		}
		
		newPos = newPos+shakeFactorPosition;		
		
		// Level boundaries
		float posX = Mathf.Clamp(newPos.x, xMin, xMax);
		float posY = Mathf.Clamp(newPos.y, yMin, yMax);
		float posZ = newPos.z;
		
		//create new position - within set boundaries
		newPos = new Vector3(posX, posY, posZ);
		
		transform.position=newPos;
		
		lastTargetPosition = target.position;		
	}
	
	// Use this method to shake the camera, passing in a Vector3 for intensity, duration and decay
	public void Shake(Vector3 shakeParameters)
	{
		shakeIntensity = shakeParameters.x;
		shakeDuration=shakeParameters.y;
		shakeDecay=shakeParameters.z;
	}
}