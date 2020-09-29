using UnityEngine;
using System.Collections;

public class VisibleParticle : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = "VisibleParticles";
	}
	
}
