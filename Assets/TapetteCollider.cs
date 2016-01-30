using UnityEngine;
using System.Collections;

public class TapetteCollider : MonoBehaviour {

    public TapetteController owner;

    void OnCollisionEnter (Collision collision)
    {
        owner.Slap(collision.contacts[0].point);
    }
}
