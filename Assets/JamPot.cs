using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JamPot : MonoBehaviour {

    public float minimumJamBreakSpeed = 3f; //vitesse mini pour casser le pot de confiture

    public AudioClip[] jamBreakSounds;

	void OnCollisionEnter (Collision collision)
    {
        TapetteController tapette;
        if((collision.gameObject.layer == LayerMask.NameToLayer("Tapette") 
            && (tapette = collision.gameObject.GetComponentInParent<TapetteController>()) != null
            && tapette.currentVelocity >= minimumJamBreakSpeed)
            || (collision.relativeVelocity.magnitude >= minimumJamBreakSpeed)
            )
        {
            Vector3 hitPoint = collision.contacts[0].point;
            playRandomSoundInList(jamBreakSounds, hitPoint);

            GameObject particleSystem = (GameObject)Instantiate(Resources.Load("Prefabs/JamSpurt"));
            particleSystem.transform.position = this.transform.position;
            Destroy(particleSystem, 0.5f);

            //Créer la texture de confiture
            GameObject jamSpillZone = (GameObject)Instantiate(Resources.Load("Prefabs/JamTexture"));
            //jamSpillZone.GetComponent<MeshRenderer>().enabled = true;

            GameObject jamSprite = (GameObject)Instantiate(Resources.Load("Prefabs/JamSprite"));

            RaycastHit hit;

            //Placer la texture confiture en dessous du pot
            if (Physics.Raycast(transform.position, -Vector3.up, out hit))
            {
                jamSpillZone.transform.position = hit.point;
                jamSprite.transform.position = new Vector3(hit.point.x, hit.point.y + 0.01f, hit.point.z);

                float yRotation = Random.Range(0f, 360f); //rotation aléatoire sur y

                jamSpillZone.transform.Rotate(Vector3.up, yRotation);
                jamSprite.transform.parent = jamSpillZone.transform;
            }

            gameObject.SetActive(false);
            jamSpillZone.AddComponent<JamScript>();
            jamSpillZone.GetComponent<JamScript>().lifeTime = GlobalVariables.JAM_LIFE_TIME;
        }
        
    }

    //Méthode qui permet de jouer un son aléatoire dans une liste
    public void playRandomSoundInList(AudioClip[] list, Vector3 hit)
    {
        if (list.Length > 0)
        {
            int randIndex = Random.Range(0, list.Length - 1);
            AudioSource.PlayClipAtPoint(list[randIndex], hit);
        }
    }
}
