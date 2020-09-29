using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
	public static LevelManager Instance { get; private set; }		
	
	public Player PlayerPrefab ;
	private Player player;
	public CameraController Camera { get; private set; }
	public TimeSpan RunningTime { get { return DateTime.UtcNow - _started ;}}
	
	public int CurrentTimeBonus
	{
		get 
		{
			var secondDifference = (int) (BonusCutoffSeconds - RunningTime.TotalSeconds);
			return Mathf.Max (0,secondDifference) * BonusSecondMultiplier;
		}
	}
	
	private List<CheckPoint> _checkpoints;
	private int _currentCheckPointIndex;
	private DateTime _started;
	private int _savedPoints;
	
	public CheckPoint DebugSpawn;
	public int BonusCutoffSeconds = 30;
	public int BonusSecondMultiplier = 1;	
	
	public void Awake()
	{
		_savedPoints=GameManager.Instance.Points;
		Instance=this;
		player = (Player)Instantiate(PlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
	}
	public void Start()
	{
		_checkpoints = FindObjectsOfType<CheckPoint>().OrderBy(t => t.transform.position.x).ToList();
		_currentCheckPointIndex = _checkpoints.Count > 0 ? 0 : -1;
		
				
		Camera = FindObjectOfType<CameraController>();
				
		_started = DateTime.UtcNow;
		
		var listeners = FindObjectsOfType<MonoBehaviour>().OfType<IPlayerRespawnListener>();
		foreach(var listener in listeners)
		{
			for (var i = _checkpoints.Count - 1; i>=0; i--)
			{
				var distance = ((MonoBehaviour) listener).transform.position.x - _checkpoints[i].transform.position.x;
				if (distance<0)
					continue;
				
				_checkpoints[i].AssignObjectToCheckPoint(listener);
				break;
			}
		}
		
		#if UNITY_EDITOR
		if (DebugSpawn!= null)
		{
			DebugSpawn.SpawnPlayer(player);
		}
		else if (_currentCheckPointIndex != -1)
		{
			_checkpoints[_currentCheckPointIndex].SpawnPlayer(player);
		}
		#else
		if (_currentCheckPointIndex != -1)
		{			
			_checkpoints[_currentCheckPointIndex].SpawnPlayer(player);
		}
		#endif		
	}
	
	public void Update()
	{
		var isAtLastCheckPoint = _currentCheckPointIndex + 1 >= _checkpoints.Count;
		if (isAtLastCheckPoint)
			return;
		
		var distanceToNextCheckPoint = _checkpoints[_currentCheckPointIndex+1].transform.position.x - player.transform.position.x;
		if (distanceToNextCheckPoint>=0)
			return;
		
		_checkpoints[_currentCheckPointIndex].PlayerLeftCheckPoint();
		
		_currentCheckPointIndex++;
		_checkpoints[_currentCheckPointIndex].PlayerHitCheckPoint();
		
		GameManager.Instance.AddPoints(CurrentTimeBonus);
		_savedPoints = GameManager.Instance.Points;
		_started = DateTime.UtcNow;
	}
	
	public void GotoLevel(string levelName)
	{
		StartCoroutine(GotoLevelCo(levelName));
	}
	
	private IEnumerator GotoLevelCo(string levelName)
	{
		player.FinishLevel();
		GameManager.Instance.AddPoints(CurrentTimeBonus);
		yield return new WaitForSeconds(2f);
		
		if (string.IsNullOrEmpty(levelName))
			Application.LoadLevel("StartScreen");
		else
			Application.LoadLevel(levelName);
		
	}
	
	public void KillPlayer()
	{
		StartCoroutine(KillPlayerCo());
	}
	
	private IEnumerator KillPlayerCo()
	{
		player.Kill();
		Camera.cameraFollowsPlayer=false;
		yield return new WaitForSeconds(2f);
		
		Camera.cameraFollowsPlayer=true;
		if (_currentCheckPointIndex!=-1)
			_checkpoints[_currentCheckPointIndex].SpawnPlayer(player);
		
		_started = DateTime.UtcNow;
		GameManager.Instance.ResetPoints(_savedPoints);
	}
}

