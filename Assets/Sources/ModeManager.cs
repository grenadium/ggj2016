using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Networking;

public class ModeManager : MonoBehaviour
{
    public bool ForceHuman = false;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

	protected void Start ()
	{
	    Player.ProcessPlayerType = ForceHuman ? Player.PlayerType.Human : DetectPlayerType();
        CreateNetworkManager(Player.ProcessPlayerType);
	}

    public static Player.PlayerType DetectPlayerType()
    {
        return System.Environment.GetCommandLineArgs().Any(arg => arg == "--config") ? Player.PlayerType.Human : Player.PlayerType.Fly;
    }

    private void CreateNetworkManager(Player.PlayerType iPlayerType)
    {
        var networkManagerObject = GameObject.Find("NetworkManager");
        if (!networkManagerObject) return;

        var networkManagerScript = networkManagerObject.GetComponent<NetworkManager>();
        if (!networkManagerScript) return;

        if (iPlayerType == Player.PlayerType.Human)
        {
            networkManagerScript.StartHost();
        }
        else
        {
            networkManagerObject.AddComponent<NetworkManagerHUD>();
        }
    }
}
