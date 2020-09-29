using UnityEngine;
using System.Collections;

public class PathedProjectile : MonoBehaviour,ITakeDamage
{
	public GameObject DestroyEffect;
	public int PointsToGivePlayer=0;

	private Transform _destination;
	private float _speed;
	

	void Start () 
	{
	
	}
	
	public void Initialize(Transform destination, float speed)
	{
		_destination=destination;
		_speed=speed;
	}
	
	
	void Update () 
	{
		transform.position=Vector3.MoveTowards(transform.position,_destination.position,Time.deltaTime * _speed);
		var distanceSquared = (_destination.transform.position - transform.position).sqrMagnitude;
		if(distanceSquared > .01f * .01f)
			return;
		
		if (DestroyEffect!=null)
		{
			Instantiate(DestroyEffect,transform.position,transform.rotation);
		}
		
		Destroy(gameObject);
	}
	
	public void TakeDamage(int damage,GameObject instigator)
	{
		if (DestroyEffect!=null)
			Instantiate(DestroyEffect,transform.position, transform.rotation);
		Destroy(gameObject);
		
		var projectile = instigator.GetComponent<Projectile>();
		if(projectile!=null && projectile.Owner.GetComponent<Player>() != null && PointsToGivePlayer != 0)
		{
			GameManager.Instance.AddPoints(PointsToGivePlayer);
		}
				
	}
}
