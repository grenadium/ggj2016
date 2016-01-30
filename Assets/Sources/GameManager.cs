using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public bool ForceHuman = false;
    public string DefaultHumanConfig = "";
    public GameObject NetworkManagerPrefab = null;
    public GameObject VrManagerPrefab = null;

	// Use this for initialization
	protected void Start ()
	{
	    CreateNetworkManager(ForceHuman ? Player.PlayerType.Human : Player.DetectPlayerType());
	}

    private void CreateNetworkManager(Player.PlayerType iPlayerType)
    {
        var networkManagerObject = Instantiate(NetworkManagerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        if (!networkManagerObject) return;

        var networkManagerScript = networkManagerObject.GetComponent<NetworkManager>();
        if (!networkManagerScript) return;

        if (iPlayerType == Player.PlayerType.Human)
        {
            var vrManagerObject = Instantiate(VrManagerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            if (vrManagerObject == null) return;

            var vrManagerScript = vrManagerObject.GetComponent<VRManagerScript>();
            vrManagerScript.ShowWand = false;
            vrManagerScript.UseVRMenu = false;
            vrManagerScript.ConfigFile = DefaultHumanConfig;

            networkManagerScript.StartHost();
            if (!VrManagerPrefab) return;
        }
        else
        {
            networkManagerObject.AddComponent<NetworkManagerHUD>();
        }
    }
}
