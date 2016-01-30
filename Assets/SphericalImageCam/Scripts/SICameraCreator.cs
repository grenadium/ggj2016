//SICameraCreater.cs
//
//Copyright (c) 2015 blkcatman
//
using UnityEngine;
using System.Collections;

public class SICameraCreator : SICameraBase {
	public bool setClippingPlanes = true;
	public float nearCllipingPlane = 0.1f;
	public float farCllipingPlane = 1000f;
	public bool copyComponents = false;
	protected bool forceUpdateParameters = false;//debug code;
	
	protected GameObject CreateCamera(Vector3 EularAngle, float fov, float depth, string name) {
		float rotX = EularAngle.x;
		float rotY = EularAngle.y;
		float rotZ = EularAngle.z;
		
		GameObject dummy = new GameObject();
		dummy.name = name;
		dummy.transform.Rotate(new Vector3(0f, 0f, 1f), rotZ);
		dummy.transform.Rotate(new Vector3(0f, 1f, 0f), rotY);
		dummy.transform.Rotate(new Vector3(1f, 0f, 0f), rotX);
		dummy.transform.SetParent(gameObject.transform, false);
		//dummy.hideFlags = HideFlags.HideInHierarchy;
		
		Camera cam = dummy.AddComponent<Camera>();
		cam.aspect = 1f;
		cam.fieldOfView = fov;
		if(setClippingPlanes) {
			cam.nearClipPlane = nearCllipingPlane;
			cam.farClipPlane = farCllipingPlane;
		}
		cam.backgroundColor = base.parentCamera.backgroundColor;
		cam.rect = new Rect(0f, 0f, 1f, 1f);
		cam.depth = depth;
		
		Material mat = new Material(shader);
		mat.SetVector("_forward", dummy.transform.forward);
		mat.SetVector("_right", dummy.transform.right);
		mat.SetVector("_up", dummy.transform.up);
		mat.SetFloat("_fov", fov);
		cam.targetTexture = targetTexture;

		if (copyComponents) {
			Component[] cs = gameObject.GetComponents<Component> ();
			for (int j = 0; j<cs.Length; j++) {
				System.Type type = cs [j].GetType ();
				if (!SIUtils.CheckTypes (type)) {
					continue;
				}
			
				Component copy = dummy.AddComponent (type);
				System.Reflection.FieldInfo[] fields = type.GetFields (); 
				foreach (System.Reflection.FieldInfo field in fields) {
					field.SetValue (copy, field.GetValue (cs [j]));
				}
			}
		}
		
		SIRenderEvent ev = dummy.AddComponent<SIRenderEvent>();
		ev.ref_camera = cam;
		ev.material = mat;
		ev.forceUpdateParameters = forceUpdateParameters;
		
		return dummy;
	}
}
