using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AddCameraIfFlyPlayer : NetworkBehaviour
{
    public GameObject FlyCameraPrefab = null;
    void Update()
    {
        if (!hasAuthority)
            return;

        var defaultCamera = GameObject.Find("Main Camera");
        if (defaultCamera != null)
        {
            defaultCamera.SetActive(false);
        }

        var camera = Instantiate(FlyCameraPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        camera.transform.parent = transform;
        camera.transform.localPosition = Vector3.zero;
        camera.transform.localRotation = Quaternion.identity;

        enabled = false;
    }
}
