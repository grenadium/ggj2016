//SICustomCamera.cs
//
//Copyright (c) 2015 blkcatman
//
using UnityEngine;

public class SICustomCamera : SICameraBase {
	public string shaderName = "Hidden/EquirectangularShader";

	void Start () {
		InitShader(shaderName);

		Camera main = gameObject.GetComponent<Camera>();
		main.aspect = 1f;

		Material mat = new Material(base.shader);
		mat.SetVector("_forward", gameObject.transform.forward);
		mat.SetVector("_right", gameObject.transform.right);
		mat.SetVector("_up", gameObject.transform.up);
		mat.SetFloat("_fov", main.fieldOfView);
		main.targetTexture = targetTexture;

		SIRenderEvent ev = gameObject.AddComponent<SIRenderEvent>();
		ev.ref_camera = main;
		ev.material = mat;
		ev.forceUpdateParameters = false;
	}
}
