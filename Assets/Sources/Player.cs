using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    public enum PlayerType
    {
        Human,
        Fly
    }

    public GameObject m_HumanHeadPrefab = null;
    public GameObject m_HumanHandPrefab = null;
    public GameObject m_FlyPrefab = null;

    public static PlayerType ProcessPlayerType = PlayerType.Human;

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
        var fly = Instantiate(m_FlyPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        NetworkServer.SpawnWithClientAuthority(fly, connectionToClient);
    }

    // Use this for initialization
    void Start ()
    {
        if (!isLocalPlayer)
            return;

        if (ProcessPlayerType == PlayerType.Human)
        {
            CmdSpawnHuman();
        }
        else
        {
            CmdSpawnFly();
        }
    }
}
