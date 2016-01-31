using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    // Victory
    public AudioClip humanVictoryJingle;
    public AudioClip flyVictoryJingle;
    private AudioSource musicSource;

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
