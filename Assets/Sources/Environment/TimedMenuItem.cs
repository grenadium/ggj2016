using UnityEngine;
using System.Collections;

public class TimedMenuItem : MonoBehaviour
{
    public float lifeTime;
    public bool removeMenuItem;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(removeMenuItem)
        {
            destroyMenuItem();
            removeMenuItem = false;
        }
    }

    void destroyMenuItem()
    {
        iTween.FadeTo(gameObject, 0f, lifeTime);
        Destroy(gameObject, lifeTime);
    }
}
