using UnityEngine;
using System.Collections;

public class SICameraTransformHelper : MonoBehaviour {

	Quaternion temp;
	bool updated = false;

	void Awake () {
		temp = gameObject.transform.rotation;
		gameObject.transform.rotation = new Quaternion();
	}
	
	// Update is called once per frame
	void Update () {
		if (!updated) {
			gameObject.transform.rotation = temp;
			updated = true;
		}
	}
}
