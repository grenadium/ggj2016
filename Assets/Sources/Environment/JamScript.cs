using UnityEngine;
using System.Collections;

//Script confiture
public class JamScript : MonoBehaviour {
    private float stickTime; //temps de blocage de la mouche
    private bool flySticked;
    public float lifeTime; //duree de vie de la confiture

	// Use this for initialization
	void Start () {
        flySticked = false;
        stickTime = GlobalVariables.JAM_STICK_TIME;

        Destroy(gameObject, lifeTime);
	
	}
}
