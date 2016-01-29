using UnityEngine;
using System.Collections;

public class FlyController : MonoBehaviour {

    public const float maxSpeed = 20f; // Maximum speed of the fly
    public const float sensibility = 0.1f; // value to surpass to apply motion

    public float accelerationCoef = 2f; // Amount of speed gained by sec
    public float deccelerationCoef = 0.1f; // Amount of speed lost by sec
    public float dodgeBoost = 2f; // Speed boost made by dodging
    public float horizontalCorrectionSpeed = 10f; // speed of correction, in degree/s

    public Vector3 angularSpeed = new Vector3(2f, 2f, 2f); // amount of rotation made by s
    Vector3 motion; // Speed & direction of motion

    // Debug purpose
    Vector3 forwardVector; // debug purpose
    Vector3 rotation; // pitch / heading / roll

	void Update ()
    {
        // Orientation then propulsion
        LateralMotion();
        ForwardMotion();

        forwardVector = transform.forward;
	}

    #region Navigation

    void    LateralMotion  ()
    {
        if ((rotation.x = Input.GetAxis("Pitch")) > sensibility || rotation.x < -sensibility)
        {
            transform.Rotate(rotation.x * Vector3.Scale(Vector3.right, angularSpeed) * Time.deltaTime);
        }

        if ((rotation.y = Input.GetAxis("Heading")) > sensibility || rotation.y < -sensibility)
        {
            transform.Rotate(rotation.y * Vector3.Scale(Vector3.up, angularSpeed) * Time.deltaTime);
        }

        if ((rotation.z = Input.GetAxis("Roll")) > sensibility || rotation.z < -sensibility)
        {
            transform.Rotate(rotation.z * Vector3.Scale(Vector3.forward, angularSpeed) * Time.deltaTime);
        }
        else
        {
            // Automatically return back to horizontal
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.z = Mathf.MoveTowardsAngle(currentRotation.z, 0, horizontalCorrectionSpeed * Time.deltaTime);
            transform.eulerAngles = currentRotation;
        }
    }

    void    ForwardMotion   ()
    {
        // Natural decceleration
        motion = motion * (1f - deccelerationCoef * Time.deltaTime);

        // Maintain "A" to acccelerate
        if(Input.GetButton("Throttle"))
        {
            motion += transform.forward * accelerationCoef * Time.deltaTime;
        }

        // Use "B" to make a dodge upward and &Xé to make it downward
        if(Input.GetButton("DodgeUp"))
        {
            motion += transform.up * dodgeBoost;
        }
        else if(Input.GetButton("DodgeDown"))
        {
            motion += -transform.up * dodgeBoost;
        }

        // Make sure we can't go backward & faster than max speed
        if(motion.magnitude > maxSpeed)
        {
            motion = motion.normalized * maxSpeed;
        }

        transform.Translate(motion * Time.deltaTime, Space.World);
    }

    #endregion
}
