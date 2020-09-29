using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour 
{
	// speed of the parallax layer
	public float speedX;
	public float speedY;
	// defines if the layer moves in the same direction as the camera or not
	public bool moveInOppositeDirection;

	private Transform cameraTransform;
	private Vector3 previousCameraPosition;
	private bool previousMoveParallax;
	private ParallaxOption options;

	void OnEnable() 
	{
		GameObject gameCamera = GameObject.Find("Main Camera");
		options = gameCamera.GetComponent<ParallaxOption>();
		cameraTransform = gameCamera.transform;
		previousCameraPosition = cameraTransform.position;
	}

	void Update () 
	{
		if(options.moveParallax && !previousMoveParallax)
			previousCameraPosition = cameraTransform.position;

		previousMoveParallax = options.moveParallax;

		if(!Application.isPlaying && !options.moveParallax)
			return;

		Vector3 distance = cameraTransform.position - previousCameraPosition;
		float direction = (moveInOppositeDirection) ? -1f : 1f;
		transform.position += Vector3.Scale(distance, new Vector3(speedX, speedY)) * direction;

		previousCameraPosition = cameraTransform.position;
	}
}
