using UnityEngine;
using System.Collections;

public class ControllerState2D 
{
	
	public bool IsCollidingRight { get; set; }
	public bool IsCollidingLeft { get; set; }
	public bool IsCollidingAbove { get; set; }
	public bool IsCollidingBelow { get; set; }
	public bool IsMovingDownSlope { get; set; }
	public bool IsMovingUpSlope { get; set; }
	public bool IsGrounded { get { return IsCollidingBelow; } }
	public float SlopeAngle { get; set; }
	
	public bool HasCollisions { get { return IsCollidingRight || IsCollidingLeft || IsCollidingAbove || IsCollidingBelow; }}
	
	// states		
	public bool CanJump{get;set;}	
	public bool CanFire{get;set;}	
	public bool CanMelee{get;set;}		
	public bool CanDoubleJump{get;set;}
	public bool CanDash{get;set;}
	public bool CanMoveFreely{get;set;}
	public bool Dashing{get;set;}
	public bool Running{get;set;}
	public bool Crouching{get;set;}
	public bool CrouchingPreviously{get;set;}
	public bool TouchingGroundPreviously{get;set;}
	public bool LookingUp{get;set;}
	public bool WallClinging{get;set;}
	public bool Jetpacking{get;set;}
	public bool Diving{get;set;}
	public bool Firing{get;set;}
	public bool FiringStop{get;set;}
	public bool MeleeAttacking{get;set;}
	
	public bool LadderColliding{get;set;}
	public bool LadderClimbing{get;set;}
	public float LadderClimbingSpeed{get;set;}
	public bool LadderTopColliding{get;set;}
	
	public int FiringDirection{get;set;}
	
	public void Reset()
	{
		IsMovingUpSlope =
			IsMovingDownSlope =
			IsCollidingLeft = 
			IsCollidingRight = 
			IsCollidingAbove =
			IsCollidingBelow = false;
		SlopeAngle = 0;
	}
	
	public override string ToString ()
	{
		return string.Format("(controller: r:{0} l:{1} a:{2} b:{3} down-slope:{4} up-slope:{5} angle: {6}",
		IsCollidingRight,
		IsCollidingLeft,
		IsCollidingAbove,
		IsCollidingBelow,
		IsMovingDownSlope,
		IsMovingUpSlope,
		SlopeAngle);
	}
	
	
}
