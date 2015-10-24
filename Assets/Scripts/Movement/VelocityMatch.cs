using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SteeringBasics))]
public class VelocityMatch : MonoBehaviour
{

    public float facingCosine = 90;
    public float timeToTarget = 0.1f;
    public float maxAcceleration = 4f;

    private float facingCosineVal;

    private Rigidbody rb;
    private SteeringBasics steeringBasics;

    // Use this for initialization
    void Start()
    {
        facingCosineVal = Mathf.Cos(facingCosine * Mathf.Deg2Rad);

        rb = GetComponent<Rigidbody>();
        steeringBasics = GetComponent<SteeringBasics>();
    }

    public Vector3 getSteering(ICollection<Rigidbody> targets)
    {
        Vector3 accel = Vector3.zero;
        int count = 0;

        foreach (Rigidbody r in targets)
        {
            if (steeringBasics.isFacing(r.position, facingCosineVal))
            {
                /* Calculate the acceleration we want to match this target */
                Vector3 a = r.velocity - rb.velocity;
                /*
                 Rather than accelerate the character to the correct speed in 1 second, 
                 accelerate so we reach the desired speed in timeToTarget seconds 
                 (if we were to actually accelerate for the full timeToTarget seconds).
                */
                a = a / timeToTarget;

                accel += a;

                count++;
            }
        }

        if (count > 0)
        {
            accel = accel / count;

            /* Make sure we are accelerating at max acceleration */
            if (accel.magnitude > maxAcceleration)
            {
                accel = accel.normalized * maxAcceleration;
            }
        }

        return accel;
    }
}
