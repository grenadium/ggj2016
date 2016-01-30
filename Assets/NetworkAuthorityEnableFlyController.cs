using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkAuthorityEnableFlyController : NetworkBehaviour {

	void Update () {
        if (!hasAuthority)
            return;

	    GetComponent<FlyController>().enabled = true;

        enabled = false;
    }
}
