using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TapetteDisplay : MonoBehaviour {
    public Sprite flyReadySprite, waitingForFlySprite;

    public List<Sprite> countSprites;
    public AudioClip flyArrivedSound, timerSound;
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
    
    //Afficher 3,2 ou 1
    public void setCountDownSprite(int index)
    {
        GetComponent<SpriteRenderer>().sprite = countSprites[index];
        GetComponent<AudioSource>().clip = timerSound;
        GetComponent<AudioSource>().Play();

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
        GetComponent<AudioSource>().clip = flyArrivedSound;
        GetComponent<AudioSource>().Play();
    }

    //cacher le sprite
    public void hideDisplay(bool hide)
    {
        if(hide)
         GetComponent<SpriteRenderer>().enabled = false;

        else
            GetComponent<SpriteRenderer>().enabled = true;
    }
}
