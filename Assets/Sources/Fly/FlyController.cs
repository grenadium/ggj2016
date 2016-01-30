using UnityEngine;
using System.Collections;

public class FlyController : MonoBehaviour {

    private bool isStunned = false;
    private float timeOfStun = 0f; // time at which the fly was hit
    public float stunDuration = 0.2f; // time during which the fly has limited control over its motion

    public float speedCorrection = 1f; // Lower to slow down the fly (when stun) 
    private float worldScale = 100f; // Originally computed from searching the world object

    public float maxSpeed = 120f; // Maximum unscaled speed of the fly (m/s)
    public float sensibility = 0.1f; // value to surpass to apply motion

    public float accelerationCoef = 2 * 9.8f; // Amount of speed gained by sec (approximately twice the acceleration of gravity)
    public float deccelerationCoef = 0.1f; // Amount of speed lost by sec
    public float dodgeBoost = 2f; // Speed boost made by dodging
    public float horizontalCorrectionSpeed = 10f; // speed of correction, in degree/s

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
    }

	void Update ()
    {
        if (!isStunned)
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
        if ((rotation.x = Input.GetAxis("Pitch")) > sensibility || rotation.x < -sensibility)
        {
            transform.Rotate(rotation.x * Vector3.Scale(Vector3.right, Vector3.Scale(maxAngularSpeed, angularCorrection)) * speedCorrection * Time.deltaTime);
        }

        if ((rotation.y = Input.GetAxis("Heading")) > sensibility || rotation.y < -sensibility)
        {
            transform.Rotate(rotation.y * Vector3.Scale(Vector3.up, Vector3.Scale(maxAngularSpeed,angularCorrection)) * speedCorrection * Time.deltaTime);
        }

        if ((rotation.z = Input.GetAxis("Roll")) > sensibility || rotation.z < -sensibility)
        {
            transform.Rotate(rotation.z * Vector3.Scale(Vector3.forward, Vector3.Scale(maxAngularSpeed, angularCorrection)) * speedCorrection * Time.deltaTime);
        }
        else
        {
            // Automatically return back to horizontal to ease things up
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.z = Mathf.MoveTowardsAngle(currentRotation.z, 0, horizontalCorrectionSpeed * Time.deltaTime);
            transform.eulerAngles = currentRotation;
        }
    }

    void    ForwardMotion   ()
    {
        // Natural decceleration (no data on it)
        motion = motion * (1f - deccelerationCoef * worldScale * Time.deltaTime);

        // Maintain "A" to acccelerate
        if(Input.GetButton("Throttle"))
        {
            motion += transform.forward * accelerationCoef * worldScale * Time.deltaTime;
        }

        // Use "B" to make a dodge upward and & X to make it downward
        if(Input.GetButton("DodgeUp"))
        {
            motion += transform.up * dodgeBoost * worldScale;
        }
        else if(Input.GetButton("DodgeDown"))
        {
            motion += -transform.up * dodgeBoost * worldScale;
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
        isStunned = true;
        timeOfStun = Time.realtimeSinceStartup;
        motion = collision.relativeVelocity;
    }

    #endregion
}
