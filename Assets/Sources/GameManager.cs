using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    // Victory
    public AudioClip humanVictoryJingle;
    public AudioClip flyVictoryJingle;
    private AudioSource musicSource;

    public enum GameState
    {
        PLAYING,
        FINISHED
    }
    public static GameState gameState = GameState.PLAYING;

    #region Victory
    void Start ()
    {
        musicSource = GetComponent<AudioSource>();
    }

    public void SignalVictory (Player.PlayerType player)
    {
        if (gameState == GameState.PLAYING)
        {
            switch (player)
            {
                case Player.PlayerType.Fly:
                    musicSource.clip = flyVictoryJingle;
                    break;

                case Player.PlayerType.Human:
                    musicSource.clip = humanVictoryJingle;
                    break;
            }

            gameState = GameState.FINISHED;
            musicSource.Play();
        }
    }
    #endregion
}
