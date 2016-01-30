using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
* Script qui gere la tapette a mouche
**/
public class TapetteScript : MonoBehaviour {

    public float minimumKillSpeed; //Vitesse minimale pour considerer que la tapette tue la mouche
    public float repulsionForce; // force pour repousser la mouche
    public float currentVelocity;
    public int nbFramesForVelocity = 5; //nombre de frames à prendre en compte pour le calcul de la vélocité (plus il y a de frames, plus le mouvement doit être grand)
    private float weightCorrection = 1.2f;
    private int currentFrame;
    private List<float> velocities; //velocite de la tapette durant les frames precedentes
    private Vector3 lastPosition;

	// Use this for initialization
	void Start () {
        velocities = new List<float>();
        currentFrame = 0;
        lastPosition = transform.position;
	
	}
	
	// Update is called once per frame
	void Update () {

        if (currentFrame < nbFramesForVelocity)
        {
            float velocity  = ((transform.position - lastPosition).magnitude) / Time.deltaTime; //velocite a la frame actuelle
            lastPosition = transform.position;

            velocities.Add(velocity);
            currentFrame++;
        }

        //quand on atteint la dernière frame prise en compte pour le mouvement
        else if(currentFrame == nbFramesForVelocity)
        {
            float sumVelo = 0;
            for(int i = 0; i < velocities.Count;i++)
            {
                sumVelo += velocities[i];
            }

            currentVelocity = sumVelo / nbFramesForVelocity;


            velocities.Clear();
            currentFrame = 0;
        }
	
	}

    void OnCollisionEnter(Collision col)
    {
        //Si on touche la mouche
        if(col.gameObject.tag == GlobalVariables.T_FLY)
        {
            Debug.Log("mouche touchee");
            if(canSquishFly())
            {
                Destroy(col.gameObject);
            }
        }

        else if(col.gameObject.tag == GlobalVariables.T_WINDOW)
        {
            Debug.Log("fenetre touchee");
            col.gameObject.GetComponent<WindowScript>().changeWindowState();
        }
    }

    //Verifier que la tapette a une vitesse suffisante pour pouvoir ecraser la mouche
    bool canSquishFly()
    {
        Debug.Log(currentVelocity);

        //Si on a assez de vitesse lors du mouvement, on peut tuer la mouche
        if(currentVelocity >= minimumKillSpeed)
        {
            return true;
        }
        //TODO : Remplacer par le bon composant de la mouche, checker si elle est collée au sol ou sur un mur
        /*
        if(col.gameObject.GetComponent<FlyScript>().isGrounded)
        {
            return true;
        }
        */

        return false;
    }
}
