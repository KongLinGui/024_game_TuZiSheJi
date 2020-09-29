using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour 
{
	private Player player;
	public Transform ForegroundSprite;
	public Color MaxHealthColor = new Color(255/255f, 63/255f, 63/255f);
	public Color MinHealthColor = new Color(64/255f, 137/255f, 255/255f);
	
	void Start()
	{
		player = FindObjectOfType<Player>();
	}
	
	public void Update()
	{
		var healthPercent = player.Health / (float) player.MaxHealth;
		ForegroundSprite.localScale = new Vector3(healthPercent,1,1);
		
	}
	
}
