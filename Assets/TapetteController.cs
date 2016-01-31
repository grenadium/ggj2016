using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class TapetteController : NetworkBehaviour {

	// Audio
    public AudioClip[] slapSounds;
    public AudioClip[] jingleSounds;
    private AudioSource tapetteAudio;

    // Sound characteristics
    private bool slapSoundIsPlaying = false;
    private bool jingleSoundIsPlaying = false;
    private float slapClipLength = 0f;
    private float lastSlapPlayed = 0f;
    private float jingleFadeDelay = 5f;
    private float lastJinglePlayed = 0f;
    private int totalFrames;

    public float currentVelocity;
    public int nbFramesForVelocity = 5; //nombre de frames à prendre en compte pour le calcul de la vélocité (plus il y a de frames, plus le mouvement doit être grand)
    private float weightCorrection = 1.2f;
    private int currentFrame;
    private List<float> velocities; //velocite de la tapette durant les frames precedentes
    private Vector3 lastPosition;

    #region Monobehaviour callbacks
    void Start ()
    {
        velocities = new List<float>();
        currentFrame = 0;
        lastPosition = transform.position;
        tapetteAudio = GetComponent<AudioSource>();
        foreach (TapetteCollider collider in GetComponentsInChildren<TapetteCollider>())
            collider.owner = this;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentFrame < nbFramesForVelocity)
        {
            float velocity = ((transform.position - lastPosition).magnitude) / Time.deltaTime; //velocite a la frame actuelle
            lastPosition = transform.position;

            velocities.Add(velocity);
            currentFrame++;
        }

        //quand on atteint la dernière frame prise en compte pour le mouvement
        else if (currentFrame == nbFramesForVelocity)
        {
            float sumVelo = 0;
            for (int i = 0; i < velocities.Count; i++)
            {
                sumVelo += velocities[i];
            }

            currentVelocity = sumVelo / nbFramesForVelocity;

            velocities.Clear();
            currentFrame = 0;
        }

        totalFrames++;
    }

    void Update ()
    {
        if((Time.realtimeSinceStartup - lastJinglePlayed) > jingleFadeDelay)
        {
            tapetteAudio.Stop();
            jingleSoundIsPlaying = false;
        }
    }

    #endregion

    #region Collision events

    public void AskForSlap (Vector3 pos)
    {
        CmdPlaySlapClip(pos);
    }

    /// <summary>
    /// Called in every client
    /// </summary>
    /// <param name="position"></param>
    public void Slap    (Vector3 position)
    {
        // Slap sounds
        if((Time.realtimeSinceStartup - lastSlapPlayed) > slapClipLength)
        {
            int index = Random.Range(0, slapSounds.Length - 1);
            AudioSource.PlayClipAtPoint(slapSounds[index], position, 1);
            slapClipLength = slapSounds[index].length;
            lastSlapPlayed = Time.realtimeSinceStartup;
        }

        if(GameObject.FindGameObjectWithTag("Fly").GetComponent<FlyController>().flyState != FlyController.FlyState.DEAD)
        {
            // Jingle
            if (!jingleSoundIsPlaying)
            {
                int index = Random.Range(0, jingleSounds.Length - 1);
                tapetteAudio.clip = jingleSounds[index];
                lastJinglePlayed = Time.realtimeSinceStartup;
                tapetteAudio.Play();
                jingleSoundIsPlaying = true;
            }
            else
            {
                lastJinglePlayed = Time.realtimeSinceStartup;
            }
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
