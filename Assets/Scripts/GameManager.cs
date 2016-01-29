using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public GameObject m_HumanPlayerPrefab = null;
    public GameObject m_FlyPlayerPrefab = null;

    public enum PlayerType
    {
        Human,
        Fly
    }

	// Use this for initialization
	protected void Start ()
	{
	    CreateNetworkManager(AutoDetectPlayerType());
	}

    private PlayerType AutoDetectPlayerType()
    {
        vrDeviceManager deviceMgr = MiddleVR.VRDeviceMgr;
        if (MiddleVR.VRDeviceMgr.GetTrackersNb() >= 2)
        {
            // We have a non-default configuration with at least 2 trackers
            return PlayerType.Human;
        }
        else
        {
            return PlayerType.Fly;
        }
    }

    private void CreateNetworkManager(PlayerType iPlayerType)
    {
        var networkManagerObject = new GameObject("NetworkManager");
        var networkManagerScript = networkManagerObject.AddComponent<UnityEngine.Networking.NetworkManager>();

        if (iPlayerType == PlayerType.Human)
        {
            networkManagerScript.playerPrefab = m_HumanPlayerPrefab;
            networkManagerScript.StartHost();
        }
        else
        {
            networkManagerScript.playerPrefab = m_FlyPlayerPrefab;
            networkManagerObject.AddComponent<NetworkManagerHUD>();
        }
    }
}
