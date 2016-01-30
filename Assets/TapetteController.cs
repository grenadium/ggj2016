using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TapetteController : NetworkBehaviour {

	// Audio
    public AudioClip[] slapSounds;

    #region Monobehaviour callbacks
    void Start ()
    {
        if(hasAuthority)
        {
            foreach (TapetteCollider collider in GetComponentsInChildren<TapetteCollider>())
            {
                collider.owner = this;
            }
        }
    }
    #endregion

    #region Collision events
    public void Slap    (Vector3 position)
    {
        AudioSource.PlayClipAtPoint(slapSounds[Random.Range(0,slapSounds.Length-1)], position, 1);
    }
    #endregion

    #region Network
    [Command]
    void CmdPlaySlapClip (Vector3 position)
    {
        RpcPlaySlapClip(position);
    }

    [ClientRpc]
    public void RpcPlaySlapClip (Vector3 position)
    {
        Slap(position);
    }
    #endregion
}
