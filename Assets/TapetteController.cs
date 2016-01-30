using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TapetteController : NetworkBehaviour {

	// Audio
    public AudioClip[] slapSounds;

    // Sound characteristics
    private bool slapSoundIsPlaying = false;
    private bool jingleSoundIsPlaying = false;
    private float audioLength = 0f;
    private float lastSlapPlayed = 0f;

    #region Monobehaviour callbacks
    void Start ()
    {
        foreach (TapetteCollider collider in GetComponentsInChildren<TapetteCollider>())
            collider.owner = this;
    }
    #endregion

    #region Collision events
    public void Slap    (Vector3 position)
    {
        if((Time.realtimeSinceStartup - lastSlapPlayed) > audioLength)
        {
            int index = Random.Range(0, slapSounds.Length - 1);
            AudioSource.PlayClipAtPoint(slapSounds[index], position, 1);
            audioLength = slapSounds[index].length;
            lastSlapPlayed = Time.realtimeSinceStartup;
        }
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
