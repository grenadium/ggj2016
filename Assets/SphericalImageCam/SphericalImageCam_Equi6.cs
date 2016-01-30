//SphericalImageCam_Equi6.cs
//
//Copyright (c) 2015 blkcatman
//
using UnityEngine;

public class SphericalImageCam_Equi6 : SICameraCreator {
	private float[] rots = {-90f, 0f, 90f, 179.99f, 0f, 0f, 0f, -90f, 0f, 90f, 0f, 179.99f};
	private float[] ds = {-0.5f, -0.5f, 0f, 0f, 0f, 0f};

	public bool drawImageOnGUI = true;

	void Start () {
		base.InitShader("Hidden/EquirectangularShader");

		Quaternion temp = gameObject.transform.rotation;
		gameObject.transform.rotation = new Quaternion();
		
		for (int i = 0; i < 6; i++) {
			float rotX, rotY;
			float fov = 90.0f;
			rotX = rots[i*2];
			rotY = rots[i*2+1];

			CreateCamera(new Vector3(rotX, rotY, 0f), fov, ds[i], "camera" + i.ToString());
		}

		if (drawImageOnGUI) {
			Camera main = gameObject.GetComponent<Camera>();
			main.nearClipPlane = 0.01f;
			main.farClipPlane = 0.02f;
		}
		gameObject.transform.rotation = temp;
	}

	void OnGUI () {
		if (base.canDraw && drawImageOnGUI) {
			GUI.depth = 2;
			GUI.DrawTexture(new Rect(0.0f, 0.0f, Screen.width, Screen.height), base.targetTexture);
		}
	}

}
