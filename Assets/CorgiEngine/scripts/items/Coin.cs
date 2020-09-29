using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour, IPlayerRespawnListener
{
	public GameObject Effect;
	public int PointsToAdd = 10;

	public void OnTriggerEnter2D (Collider2D other) 
	{
		if (other.GetComponent<Player>() == null)
		{
			return;
		}
		
		GameManager.Instance.AddPoints(PointsToAdd);
		// adds an instance of the effect at the coin's position
		Instantiate(Effect,transform.position,transform.rotation);
		
		gameObject.SetActive(false);
	}
	public void onPlayerRespawnInThisCheckpoint(CheckPoint checkpoint, Player player)
	{
		gameObject.SetActive(true);
	}
}
