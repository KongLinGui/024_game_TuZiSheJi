using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
	private static GameManager _instance;
	
	public static GameManager Instance { get; private set; }	
	
	public int Points { get; private set; }
	public float TimeScale { get; private set; }
	public bool Paused { get; set; } 
	
	private float savedTimeScale;
	
	
	void Awake()
	{
		Instance=this;
	}
	
	public void Reset()
	{
		Points = 0;
		TimeScale = 1f;
	}
	
	public void AddPoints(int pointsToAdd)
	{
		Points += pointsToAdd;
	}
	
	public void ResetPoints(int points)
	{
		Points = points;
	}
	
	public void SetTimeScale(float newTimeScale)
	{
		savedTimeScale = Time.timeScale;
		Time.timeScale = newTimeScale;
	}
	
	public void ResetTimeScale()
	{
		Time.timeScale = savedTimeScale;
	}
	
	
}
