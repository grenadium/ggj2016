using UnityEngine;
using System.Collections;

public class CameraStrip : MonoBehaviour {

	public	Camera	attachedCamera;

    public void ChangeValues(Vector2 _pos, Vector2 _size, float vfov)
    {
		attachedCamera.rect = new Rect(_pos.x, _pos.y, _size.x, _size.y);
		attachedCamera.fieldOfView = vfov;
    }
}
