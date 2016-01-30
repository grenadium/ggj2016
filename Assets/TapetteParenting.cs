using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class TapetteParenting : NetworkBehaviour {

	// Use this for initialization
	void Start ()
	{
	    var tapetteRoot = transform.Find("tapette_01/tapette_01_root");
	    var tapette = transform.Find("tapette_01");

	    tapetteRoot.parent = transform;
	    tapette.parent = null;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
