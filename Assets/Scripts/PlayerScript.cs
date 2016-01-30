using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerScript : NetworkBehaviour
{
    public enum PlayerType
    {
        Human,
        Fly
    }

    public GameObject m_HumanHeadPrefab = null;
    public GameObject m_HumanHandPrefab = null;
    public GameObject m_FlyPrefab = null;

    public PlayerType m_PlayerType = PlayerType.Human;

    [Command]
    public void CmdSpawnHuman()
    {
        GameObject humanHead = Instantiate(m_HumanHeadPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        NetworkServer.SpawnWithClientAuthority(humanHead, connectionToClient);

        GameObject humanHand = Instantiate(m_HumanHandPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        NetworkServer.SpawnWithClientAuthority(humanHand, connectionToClient);
    }

    [Command]
    public void CmdSpawnFly()
    {
        GameObject humanHead = Instantiate(m_FlyPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        NetworkServer.SpawnWithClientAuthority(humanHead, connectionToClient);
    }

    public static PlayerType DetectPlayerType()
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

    // Use this for initialization
    void Start ()
    {
        if (!isLocalPlayer)
            return;

        m_PlayerType = DetectPlayerType();
        if (m_PlayerType == PlayerType.Human)
        {
            CmdSpawnHuman();
        }
        else
        {
            CmdSpawnFly();
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
