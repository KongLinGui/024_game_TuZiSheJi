using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour 
{
	public GameObject ladderPlatform;

	public void OnTriggerEnter2D(Collider2D other)
	{
		// we check that the player is actually colliding with the ladder
		var player = other.GetComponent<Player>();
		if (player==null)
			return;
		
		var controller = other.GetComponent<CorgiController2D>();
		if (controller==null)
			return;		
	}
		
	public void OnTriggerStay2D(Collider2D other)
	{		
		// we check that the player is actually colliding with the ladder
		var player = other.GetComponent<Player>();
		if (player==null)
			return;
		
		var controller = other.GetComponent<CorgiController2D>();
		if (controller==null)
			return;
		
		controller.State.LadderColliding=true;
		if (controller.State.LadderClimbing)
		{
			// if the player is climbing a ladder, we center it on the ladder
			controller.transform.position=new Vector2(transform.position.x,controller.transform.position.y);
		}
				
		// if the feet of the player are above the ladder platform, we release it from the ladder.
		if (ladderPlatform.transform.position.y < controller.transform.position.y-(controller.transform.localScale.y/2)-0.1f)
		{
			controller.State.LadderClimbing=false;
			controller.State.CanMoveFreely=true;
			controller.State.LadderClimbingSpeed=0;			
		}
		
	}
	
	public void OnTriggerExit2D(Collider2D other)
	{
		var player = other.GetComponent<Player>();
		if (player==null)
			return;
		
		var controller = other.GetComponent<CorgiController2D>();
		if (controller==null)
			return;
		
		// when the player is not colliding with the ladder anymore, we reset the various ladder related states
		controller.State.LadderColliding=false;
		controller.State.LadderClimbing=false;
		controller.State.CanMoveFreely=true;
		
		
	}
}
