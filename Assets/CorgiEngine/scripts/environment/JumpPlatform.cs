using UnityEngine;
using System.Collections;

public class JumpPlatform : MonoBehaviour 
{
	public float JumpPlatformBoost = 40;
	public void ControllerEnter2D(CorgiController2D controller)
	{
		controller.State.CanDoubleJump = false;
		controller.SetVerticalForce(Mathf.Sqrt( 2f * JumpPlatformBoost * -controller.Parameters.Gravity ));
	}
}
