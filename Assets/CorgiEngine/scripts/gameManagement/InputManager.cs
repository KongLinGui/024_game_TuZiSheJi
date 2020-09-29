using UnityEngine;
using System.Collections;
using UnitySampleAssets.CrossPlatformInput;

public class InputManager : MonoBehaviour 
{
		
	private static InputManager _instance;
	private static Player _player;
		
	//This is the public reference that other classes will use
	public static InputManager Instance
	{
		get
		{
			//If _instance hasn't been set yet, we grab it from the scene!
			//This will only happen the first time this reference is used.
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<InputManager>();
			return _instance;
		}
	}
	
	void Start()
	{
		_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();	
		
	}
	
	void Update()
	{		
		if ( CrossPlatformInputManager.GetButtonDown("Pause") )
			_player.Pause();
			
		if (GameManager.Instance.Paused)
			return;
			
		_player.SetHorizontalMove(CrossPlatformInputManager.GetAxis ("Horizontal"));
		_player.SetVerticalMove(CrossPlatformInputManager.GetAxis ("Vertical"));	
		
		if ((CrossPlatformInputManager.GetButtonDown("Run")||CrossPlatformInputManager.GetButton("Run")) )
			_player.RunStart();		
		
		if (CrossPlatformInputManager.GetButtonUp("Run"))
			_player.RunStop();		
				
		if (CrossPlatformInputManager.GetButtonDown("Jump"))
			_player.JumpStart();
				
		if (CrossPlatformInputManager.GetButtonUp("Jump"))
			_player.JumpStop();
				
		if ((CrossPlatformInputManager.GetButtonDown("Jetpack")||CrossPlatformInputManager.GetButton("Jetpack")) )
			_player.JetpackStart();
				
		if (CrossPlatformInputManager.GetButtonUp("Jetpack"))
			_player.JetpackStop();
				
		if ( CrossPlatformInputManager.GetButtonDown("Dash") )
			_player.Dash();
				
		if ( CrossPlatformInputManager.GetButtonDown("Melee")  )
			_player.Melee();
				
		if (CrossPlatformInputManager.GetButtonDown("Fire"))
			_player.ShootOnce();			
				
		if (CrossPlatformInputManager.GetButton("Fire")) 
			_player.ShootStart();
				
		if (CrossPlatformInputManager.GetButtonUp("Fire"))
			_player.ShootStop();
	}	
}
