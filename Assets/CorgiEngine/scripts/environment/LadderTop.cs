using UnityEngine;
using System.Collections;

public class LadderTop : MonoBehaviour 
{

	public void OnTriggerStay2D(Collider2D other)
	{		
		var player = other.GetComponent<Player>();
		if (player==null)
			return;
		
		var controller = other.GetComponent<CorgiController2D>();
		if (controller==null)
			return;
		
		controller.State.LadderTopColliding=true;
		
	}
	
	public void OnTriggerExit2D(Collider2D other)
	{
		var player = other.GetComponent<Player>();
		if (player==null)
			return;
		
		var controller = other.GetComponent<CorgiController2D>();
		if (controller==null)
			return;
		
		controller.State.LadderTopColliding=false;
		
		
	}
}
