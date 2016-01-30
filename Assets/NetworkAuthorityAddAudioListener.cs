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

        // Hide head on self
        transform.Find("head").gameObject.SetActive( false );

        enabled = false;
	}
}
