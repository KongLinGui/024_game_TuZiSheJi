using UnityEngine;
using System.Collections;

public class FinishLevel : MonoBehaviour 
{
	public string LevelName;

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (other.GetComponent<Player>() == null)
			return;
			
		LevelManager.Instance.GotoLevel(LevelName);
	}

	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
