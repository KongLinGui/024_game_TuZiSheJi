using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class ControllerParameters2D 
{
	public enum JumpBehavior
	{
		CanJumpOnGround,
		CanJumpAnywhere,
		CantJump
	}

	public Vector2 MaxVelocity = new Vector2(float.MaxValue, float.MaxValue);
	
	[Range(0,90)]
	public float SlopeLimit = 45;	
	public float Gravity = -15;
	public float JumpHeight = 3.025f;
	public JumpBehavior JumpRestrictions;
	public float JumpFrequency = 0.25f;
	public float SpeedAccelerationOnGround = 20f;
	public float SpeedAccelerationInAir = 5f;

	public float MovementSpeed = 8f;
	public float CrouchSpeed = 4f;
	public float WalkSpeed = 8f;
	public float RunSpeed = 16f;
	public float LadderSpeed = 2f;
	public float JumpMinimumAirTime = 0.1f;
	public float DashDuration = 0.15f;
	public float DashForce = 5f;	
	public float DashCooldown = 2f;	
	public float WallJumpDuration = 0.15f;
	public float WallJumpForce = 3f;
	public float JetpackForce = 2.5f;
	
	public bool EightDirectionShooting=true;
	public bool StrictEightDirectionShooting=true;
	
}
