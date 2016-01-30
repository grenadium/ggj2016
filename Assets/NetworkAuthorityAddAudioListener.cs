using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkAuthorityAddAudioListener : NetworkBehaviour {
	
	// Update is called once per frame
	void Update ()
	{
	    if (!hasAuthority)
	        return;

	    gameObject.AddComponent<AudioListener>();

	    enabled = false;
	}
}
