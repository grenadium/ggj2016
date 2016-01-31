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

    public string m_DefaultHumanConfig = "";
    public GameObject m_VrManagerPrefab = null;
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
        var fly = Instantiate(m_FlyPrefab, new Vector3(0.0f,1.0f,0.0f), Quaternion.identity) as GameObject;
        NetworkServer.SpawnWithClientAuthority(fly, connectionToClient);
    }

    // Use this for initialization
    void Start ()
    {
        if (!isLocalPlayer)
            return;

        if (ProcessPlayerType == PlayerType.Human)
        {
            var vrManagerPrefabScript = m_VrManagerPrefab.GetComponent<VRManagerScript>();
            vrManagerPrefabScript.ShowWand = false;
            vrManagerPrefabScript.UseVRMenu = false;
            vrManagerPrefabScript.ConfigFile = m_DefaultHumanConfig;
            vrManagerPrefabScript.Navigation = VRManagerScript.ENavigation.None;
            vrManagerPrefabScript.Manipulation = VRManagerScript.EManipulation.None;

            var mainCamera = GameObject.Find("Main Camera");
            if (mainCamera != null)
            {
                vrManagerPrefabScript.TemplateCamera = mainCamera;
            }

            var vrManagerObject = Instantiate(m_VrManagerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            if (vrManagerObject == null) return;
            vrManagerObject.name = "VRManager";

            CmdSpawnHuman();
        }
        else
        {
            CmdSpawnFly();
        }
    }
}
