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

    // State
    public enum FlyState
    {
        FLYING,
        LANDING,
        LANDED,
        STUNNED
    }
    private FlyState flyState = FlyState.FLYING;

    // Stun state
    //private bool isStunned = false;
    private float timeOfStun = 0f; // time at which the fly was hit
    public float stunDuration = 0.2f; // time during which the fly has limited control over its motion
    public float speedStunCorrection = 1f; // Lower to slow down the fly (when stun) 

    // Landing state
    //private bool isLanding = false; // Is the fly currently landing ?
    private float timeOfLanding = 0f; // Time of landing initiated
    public float landingTime = 0.3f;
    public float landedSlowCoefficient = 0.2f; // How much of the normal throttle is kept during landed navigation
    private Quaternion rotationOriginal; // Rotation of the fly when landing was initiated, used for interpolation
    private Quaternion rotationForLanding; // Rotation to be made to land properly
    private Vector3 landingNormal; // Normal of the surface used to land

    private float worldScale = 100f; // Originally computed from searching the world object

    // Motion
    public float maxSpeed = 120f; // Maximum unscaled speed of the fly (m/s)
    public float sensibility = 0.1f; // value to surpass to apply motion
    public float tweakingThrottleSpeed = 0.7f; // Used to modify the motion speed to make it usable for human beings
    public float accelerationCoef = 2 * 9.8f; // Amount of speed gained by sec (approximately twice the acceleration of gravity)
    public float deccelerationCoef = 0.1f; // Amount of speed lost by sec

    public Vector3 tweakingAngularSpeed = new Vector3(0.1f, 0.1f, 0.1f); // Just to make it playable
    public Vector3 maxAngularSpeed = new Vector3(2160f, 2160f, 2160f); // Yes, that is the top angular speed of a housefly, 6 x 360 turns in 1 second
    Vector3 inductedMotion; // motion inducted by user
    //Vector3 resultingMotion; // Combination of motions from gravity & inducted motion

    // Dodging
    private float timeOfDodge = 0f; // last time a dodge was made
    public float dodgeDelay = 1f; // We can't just spam dodging
    public Vector3 dodgeBoost = new Vector3(12f, 12f, 12f); // Speed boost made by dodging

    // Stabilization (deprecated)
    public bool enableStabilization = false; // We might want more control
    public Vector3 angularStabilizationSpeed = new Vector3(2f,2f,2f); // speed of correction, in degree/s, for each axis.

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
        switch(flyState)
        {
            case FlyState.LANDING:
                {
                    transform.rotation = Quaternion.Lerp(rotationOriginal, rotationForLanding * rotationOriginal, (Time.realtimeSinceStartup - timeOfLanding) / landingTime);
                    if((Time.realtimeSinceStartup - timeOfLanding) / landingTime >= 1)
                    {
                        flyState = FlyState.LANDED;
                    }
                }
                break;

            case FlyState.LANDED:
            case FlyState.FLYING:
                {
                    LateralMotion(); // Computes angular motion
                    ForwardMotion(); // Computes speed modifcation
                }
                break;

            case FlyState.STUNNED:
                {
                    // Check time left before regaining control
                    if ((Time.realtimeSinceStartup - timeOfStun) / stunDuration >= 1)
                        flyState = FlyState.FLYING;
                }
                break;
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
#else
        if ((rotation.z = Input.GetAxis(controllerMappings.RollAxis)) > sensibility || rotation.z < -sensibility)
        {
            transform.Rotate(rotation.z * Vector3.Scale(Vector3.forward, Vector3.Scale(maxAngularSpeed, tweakingAngularSpeed)) * speedStunCorrection * Time.deltaTime);
        }
        if ((rotation.y = Input.GetAxis(controllerMappings.HeadingAxis)) > sensibility || rotation.y < -sensibility)
        {
            transform.Rotate(rotation.y * Vector3.Scale(Vector3.up, Vector3.Scale(maxAngularSpeed, tweakingAngularSpeed)) * speedStunCorrection * Time.deltaTime);
        }
#endif

#if MVR_INPUT
        if ((rotation.x = userJoystick.GetAxisValue(controllerMappings.PitchAxis)) > sensibility || rotation.x < -sensibility)
#else
        if ((rotation.x = Input.GetAxis(controllerMappings.PitchAxis)) > sensibility || rotation.x < -sensibility)
        {
            Vector3 rotationToApply = (controllerMappings.invertPitch ? -1 : 1) * rotation.x * Vector3.Scale(Vector3.right, Vector3.Scale(maxAngularSpeed, tweakingAngularSpeed)) * speedStunCorrection * Time.deltaTime;

            // Keep pitch between 0 & 45 when landed
            if (flyState == FlyState.LANDED)
            {
                // To fix
                /*float angle = 0;
                if ((angle = (Quaternion.Euler(rotationToApply) * Quaternion.FromToRotation(transform.forward, landingNormal)).eulerAngles.magnitude) > 45 && angle < 90)
                {
                    transform.Rotate(rotationToApply);
                }*/
            }
            else
            {
                transform.Rotate(rotationToApply);
            }
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
        // Natural decceleration (no data on it, just to add a little inertia)
        // Poses some problem with gravity
        inductedMotion = inductedMotion * (1f - deccelerationCoef * worldScale * Time.deltaTime);

        // Maintain "A" to acccelerate
        // Note The axis seems inversed
        float throttle = 0f;
#if MVR_INPUT
        if ((throttle = userJoystick.GetAxisValue(controllerMappings.ThrottleAxis)) > sensibility || throttle < -sensibility)
#else
        if ((throttle = Input.GetAxis(controllerMappings.ThrottleAxis)) > sensibility || throttle < -sensibility)
#endif
        {
            Vector3 motion = throttle * transform.forward * tweakingThrottleSpeed * accelerationCoef * worldScale * Time.deltaTime;
            if(flyState == FlyState.LANDED)
                motion *= landedSlowCoefficient;
            inductedMotion += motion;
        }

        // We can dodge
        if (Time.realtimeSinceStartup - timeOfDodge > dodgeDelay)
        {
#if MVR_INPUT
            // Use "B" to make a dodge upward and & X to make it downward
            if (userJoystick.IsButtonPressed(controllerMappings.DodgeUpButton))
            {
                inductedMotion += Vector3.Scale(transform.up, dodgeBoost) * worldScale;
                timeOfDodge = Time.realtimeSinceStartup;
            }
            else if (userJoystick.IsButtonPressed(controllerMappings.DodgeDownButton))
            {
                inductedMotion += -Vector3.Scale(transform.up, dodgeBoost) * worldScale;
                timeOfDodge = Time.realtimeSinceStartup;
            }
            else if (userJoystick.IsButtonPressed(controllerMappings.DodgeLeftButton))
            {
                inductedMotion += -Vector3.Scale(transform.right, dodgeBoost) * worldScale;
                timeOfDodge = Time.realtimeSinceStartup;
            }
            else if (userJoystick.IsButtonPressed(controllerMappings.DodgeRightButton))
            {
                inductedMotion += Vector3.Scale(transform.right, dodgeBoost) * worldScale;
                timeOfDodge = Time.realtimeSinceStartup;
            }
#else
            // Use "B" to make a dodge upward and & X to make it downward
            if (Input.GetButton(controllerMappings.DodgeUpButton))
            {
                inductedMotion += Vector3.Scale(transform.up, dodgeBoost) * worldScale;
                timeOfDodge = Time.realtimeSinceStartup;

                // Special take-off motion
                if(flyState == FlyState.LANDED)
                {
                    flyState = FlyState.FLYING;
                }
            }
            else if (flyState == FlyState.FLYING && Input.GetButton(controllerMappings.DodgeDownButton))
            {
                inductedMotion += -Vector3.Scale(transform.up, dodgeBoost) * worldScale;
                timeOfDodge = Time.realtimeSinceStartup;
            }
            else if (Input.GetButton(controllerMappings.DodgeLeftButton))
            {
                inductedMotion += -Vector3.Scale(transform.right, dodgeBoost) * worldScale;
                timeOfDodge = Time.realtimeSinceStartup;
            }
            else if (Input.GetButton(controllerMappings.DodgeRightButton))
            {
                inductedMotion += Vector3.Scale(transform.right, dodgeBoost) * worldScale;
                timeOfDodge = Time.realtimeSinceStartup;
            }
#endif
        }

        // Make sure we can't go backward & faster than max speed
        if (inductedMotion.magnitude > maxSpeed * worldScale)
        {
            inductedMotion = inductedMotion.normalized * maxSpeed * worldScale;
        }
    }

    void PhysicalMotion ()
    {
        // Gravity
        if (flyState == FlyState.FLYING || flyState == FlyState.STUNNED)
        {
            inductedMotion += new Vector3(0, -9.8f, 0) * worldScale * Time.deltaTime;
        }

        flyBody.velocity = inductedMotion * speedStunCorrection;
        //transform.Translate(motion * speedCorrection * Time.deltaTime, Space.World);
    }

    #endregion

    #region Collision

    void    OnCollisionEnter (Collision collision)
    {
        // Stun
        if(collision.gameObject.layer == LayerMask.NameToLayer("StunningObject"))
        {
            flyState = FlyState.STUNNED;
            timeOfStun = Time.realtimeSinceStartup;

            // Bouncing motion
            inductedMotion = Vector3.Reflect(-collision.relativeVelocity, collision.contacts[0].normal);
        }
        // Walls & other obstacles
        else if (collision.gameObject.layer == LayerMask.NameToLayer("GraspableObject") && flyState == FlyState.FLYING)
        {
            flyState = FlyState.LANDING;
            timeOfLanding = Time.realtimeSinceStartup;

            // No velocity
            inductedMotion = Vector3.zero;
            rotationOriginal = transform.rotation;
            landingNormal = collision.contacts[0].normal;
            rotationForLanding = Quaternion.FromToRotation(transform.up, landingNormal);
        }

    }

    void OnCollisionExit (Collision collision)
    {
        if(flyState == FlyState.LANDED)
        {
            flyState = FlyState.FLYING;
        }
    }

    #endregion
}
