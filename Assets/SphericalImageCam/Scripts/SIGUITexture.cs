//SIGUITexture.cs
//
//Copyright (c) 2015 blkcatman
//
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SIGUITexture : MonoBehaviour {

	public RenderTexture target;
	bool clipChanged = false;

	void OnGUI () {
		if (target != null) {
			if(!clipChanged) {
				Camera main = gameObject.GetComponent<Camera>();
				if(main) {
					main.nearClipPlane = 0.01f;
					main.farClipPlane = 0.02f;
				}
				clipChanged = true;
			}

			GUI.depth = 2;
			GUI.DrawTexture(new Rect(0.0f, 0.0f, Screen.width, Screen.height), target);
		}
	}
}
