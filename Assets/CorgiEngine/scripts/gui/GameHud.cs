using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameHud : MonoBehaviour 
{

	private Text PointsText;
	
	public void Start()
	{
		PointsText = gameObject.GetComponent<Text>();
		PointsText.text="$"+GameManager.Instance.Points;		
	}
	
	public void Update()
	{
		PointsText.text="$"+GameManager.Instance.Points;		
	}
}
