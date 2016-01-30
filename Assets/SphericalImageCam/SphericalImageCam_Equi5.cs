//SphericalImageCam_Equi5.cs
//
//Copyright (c) 2015 blkcatman
//
using UnityEngine;

public class SphericalImageCam_Equi5 : SICameraCreator {
	private float[] rots = {-90f, 0f, 90f, 179.99f, 0f, 0f, 0f, -120f, 0f, 120f};
	private float[] ds = {-0.5f, -0.5f, 0f, 0f, 0f};

	public bool drawImageOnGUI = true;

    private GameObject[] cameras;
	
	void Start () {
		base.InitShader("Hidden/EquirectangularShader");

		Quaternion temp = gameObject.transform.rotation;
		gameObject.transform.rotation = new Quaternion();

        cameras = new GameObject[5];
		for (int i = 0; i < 5; i++) {
			float rotX, rotY;
			float fov = 120.0f;
			rotX = rots[i*2];
			rotY = rots[i*2+1];

            cameras[i] = CreateCamera(new Vector3(rotX, rotY, 0f), fov, ds[i], "camera" + i.ToString());
		}

		if (drawImageOnGUI) {
			Camera main = gameObject.GetComponent<Camera>();
			main.nearClipPlane = 0.01f;
			main.farClipPlane = 0.02f;
		}
		gameObject.transform.rotation = temp;

        Invoke("RepairCameraRendering", 0.2f);
	}

    void RepairCameraRendering ()
    {
        foreach(GameObject cam in cameras)
        {
            cam.GetComponent<SIRenderEvent>().FixRendering();
        }
    }

	void OnGUI () {
		if (base.canDraw && drawImageOnGUI) {
			GUI.depth = 2;
			GUI.DrawTexture(new Rect(0.0f, 0.0f, Screen.width, Screen.height), base.targetTexture);
		}
	}

}
