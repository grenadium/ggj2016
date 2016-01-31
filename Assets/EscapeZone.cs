using UnityEngine;
using System.Collections;

public class EscapeZone : MonoBehaviour {

    public bool flyHasEscaped = false;

	void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.tag == "Fly")
        {
            flyHasEscaped = true;

            GameManager gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
            gameManager.SignalVictory(Player.PlayerType.Fly);
        }
    }
}
