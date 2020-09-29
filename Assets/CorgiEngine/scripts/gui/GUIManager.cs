using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour 
{
	
	public GameObject PauseScreen;	
	
	private static GUIManager _instance;
	
	//This is the public reference that other classes will use
	public static GUIManager Instance
	{
		get
		{
			//If _instance hasn't been set yet, we grab it from the scene!
			//This will only happen the first time this reference is used.
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<GUIManager>();
			return _instance;
		}
	}
	
	public void SetPause()
	{
		PauseScreen.SetActive(true);
	}
	
	public void UnPause()
	{
		PauseScreen.SetActive(false);
	}
	
}
