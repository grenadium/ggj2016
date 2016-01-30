//SIRenderEvent.cs
//
//Copyright (c) 2015 blkcatman
//
using UnityEngine;

class SIRenderEvent : MonoBehaviour {
	[HideInInspector]
	public Camera ref_camera;
	[HideInInspector]
	public Material material;
	public bool forceUpdateParameters = false;
	
	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if(material == null) {
			Graphics.Blit (source, destination);
		}
		
		if (forceUpdateParameters) {
			material.SetFloat("_fov", ref_camera.fieldOfView);
			material.SetVector("_forward", transform.forward);
			material.SetVector("_right", transform.right);
			material.SetVector("_up", transform.up);
		}
		
		Graphics.Blit (source, destination, material);
	}
	
	void OnDestroy() {
		DestroyImmediate(material);
	}
}