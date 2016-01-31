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
	
	// Update is called once per frame
	void Update () {
	
	}


    //Fonction pour coller la mouche pendant quelques secondes
    public IEnumerator stickFly(GameObject fly)
    {
        Debug.Log("fly stuck !");
        //backup de la velocite
        Vector3 flyVelocity = fly.GetComponent<Rigidbody>().velocity;
        Vector3 flyAngVelocity = fly.GetComponent<Rigidbody>().angularVelocity;

        fly.GetComponent<Rigidbody>().velocity = Vector3.zero;
        fly.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        fly.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        yield return new WaitForSeconds(stickTime);

        //remettre la velocité et aucune contrainte
        fly.GetComponent<Rigidbody>().velocity = flyVelocity;
        fly.GetComponent<Rigidbody>().angularVelocity = flyAngVelocity;
        fly.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        fly.GetComponent<Rigidbody>().AddForce(Vector3.up * 10f);
        flySticked = false;    

    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == GlobalVariables.T_FLY)
        {
            //bloquer la mouche quelques secondes
            Debug.Log("mouche a touche la confiture");
            flySticked = true;
            StartCoroutine(stickFly(col.gameObject));

        }
    }
}
