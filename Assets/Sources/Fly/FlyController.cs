using UnityEngine;
using System.Collections;

public class FlyController : MonoBehaviour {

    [System.Serializable]
    public class ControllerMappings
    {
#if MVR_INPUT
        public uint ThrottleAxis = 5; // Right joystick forward
        public uint HeadingAxis = 0; // Left joystick right
        public uint PitchAxis = 1; // Left joystick forward
        public uint RollAxis = 2; // Triggers

        public uint DodgeDownButton = 0; // A
        public uint DodgeUpButton = 3; // Y
        public uint DodgeLeftButton = 2; // X
        public uint DodgeRightButton = 1; // B
#else
        public string ThrottleAxis = "Throttle";
        public string PitchAxis = "Pitch";
        public string HeadingAxis = "Heading";
        public string RollAxis = "Roll";

        public string DodgeDownButton = "DodgeDown"; // A
        public string DodgeUpButton = "DodgeUp"; // Y
        public string DodgeLeftButton = "DodgeLeft"; // X
        public string DodgeRightButton = "DodgeRight"; // B
#endif
        public bool invertPitch = false;
    }
    public ControllerMappings controllerMappings;

#if MVR_INPUT
    private vrJoystick userJoystick = null; // MVR controller
#endif

    private bool isStunned = false;
    private float timeOfStun = 0f; // time at which the fly was hit
    public float stunDuration = 0.2f; // time during which the fly has limited control over its motion

    public float speedCorrection = 1f; // Lower to slow down the fly (when stun) 
    private float worldScale = 100f; // Originally computed from searching the world object

    public float maxSpeed = 120f; // Maximum unscaled speed of the fly (m/s)
    public float sensibility = 0.1f; // value to surpass to apply motion

    public float accelerationCoef = 2 * 9.8f; // Amount of speed gained by sec (approximately twice the acceleration of gravity)
    public float deccelerationCoef = 0.1f; // Amount of speed lost by sec

    private float timeOfDodge = 0f; // last time a dodge was made
    public float dodgeDelay = 2f; // We can't just spam dodging
    public Vector3 dodgeBoost = new Vector3(2f, 2f, 2f); // Speed boost made by dodging

    private bool isLanding = false; // Is the fly currently landing ?
    private float timeOfLanding = 0f; // Time of landing initiated
    public float landingTime = 2f;
    private Quaternion rotationOriginal; // Rotation of the fly when landing was initiated, used for interpolation
    private Quaternion rotationForLanding; // Rotation to be made to land properly

    public bool enableStabilization = false; // We might want more control
    public Vector3 angularStabilizationSpeed = new Vector3(2f,2f,2f); // speed of correction, in degree/s, for each axis.

    public Vector3 angularCorrection = new Vector3(0.5f, 0.5f, 0.5f); // Just to make it playable
    public Vector3 maxAngularSpeed = new Vector3(2160f, 2160f, 2160f); // Yes, that is the top angular speed of a housefly, 6 x 360 turns in 1 second
    Vector3 motion; // Speed & direction of motion

    private Rigidbody flyBody;

    // Debug purpose
    Vector3 forwardVector; // debug purpose
    Vector3 rotation; // pitch / heading / roll

    void Start ()
    {
        flyBody = GetComponent<Rigidbody>();
        GameObject world = GameObject.FindGameObjectWithTag("World");
        worldScale = world.transform.localScale.x; // We suppose scaling is uniform

#if MVR_INPUT
        // Hack for getting XBOX Controller
        vrDeviceManager deviceMgr = MiddleVR.VRDeviceMgr;
        for (uint i = 0, iEnd = deviceMgr.GetJoysticksNb(); i < iEnd; ++i)
        {
            vrJoystick joystick = deviceMgr.GetJoystick(i);
            if (joystick.GetName().StartsWith("Controller"))
            {
                userJoystick = joystick;
                break;
            }
        }
#endif
    }

