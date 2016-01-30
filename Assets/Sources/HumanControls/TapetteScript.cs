﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
* Script qui gere la tapette a mouche
**/
public class TapetteScript : MonoBehaviour {

    public float minimumKillSpeed; //Vitesse minimale pour considerer que la tapette tue la mouche
    public float minimumJamBreakSpeed; //vitesse mini pour casser le pot de confiture
    public float minimumSoundSpeed; //vitesse mini pour jouer un son de swing
    private int totalFrames;

    public float currentVelocity;
    public int nbFramesForVelocity = 5; //nombre de frames à prendre en compte pour le calcul de la vélocité (plus il y a de frames, plus le mouvement doit être grand)
    private float weightCorrection = 1.2f;
    private int currentFrame;
    private List<float> velocities; //velocite de la tapette durant les frames precedentes
    private Vector3 lastPosition;

    private AudioSource audioSource;
    public List<AudioClip> swingSounds, jamBreakSounds;

	// Use this for initialization
	void Start () {
        velocities = new List<float>();
        currentFrame = 0;
        lastPosition = transform.position;

        audioSource = gameObject.GetComponent<AudioSource>() as AudioSource;

    }
	
	// Update is called once per frame
	void FixedUpdate () {

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

            //Play a swing sound (total frames > 30 car le son se joue dès le début même sans faire de mouvements... fix à revoir)
            if (totalFrames > 30 && currentVelocity >= minimumSoundSpeed)
            {
                playRandomSoundInList(swingSounds);
            }
                

            velocities.Clear();
            currentFrame = 0;
        }

        totalFrames++;
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

        //Touche la fenetre
        else if(col.gameObject.tag == GlobalVariables.T_WINDOW)
        {
            Debug.Log("fenetre touchee");
            col.gameObject.GetComponent<WindowScript>().changeWindowState();
        }

        //Touche un pot de confiture
        else if (col.gameObject.tag == GlobalVariables.T_JAM)
        {
            Debug.Log("confiture touchee");


            if(canBreakJamPot())
            {
                playRandomSoundInList(jamBreakSounds);

                GameObject particleSystem = (GameObject)Instantiate(Resources.Load("Prefabs/JamSpurt"));
                particleSystem.transform.position = this.transform.position;
                Destroy(particleSystem, 0.5f);

                //Créer la texture de confiture
                GameObject jamSpillZone = (GameObject)Instantiate(Resources.Load("Prefabs/JamTexture"));
                //jamSpillZone.GetComponent<MeshRenderer>().enabled = true;

                GameObject jamSprite = (GameObject)Instantiate(Resources.Load("Prefabs/JamSprite"));

                RaycastHit hit;

                //Placer la texture confiture en dessous du pot
                if (Physics.Raycast(col.gameObject.transform.position, -Vector3.up, out hit))
                {
                    jamSpillZone.transform.position = hit.point;
                    jamSprite.transform.position = new Vector3(hit.point.x, hit.point.y+0.01f, hit.point.z);

                    float yRotation = Random.Range(0f, 360f); //rotation aléatoire sur y

                    jamSpillZone.transform.Rotate(Vector3.up, yRotation);
                    jamSpillZone.transform.Rotate(Vector3.up, yRotation);
                }

                Destroy(col.gameObject);
                jamSpillZone.AddComponent<JamScript>();
                jamSpillZone.GetComponent<JamScript>().lifeTime = GlobalVariables.JAM_LIFE_TIME;
            }
            
        }


    }

    //Verifier que la tapette a une vitesse suffisante pour pouvoir ecraser la mouche
    public bool canSquishFly()
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

    //Verifier qu'on a assez de vitesse pour casser un pot de confiture
    public bool canBreakJamPot()
    {
        //Si assez de vitesse, on peut casser le pot
        if (currentVelocity >= minimumJamBreakSpeed)
        {
            return true;
        }

        return false;
    }

    
	 //Méthode qui permet de jouer un son aléatoire dans une liste
    public void playRandomSoundInList(List<AudioClip> list)
    {
        int randIndex = Random.Range(0, list.Count);
        audioSource.clip = list[randIndex];

        if (!audioSource.isPlaying)
         audioSource.Play();

    }

}
