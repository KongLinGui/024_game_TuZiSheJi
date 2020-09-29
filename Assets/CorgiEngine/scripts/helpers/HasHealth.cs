using UnityEngine;
using System.Collections;

public class HasHealth : MonoBehaviour, ITakeDamage
{
	public int Health;
	public GameObject HurtEffect;
	public GameObject DestroyEffect;

	void Start () 
	{
	
	}
	
	void Update () 
	{
	
	}
	
	public void TakeDamage(int damage,GameObject instigator)
	{				
		Instantiate(HurtEffect,instigator.transform.position,transform.rotation);
				
		Health -= damage;
		if (Health<=0)
		{
			DestroyObject();
		}
	}
	
	private void DestroyObject()
	{
		if (DestroyEffect!=null)
		{
			Instantiate(DestroyEffect,transform.position,transform.rotation);
		}
		
		Destroy(gameObject);
	}
}
