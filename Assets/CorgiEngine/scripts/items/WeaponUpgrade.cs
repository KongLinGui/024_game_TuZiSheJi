using UnityEngine;
using System.Collections;

public class WeaponUpgrade : MonoBehaviour, IPlayerRespawnListener
{
	public GameObject Effect;
	public Weapon WeaponToGive;

	public void OnTriggerEnter2D (Collider2D other) 
	{
		if (other.GetComponent<Player>() == null)
		{
			return;
		}		
		// adds an instance of the effect at the coin's position
		Instantiate(Effect,transform.position,transform.rotation);
		
		other.GetComponent<Player>().ChangeWeapon(WeaponToGive);
		
		gameObject.SetActive(false);
	}
	public void onPlayerRespawnInThisCheckpoint(CheckPoint checkpoint, Player player)
	{
		gameObject.SetActive(true);
	}
}
