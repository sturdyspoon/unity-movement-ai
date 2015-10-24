using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Flee : MonoBehaviour {

    public float panicDist = 3.5f;

    public bool decelerateOnStop = true;

    public float maxAcceleration = 10f;

    public float timeToTarget = 0.1f;

    private Rigidbody rb;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
	}

    /* A flee steering behavior. Will return the steering for the current game object to flee a given position */
    public Vector3 getSteering(Vector3 targetPosition)
    {
        //Get the direction
        Vector3 acceleration = transform.position - targetPosition;

        //If the target is far way then don't flee
        if (acceleration.magnitude > panicDist)
        {
            //Slow down if we should decelerate on stop
            if (decelerateOnStop && rb.velocity.magnitude > 0.001f)
            {
                //Decelerate to zero velocity in time to target amount of time
                acceleration = -rb.velocity / timeToTarget;

                if (acceleration.magnitude > maxAcceleration)
                {
                    acceleration = giveMaxAccel(acceleration);
                }

                return acceleration;
            }
            else
            {
                rb.velocity = Vector2.zero;
                return Vector3.zero;
            }
        }

        return giveMaxAccel(acceleration);
    }

    private Vector3 giveMaxAccel(Vector3 v)
    {
        //Remove the z coordinate
        v.z = 0;

        v.Normalize();

        //Accelerate to the target
        v *= maxAcceleration;

        return v;
    }
}
