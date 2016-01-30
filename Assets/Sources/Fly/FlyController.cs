using UnityEngine;
using System.Collections;

public class FlyController : MonoBehaviour {

    [System.Serializable]
    public class ControllerMappings
    {
        public string ThrottleAxis = "Throttle";
        public string PitchAxis = "Pitch";
        public string HeadingAxis = "Heading";
        public string RollAxis = "Roll";
        public string StrafeAxis = "Strafe";
        public string VerticalAxis = "TakeOff";

        public string StabilizeButton = "Stabilize";
    }
    public ControllerMappings controllerMappings;

    // State
    public enum FlyState
    {
        FLYING,
        LANDING,
        LANDED,
        STUNNED,
        STABILIZING,
        TAKINGOFF
    }
    private FlyState flyState = FlyState.FLYING;

    // Stun state
    private float timeOfStun = 0f; // time at which the fly was hit
    public float stunDuration = 0.2f; // time during which the fly has limited control over its motion
    public float speedStunCorrection = 1f; // Lower to slow down the fly (when stun) 

    // Landing state
    private float lastRequestedLanding = 0f; // LAst time the trigger was pushed
    private float delayBetweenTriggerAndLanding = 0.8f; // Time necessary between the trigger input & the collision
    private float timeOfLanding = 0f; // Time of landing initiated
    public float landedMaxSpeed = 40f;
    public float landingTime = 0.3f;
    public float tweakingLandedSpeed = 0.2f; // How much of the normal throttle is kept during landed navigation
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
    Vector3 forwardSpeed; // Just a test

    // Angular motion
    public float maxAngleDeccelerated = 10f; // the deccelleration of the angular velocity
    public Vector3 tweakingAngularSpeed = new Vector3(0.1f, 0.1f, 0.1f); // Just to make it playable
    public Vector3 maxAngularSpeed = new Vector3(2160f, 2160f, 2160f); // Yes, that is the top angular speed of a housefly, 6 x 360 turns in 1 second
    Quaternion angularMotion; // rotation velocity induced by user

    public float stabilizationDelay = 0.5f;
    private float timeOfStabilization;
    private Quaternion rotationForStabilization; // rotation necessary to stabilize the roll

    // Taking off
    private float timeOfTakingOff = 0f; // last time a dodge was made
    public float takingOffDelay = 0.5f;
    public float dodgeBoost = 12f; // Speed boost made by takeoff

    // Stabilization (deprecated)
    //public bool enableStabilization = false; // We might want more control
    //public Vector3 angularStabilizationSpeed = new Vector3(2f,2f,2f); // speed of correction, in degree/s, for each axis.

    private Rigidbody flyBody;

    // Debug purpose
    Vector3 forwardVector; // debug purpose
    Vector3 rotation; // pitch / heading / roll