	void Update ()
    {
        if (isLanding)
        {
            transform.rotation = Quaternion.Lerp(rotationOriginal, rotationForLanding * rotationOriginal, (Time.realtimeSinceStartup - timeOfLanding) / landingTime);
            if((Time.realtimeSinceStartup - timeOfLanding) / landingTime >= 1)
            {
                isLanding = false;
            }
        }
        else if (!isStunned)
        {
            LateralMotion(); // Computes angular motion
            ForwardMotion(); // Computes speed modifcation
        }
        else
        {
            // Check time left before regaining control
            if ((Time.realtimeSinceStartup - timeOfStun) / stunDuration >= 1)
                isStunned = false;
        }

        PhysicalMotion(); // Apply speed to physical world

        forwardVector = transform.forward;
	}

    #region Navigation

    void    LateralMotion  ()
    {
#if MVR_INPUT
        if ((rotation.z = userJoystick.GetAxisValue(controllerMappings.RollAxis)) > sensibility || rotation.z < -sensibility)
        {
            transform.Rotate(rotation.z * Vector3.Scale(Vector3.forward, Vector3.Scale(maxAngularSpeed, angularCorrection)) * speedCorrection * Time.deltaTime);
        }
        if ((rotation.y = userJoystick.GetAxisValue(controllerMappings.HeadingAxis)) > sensibility || rotation.y < -sensibility)
        {
            transform.Rotate(rotation.y * Vector3.Scale(Vector3.up, Vector3.Scale(maxAngularSpeed,angularCorrection)) * speedCorrection * Time.deltaTime);
        }
        if ((rotation.x = userJoystick.GetAxisValue(controllerMappings.PitchAxis)) > sensibility || rotation.x < -sensibility)
        {
            transform.Rotate((controllerMappings.invertPitch ? -1 : 1) * rotation.x * Vector3.Scale(Vector3.right, Vector3.Scale(maxAngularSpeed, angularCorrection)) * speedCorrection * Time.deltaTime);
        }
#else
        if ((rotation.z = Input.GetAxis(controllerMappings.RollAxis)) > sensibility || rotation.z < -sensibility)
        {
            transform.Rotate(rotation.z * Vector3.Scale(Vector3.forward, Vector3.Scale(maxAngularSpeed, angularCorrection)) * speedCorrection * Time.deltaTime);
        }
        if ((rotation.y = Input.GetAxis(controllerMappings.HeadingAxis)) > sensibility || rotation.y < -sensibility)
        {
            transform.Rotate(rotation.y * Vector3.Scale(Vector3.up, Vector3.Scale(maxAngularSpeed, angularCorrection)) * speedCorrection * Time.deltaTime);
        }
        if ((rotation.x = Input.GetAxis(controllerMappings.PitchAxis)) > sensibility || rotation.x < -sensibility)
        {
            transform.Rotate((controllerMappings.invertPitch ? -1 : 1) * rotation.x * Vector3.Scale(Vector3.right, Vector3.Scale(maxAngularSpeed, angularCorrection)) * speedCorrection * Time.deltaTime);
        }
#endif
        else if (enableStabilization)
        {
            // Automatically return back to horizontal to ease things up
            //Vector3 currentRotation = transform.eulerAngles;
            //currentRotation.z = Mathf.MoveTowardsAngle(currentRotation.z, 0, horizontalCorrectionSpeed * Time.deltaTime);
            //transform.eulerAngles = currentRotation;

            // Multi-axis rotation
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.identity, angularStabilizationSpeed.x * Time.deltaTime);
        }
    }

    void    ForwardMotion   ()
    {
        // Natural decceleration (no data on it)
        motion = motion * (1f - deccelerationCoef * worldScale * Time.deltaTime);

        // Maintain "A" to acccelerate
        // Note The axis seems inversed
        float throttle = 0f;
#if MVR_INPUT
        if ((throttle = userJoystick.GetAxisValue(controllerMappings.ThrottleAxis)) > sensibility || throttle < -sensibility)
        {
            motion += -throttle * transform.forward * accelerationCoef * worldScale * Time.deltaTime;
        }
#else
        if ((throttle = Input.GetAxis(controllerMappings.ThrottleAxis)) > sensibility || throttle < -sensibility)
        {
            motion += -throttle * transform.forward * accelerationCoef * worldScale * Time.deltaTime;
        }
#endif

        // We can dodge
        if (Time.realtimeSinceStartup - timeOfDodge > dodgeDelay)
        {
#if MVR_INPUT
            // Use "B" to make a dodge upward and & X to make it downward
            if (userJoystick.IsButtonPressed(controllerMappings.DodgeUpButton))
            {
                motion += Vector3.Scale(transform.up, dodgeBoost) * worldScale;
                timeOfDodge = Time.realtimeSinceStartup;
            }
            else if (userJoystick.IsButtonPressed(controllerMappings.DodgeDownButton))
            {
                motion += -Vector3.Scale(transform.up, dodgeBoost) * worldScale;
                timeOfDodge = Time.realtimeSinceStartup;
            }
            else if (userJoystick.IsButtonPressed(controllerMappings.DodgeLeftButton))
            {
                motion += -Vector3.Scale(transform.right, dodgeBoost) * worldScale;
                timeOfDodge = Time.realtimeSinceStartup;
            }
            else if (userJoystick.IsButtonPressed(controllerMappings.DodgeRightButton))
            {
                motion += Vector3.Scale(transform.right, dodgeBoost) * worldScale;
                timeOfDodge = Time.realtimeSinceStartup;
            }
#else
            // Use "B" to make a dodge upward and & X to make it downward
            if (Input.GetButton(controllerMappings.DodgeUpButton))
            {
                motion += Vector3.Scale(transform.up, dodgeBoost) * worldScale;
                timeOfDodge = Time.realtimeSinceStartup;
            }
            else if (Input.GetButton(controllerMappings.DodgeDownButton))
            {
                motion += -Vector3.Scale(transform.up, dodgeBoost) * worldScale;
                timeOfDodge = Time.realtimeSinceStartup;
            }
            else if (Input.GetButton(controllerMappings.DodgeLeftButton))
            {
                motion += -Vector3.Scale(transform.right, dodgeBoost) * worldScale;
                timeOfDodge = Time.realtimeSinceStartup;
            }
            else if (Input.GetButton(controllerMappings.DodgeRightButton))
            {
                motion += Vector3.Scale(transform.right, dodgeBoost) * worldScale;
                timeOfDodge = Time.realtimeSinceStartup;
            }
#endif
        }

        // Make sure we can't go backward & faster than max speed
        if (motion.magnitude > maxSpeed * worldScale)
        {
            motion = motion.normalized * maxSpeed * worldScale;
        }
    }

    void PhysicalMotion ()
    {
        flyBody.velocity = motion * speedCorrection;
        //transform.Translate(motion * speedCorrection * Time.deltaTime, Space.World);
    }

    #endregion

    #region Collision

    void    OnCollisionEnter (Collision collision)
    {
        // Stun
        if(collision.gameObject.layer == LayerMask.NameToLayer("StunningObject"))
        {
            isStunned = true;
            timeOfStun = Time.realtimeSinceStartup;

            // Bouncing motion
            motion = Vector3.Reflect(-collision.relativeVelocity, collision.contacts[0].normal);
        }
        // Walls & other obstacles
        else if (collision.gameObject.layer == LayerMask.NameToLayer("GraspableObject"))
        {
            isLanding = true;
            timeOfLanding = Time.realtimeSinceStartup;

            // No velocity
            motion = Vector3.zero;
            rotationOriginal = transform.rotation;
            rotationForLanding = Quaternion.FromToRotation(transform.up, collision.contacts[0].normal);
        }

    }

    #endregion
}
