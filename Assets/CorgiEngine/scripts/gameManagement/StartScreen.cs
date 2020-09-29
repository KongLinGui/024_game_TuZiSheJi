using UnityEngine;
using System.Collections;
using UnitySampleAssets.CrossPlatformInput;

public class StartScreen : MonoBehaviour 
{
	public string FirstLevel;
	
	void Start () 
	{
	
	}
	
	void Update () 
	{
		if (!CrossPlatformInputManager.GetButtonDown("Jump"))
			return;
		
		Application.LoadLevel(FirstLevel);
		
	}
}
