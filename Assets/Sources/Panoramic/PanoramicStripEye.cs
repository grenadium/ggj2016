using UnityEngine;
using System.Collections;

public class PanoramicStripEye : MonoBehaviour
{
	public 	GameObject 		cameraPrefab;
	public 	RenderTexture 	mainEye;
    public 	Transform 		head; // Serves as pivot for eye position
	public 	Transform 		pivot;

    public 	float 		eyeOffset 		= 0.1f; // distance eye-head center in cm
	public	float 		focalLength 	= 1f;// For safe viewing, Paul Bourke recommends Eye separation to be at 1/25 of the focal length
	public	Vector2		imageSize		= new Vector2(2048,2048);

    public 	float 		horizontalFov 	= 5f; // Minimal Fov of each camera
    public 	float 		verticalFov 	= 90f;
    public 	float 		marginFov 		= 0f; // Additional fov

	public 	bool 		toeInCameras; // Wether or not cameras should be oriented
	public 	bool 		enableZeroParallax; // Useless if oeInCameras is enabled
	public 	bool 		monoscopic = false;
	public	bool		fadeInDepthAtPoles	=	false; // Decrease eye disparity when eyes get closer to poles

	public	bool		camerasAreActive = false;

    CameraStrip[] 	leftEyePositions; // Various cameras
    CameraStrip[] 	rightEyePositions;


	int 		nbOfStrips;
	int			nbOfCamerasByStrip;
	Vector2 	cameraPixelSize;

    void Awake()
    {

		nbOfStrips = (int)(360f / horizontalFov);
		nbOfCamerasByStrip = (int)(180f / verticalFov);
		leftEyePositions = new CameraStrip[nbOfStrips * nbOfCamerasByStrip];

        if (!monoscopic)
			rightEyePositions = new CameraStrip[nbOfStrips * nbOfCamerasByStrip];

        cameraPixelSize = new Vector2(horizontalFov / 360f, (verticalFov / 180f) * (monoscopic ? 1f : 0.5f));// 0.5 is for stereo up/down

		float rotationOffsetAngle = 0f;

		// Percentage of the space taken by the view
		Vector2 partOfView = new Vector2(horizontalFov / 360f, verticalFov / 180f);


		// Toe-*in rotation
		if (toeInCameras && eyeOffset > 0f)
		{
			rotationOffsetAngle = 90f - Mathf.Rad2Deg * Mathf.Atan(2 * focalLength / eyeOffset);
		}
		
		// Orient correctly the camera and move it to half the eye separation
		Quaternion rotOffset;
		Vector3 trOffset = new Vector3(eyeOffset * 0.5f, 0, 0);

		for (int i = 0; i < nbOfStrips; i++)
        {
			for (int j = 0; j < nbOfCamerasByStrip; j++)
            {

				// In top-bottom configuration with a square texture, we have 1/2 the vertical size
                float y = j * partOfView.y * (monoscopic ? 1f : 0.5f);
				float x = i * partOfView.x;

				// Depth modificator (to fade depth at poles)
				float fadeMult = 1f;
				if(fadeInDepthAtPoles)
				{
					// fadecoef = cosinus between eye orientation and poles
					// Beginning by the north pole and toward the south
					fadeMult = Mathf.Cos(Mathf.Deg2Rad * (90f - j * verticalFov - verticalFov * 0.5f));
				}

				// Create eye
				// Size is Index of Camera * part of 
				rotOffset = Quaternion.AngleAxis(fadeMult * rotationOffsetAngle, head.up);
				createEye(i,j, rotOffset, fadeMult * trOffset, ref leftEyePositions, "Left",
                    new Vector2(x, (monoscopic ? 0f : 0.5f) + y),
                            90f - j * verticalFov - verticalFov * 0.5f
				          );

				// Right eye
                if (!monoscopic)
                {
					// Needs to be aligned if not toed-in and if we want a point of Zero Parallax
					// Align stereo pairs if required
					if(!toeInCameras && enableZeroParallax)
					{
						float theta = 2f * Mathf.Asin(0.5f * fadeMult * eyeOffset / focalLength);
						x -= theta / (2f * Mathf.PI);
					}

					rotOffset = Quaternion.AngleAxis(-fadeMult * rotationOffsetAngle, head.up);
					createEye(i,j, rotOffset, -fadeMult * trOffset, ref rightEyePositions, "Right",
					          new Vector2(x, y),
					          90f - j * verticalFov - verticalFov * 0.5f);
                }
            }

        }
    }

    void createEye(int _i, int _j,
                    Quaternion _rotOffset, // What does it do ?
                    Vector3 _trOffset, // Eye distance offset
                    ref CameraStrip[] _eyePositions,
                    string _eye,
                    Vector2 screenPosition,
                    float horizontalAngle)
    {

        GameObject go = Instantiate(cameraPrefab,
                                    head.position + Quaternion.AngleAxis(_i * horizontalFov, head.up) * _trOffset,
		                            pivot.rotation * _rotOffset * Quaternion.AngleAxis(_i * horizontalFov, head.up)) as GameObject;

        go.name = _eye + "_Cam_h" + _i + "_v" + _j;

        go.transform.parent = pivot;
        _eyePositions[_i * nbOfCamerasByStrip  + _j] = go.GetComponent<CameraStrip>();
		_eyePositions[_i * nbOfCamerasByStrip + _j].attachedCamera.targetTexture = mainEye;

        go.transform.Rotate(new Vector3(horizontalAngle, 0, 0));

		_eyePositions[_i * nbOfCamerasByStrip + _j].ChangeValues(
			screenPosition, // Position X
			cameraPixelSize,
           	verticalFov
       );
		
		_eyePositions[_i * nbOfCamerasByStrip + _j].attachedCamera.enabled = camerasAreActive;
 
    }

	public string ScreenShotName() {
		return string.Format("{0}/{1}_{2}x{3}_{4}{5}{6}{7}.png",
		                     Application.dataPath, 
		                     monoscopic ? "Mono" : "Stereo",
		                     imageSize.x, imageSize.y,
		                     fadeInDepthAtPoles ? "Faded_" : "",
		                     toeInCameras ? "ToedIn_" : "",
		                     (toeInCameras || enableZeroParallax) ? "f" + focalLength.ToString()+ "_" : "",
		                     System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
	}

	public	void	takeScreenshot	()
	{
		Texture2D screen = new Texture2D((int)imageSize.x, (int)imageSize.y, TextureFormat.RGB24, false);

		if(!camerasAreActive)
		{
			for(int i=0; i<leftEyePositions.Length; i++)
			{
				leftEyePositions[i].attachedCamera.Render();
				rightEyePositions[i].attachedCamera.Render();
			}

			RenderTexture.active = mainEye;
			screen.ReadPixels(new Rect(0, 0, imageSize.x, imageSize.y), 0, 0);
			RenderTexture.active = null;

			// Write data
			byte[] bytes = screen.EncodeToPNG();
			string filename = ScreenShotName();
			//System.IO.File.WriteAllBytes(filename, bytes);

		}
	}
}
