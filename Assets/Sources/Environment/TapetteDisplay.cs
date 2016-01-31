using UnityEngine;
using System.Collections;

public class TapetteDisplay : MonoBehaviour {
    public Sprite flyReadySprite, waitingForFlySprite;
    public bool switchSprite, hideSprite;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (hideSprite)
            hideDisplay(true);

        else hideDisplay(false);
	
	}

    //afficher message fly has arrived
    public void setDisplayWaitingForFly()
    {
        GetComponent<SpriteRenderer>().sprite = waitingForFlySprite;
    }

    //afficher message fly has arrived
    public void setDisplayFlyReady()
    {
        GetComponent<SpriteRenderer>().sprite = flyReadySprite;
    }

    public void hideDisplay(bool hide)
    {
        if(hide)
         GetComponent<SpriteRenderer>().enabled = false;

        else
            GetComponent<SpriteRenderer>().enabled = true;
    }
}
