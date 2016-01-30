using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
	// Use this for initialization
	protected void Start ()
	{
	    CreateNetworkManager( PlayerScript.DetectPlayerType() );
	}

    private void CreateNetworkManager(PlayerScript.PlayerType iPlayerType)
    {
        var networkManagerObject = GameObject.Find("NetworkManager");
        if (networkManagerObject)
        {
            var networkManagerScript = networkManagerObject.GetComponent<NetworkManager>();
            if (networkManagerScript)
            {
                if (iPlayerType == PlayerScript.PlayerType.Human)
                {
                    networkManagerScript.StartHost();
                }
                else
                {
                    networkManagerObject.AddComponent<NetworkManagerHUD>();
                }
            }
        }
    }
}
