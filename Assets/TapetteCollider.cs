using UnityEngine;
using System.Collections;

public class TapetteCollider : MonoBehaviour {

    public TapetteController owner;

    void OnCollisionEnter (Collision collision)
    {
        if(owner != null)
            owner.AskForSlap(collision.contacts[0].point);
    }
}
