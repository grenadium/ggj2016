using UnityEngine;
using System.Collections;

public class FlyController : MonoBehaviour {

    public const float maxSpeed = 20f; // MAximum speed of the fly
    public const float sensibility = 0.01f; // value to surpass to apply motion

    public float accelerationCoef = 2f; // Amount of speed gained by sec
    public float deccelerationCoef = 0.1f; // Amount of speed lost by sec

    public Vector3 angularSpeed = new Vector3(2f, 2f, 2f); // amount of rotation made by s
    public Vector3 motion; // forward momemtum

    // Debug purpose
    float accel;
    float deccel;
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
    }

    void    ForwardMotion   ()
    {
        // Natural decceleration
        motion = motion * (1f - deccelerationCoef * Time.deltaTime);

        // Acceleration
        if ((accel = Input.GetAxis("Throttle")) > 0.01f)
        {
            motion += transform.forward * accel * accelerationCoef * Time.deltaTime;
        }
        // Manual decceleration
        else if ((deccel = Input.GetAxis("Throttle")) < -0.01f)
        {
            motion += transform.forward * deccel * accelerationCoef * Time.deltaTime;
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
