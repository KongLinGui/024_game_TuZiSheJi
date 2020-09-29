using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckPoint : MonoBehaviour 
{
	private List<IPlayerRespawnListener> _listeners;

	public void Awake () 
	{
		_listeners = new List<IPlayerRespawnListener>();
	}
	
	public void PlayerHitCheckPoint()
	{
		
	}
	
	private IEnumerator PlayerHitCheckPointCo(int bonus)
	{
		yield break;
	}
	
	public void PlayerLeftCheckPoint()
	{
	
	}
	
	public void SpawnPlayer(Player player)
	{
		player.RespawnAt(transform);
		
		foreach(var listener in _listeners)
		{
			listener.onPlayerRespawnInThisCheckpoint(this,player);
		}
	}
	
	public void AssignObjectToCheckPoint (IPlayerRespawnListener listener) 
	{
		_listeners.Add(listener);
	}
}