    void Start ()
    {
        flyBody = GetComponent<Rigidbody>();
        GameObject world = GameObject.FindGameObjectWithTag("World");
        worldScale = world.transform.localScale.x; // We suppose scaling is uniform
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
                        transform.rotation = rotationForLanding * rotationOriginal;
                    }
                }
                break;

            // The fly is stabilizing itself
            case FlyState.STABILIZING:
                {
                    transform.rotation = Quaternion.Lerp(rotationOriginal, rotationForStabilization * rotationOriginal, (Time.realtimeSinceStartup - timeOfStabilization) / stabilizationDelay);
                    if ((Time.realtimeSinceStartup - timeOfStabilization) / stabilizationDelay >= 1)
                    {
                        flyState = FlyState.FLYING;
                        transform.rotation = rotationForStabilization * rotationOriginal;
                    }
                }
                break;

            case FlyState.TAKINGOFF:
                {
                    // 
                    forwardSpeed += dodgeBoost * transform.up * tweakingThrottleSpeed * worldScale * Time.deltaTime;
                    if ((Time.realtimeSinceStartup - timeOfTakingOff) / takingOffDelay >= 1)
                    {
                        flyState = FlyState.FLYING;
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
        // Y axis rotation
        if ((rotation.y = Input.GetAxis(controllerMappings.HeadingAxis)) > sensibility || rotation.y < -sensibility)
        {
            Vector3 rotationToApply = rotation.y * Vector3.Scale(Vector3.up, Vector3.Scale(maxAngularSpeed, tweakingAngularSpeed)) * speedStunCorrection * Time.deltaTime;
            transform.Rotate(rotationToApply);
        }

        // X axis rotation
        if ((rotation.x = Input.GetAxis(controllerMappings.PitchAxis)) > sensibility || rotation.x < -sensibility)
        {
            Vector3 rotationToApply = rotation.x * Vector3.Scale(Vector3.right, Vector3.Scale(maxAngularSpeed, tweakingAngularSpeed)) * speedStunCorrection * Time.deltaTime;

            // Keep pitch between 0 & 45 when landed
            if (flyState == FlyState.LANDED)
            {

            }
            else
            {
                transform.Rotate(rotationToApply);
            }
        }

        // Stabilization
        if(Input.GetButton(controllerMappings.StabilizeButton) && flyState == FlyState.FLYING)
        {
            flyState = FlyState.STABILIZING;
            timeOfStabilization = Time.realtimeSinceStartup;

            // No velocity
            forwardSpeed = Vector3.zero;
            rotationOriginal = transform.rotation;
            rotationForStabilization = Quaternion.FromToRotation(transform.up, Vector3.up);
        }

    }

    void    ForwardMotion   ()
    {
        float throttle = 0f;
        if ((throttle = Input.GetAxis(controllerMappings.ThrottleAxis)) > sensibility || throttle < -sensibility)
        {
            forwardSpeed += throttle * transform.forward * (flyState == FlyState.LANDED ? tweakingLandedSpeed : tweakingThrottleSpeed) * accelerationCoef * worldScale * Time.deltaTime;
        }

        float strafe = 0;
        if ((strafe = Input.GetAxis(controllerMappings.StrafeAxis)) > sensibility || strafe < -sensibility)
        {
            forwardSpeed += strafe * transform.right * (flyState == FlyState.LANDED ? tweakingLandedSpeed : tweakingThrottleSpeed) * accelerationCoef * worldScale * Time.deltaTime;
        }

        float verticalMotion = 0f;
        if ((verticalMotion = Input.GetAxis(controllerMappings.VerticalAxis)) > sensibility || verticalMotion < -sensibility)
        {
            if(flyState == FlyState.LANDED)
            {
                flyState = FlyState.TAKINGOFF;
            }
            else
            {
                forwardSpeed += verticalMotion * transform.up * tweakingThrottleSpeed * accelerationCoef * worldScale * Time.deltaTime;
                lastRequestedLanding = Time.realtimeSinceStartup;
            }
        }
    }

    void PhysicalMotion ()
    {
         // Natural decceleration (no data on it, just to add a little inertia)
        forwardSpeed -= forwardSpeed.normalized * deccelerationCoef * worldScale * Time.deltaTime;

        // Make sure we can't go faster than max speed
        if (flyState == FlyState.LANDED)
        {
            if (forwardSpeed.magnitude > tweakingThrottleSpeed * landedMaxSpeed * worldScale)
            {
                forwardSpeed = forwardSpeed.normalized * tweakingThrottleSpeed * landedMaxSpeed * worldScale;
            }
        }
        else
        {
            if (forwardSpeed.magnitude > tweakingThrottleSpeed * maxSpeed * worldScale)
            {
                forwardSpeed = forwardSpeed.normalized * tweakingThrottleSpeed * maxSpeed * worldScale;
            }
        }

        flyBody.velocity = forwardSpeed;
    }

    #endregion

    #region Collision

    void    OnCollisionEnter (Collision collision)
    {
        // Walls & other obstacles
        if (collision.gameObject.layer == LayerMask.NameToLayer("GraspableObject") 
            && flyState == FlyState.FLYING 
            && (Time.realtimeSinceStartup - lastRequestedLanding) / delayBetweenTriggerAndLanding < 1)
        {
            flyState = FlyState.LANDING;
            timeOfLanding = Time.realtimeSinceStartup;

            // No velocity
            forwardSpeed = Vector3.zero;

            // Freezes
            flyBody.angularVelocity = Vector3.zero;
            flyBody.velocity = Vector3.zero;

            // Rotation
            rotationOriginal = transform.rotation;
            landingNormal = collision.contacts[0].normal;
            rotationForLanding = Quaternion.FromToRotation(transform.up, landingNormal);

            transform.Translate(landingNormal.normalized * 0.04f);
        }
        else
        {
            flyState = FlyState.STUNNED;
            timeOfStun = Time.realtimeSinceStartup;

            // Bouncing motion
            forwardSpeed = Vector3.Reflect(-collision.relativeVelocity * worldScale, collision.contacts[0].normal);
            //angularMotion = Random.rotation;
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
