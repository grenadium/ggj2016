using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    public bool ForceHuman = false;
    public string DefaultHumanConfig = "";
    public GameObject NetworkManagerPrefab = null;
    public GameObject VrManagerPrefab = null;

    // Victory
    public AudioClip humanVictoryJingle;
    public AudioClip flyVictoryJingle;
    private AudioSource musicSource;

	// Use this for initialization
	protected void Start ()
	{
        musicSource = GetComponent<AudioSource>();
	    Player.ProcessPlayerType = ForceHuman ? Player.PlayerType.Human : DetectPlayerType();
        CreateNetworkManager(Player.ProcessPlayerType);
	}

    public static Player.PlayerType DetectPlayerType()
    {
        return System.Environment.GetCommandLineArgs().Any(arg => arg == "--config") ? Player.PlayerType.Human : Player.PlayerType.Fly;
    }

    private void CreateNetworkManager(Player.PlayerType iPlayerType)
    {
        var networkManagerObject = Instantiate(NetworkManagerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        if (!networkManagerObject) return;
        networkManagerObject.name = "NetworkManager";

        var networkManagerScript = networkManagerObject.GetComponent<NetworkManager>();
        if (!networkManagerScript) return;

        if (iPlayerType == Player.PlayerType.Human)
        {
            var vrManagerPrefabScript = VrManagerPrefab.GetComponent<VRManagerScript>();
            vrManagerPrefabScript.ShowWand = false;
            vrManagerPrefabScript.UseVRMenu = false;
            vrManagerPrefabScript.ConfigFile = DefaultHumanConfig;
            vrManagerPrefabScript.Navigation = VRManagerScript.ENavigation.None;
            vrManagerPrefabScript.Manipulation = VRManagerScript.EManipulation.None;

            var vrManagerObject = Instantiate(VrManagerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            if (vrManagerObject == null) return;
            vrManagerObject.name = "VRManager";

            networkManagerScript.StartHost();
            if (!VrManagerPrefab) return;
        }
        else
        {
            networkManagerObject.AddComponent<NetworkManagerHUD>();
        }
    }

    #region Victory

    public void SignalVictory (Player.PlayerType player)
    {
        switch(player)
        {
            case Player.PlayerType.Fly:
                musicSource.clip = flyVictoryJingle;
                break;

            case Player.PlayerType.Human:
                musicSource.clip = humanVictoryJingle;
                break;
        }

        musicSource.Play();
    }

    #endregion
}
