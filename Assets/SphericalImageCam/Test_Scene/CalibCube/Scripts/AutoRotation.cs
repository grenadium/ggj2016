using UnityEngine;
using System.Collections;

public class AutoRotation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	public bool isRotate;
	public bool isScale;
	
	// Update is called once per frame
	void FixedUpdate () {
		if (isRotate) {
			transform.Rotate(new Vector3 (0f, 1f, 0f) * Time.deltaTime * 10f);
		}
		if (isScale) {
			transform.localScale = new Vector3(1f, 1f, 1f) * 
				((Mathf.Sin(Time.time*Mathf.PI*0.1f)+1.0f)*31.0f + 0.256f);
		}
		if(Input.GetKeyDown(KeyCode.Space)) {
			isRotate = !isRotate;
		}
	}
}
