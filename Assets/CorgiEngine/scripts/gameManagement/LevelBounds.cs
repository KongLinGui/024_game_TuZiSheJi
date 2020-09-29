using UnityEngine;
using System.Collections;

public class LevelBounds : MonoBehaviour {
	
	public float boundsXMin;
	public float boundsXMax;
	public float boundsYMin;
	public float boundsYMax;
	private BoxCollider2D _boxCollider;
	
	void Awake () {
		_boxCollider=GetComponent<BoxCollider2D>();
		
		boundsXMin=_boxCollider.bounds.min.x;
		boundsXMax=_boxCollider.bounds.max.x;
		boundsYMin=_boxCollider.bounds.min.y;
		boundsYMax=_boxCollider.bounds.max.y;		
	}
	
	void Update () {
		
	}
}