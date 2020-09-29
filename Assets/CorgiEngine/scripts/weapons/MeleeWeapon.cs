using UnityEngine;
using System.Collections;

public class MeleeWeapon : MonoBehaviour 
{
	
	public LayerMask CollisionMask;
	public int Damage;
	public GameObject HitEffect;
	public GameObject Owner;

	void Start () 
	{
	
	}
	
	
	void Update () 
	{
	
	}
	
	public virtual void OnTriggerEnter2D(Collider2D other)
	{
		// if the collider the melee weapon is colliding with is not on the targeted layer mask, we do nothing
		if ((CollisionMask.value & (1 << other.gameObject.layer)) == 0)
		{
			return;
		}
				
		// if the collider the melee weapon is colliding with is its owner (the player), we do nothing	
		var isOwner = other.gameObject == Owner;
		if (isOwner)
		{
			return;
		}
		
		
		// if the collider the melee weapon is colliding with can take damage, we apply the melee weapon's damage to it, and instantiate a hit effect
		var takeDamage= (ITakeDamage) other.GetComponent(typeof(ITakeDamage));
		if (takeDamage!=null)
		{
			OnCollideTakeDamage(other,takeDamage);
			return;
		}
		
		OnCollideOther(other);
	}
		
	void OnCollideTakeDamage(Collider2D other, ITakeDamage takeDamage)
	{
		Instantiate(HitEffect,other.transform.position,other.transform.rotation);
		takeDamage.TakeDamage(Damage,gameObject);
		DisableMeleeWeapon();		
	}
	
	void OnCollideOther(Collider2D other)
	{
		DisableMeleeWeapon();
	}
		
	void DisableMeleeWeapon()
	{
		// if you have longer lasting melee animations, you might want to disable the melee weapon's collider after it hits something, until the end of the animation.
	}
}
