using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class FlyMovement : NetworkBehaviour
{
    private vrJoystick m_Joystick = null;

	// Use this for initialization
	void Start ()
	{
	    vrDeviceManager deviceMgr = MiddleVR.VRDeviceMgr;
        // Get XBOX Controller
        for (uint i=0, iEnd=deviceMgr.GetJoysticksNb(); i<iEnd; ++i)
	    {
            vrJoystick joystick = deviceMgr.GetJoystick(i);
	        if (joystick.GetName().StartsWith("Controller"))
	        {
	            m_Joystick = joystick;
	            break;
	        }
	    }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!isLocalPlayer)
            return;
        
        if (m_Joystick != null)
	    {
            var x = m_Joystick.GetAxisValue(0) * 0.1f;
            var y = m_Joystick.GetAxisValue(1) * 0.1f;
            var b = m_Joystick.IsButtonPressed(0);
           

            transform.Translate(x, 0, y);
        }
    }
}
