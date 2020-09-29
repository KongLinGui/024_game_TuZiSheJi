using UnityEngine;
using System.Collections;

public class GiveDamageToPlayer : MonoBehaviour 
{

	public int DamageToGive = 10;
		
	private Vector2
		_lastPosition,
		_velocity;
	
	

	void Start () 
	{
	
	}
	
	
	public void LateUpdate () 
	{
		_velocity = (_lastPosition - (Vector2)transform.position) /Time.deltaTime;
		_lastPosition = transform.position;
	}
	
	public void OnTriggerEnter2D(Collider2D other)
	{
		var player=other.GetComponent<Player>();
		if (player==null)
			return;
		
		player.TakeDamage(DamageToGive,gameObject);
		var controller=player.GetComponent<CorgiController2D>();
		var totalVelocity=controller.Velocity + _velocity;
		
		controller.SetForce(new Vector2(
			-1*Mathf.Sign(totalVelocity.x) * Mathf.Clamp(Mathf.Abs(totalVelocity.x) * 5,10,20),
			-1*Mathf.Sign(totalVelocity.y) * Mathf.Clamp(Mathf.Abs(totalVelocity.y) * 2,0,15)			
			));
	}
}
