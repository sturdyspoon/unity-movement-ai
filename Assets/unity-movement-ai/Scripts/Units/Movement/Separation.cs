using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MovementAIRigidbody))]
public class Separation : MonoBehaviour {

    /* The maximum acceleration for separation */
    public float sepMaxAcceleration = 25;

    /* This should be the maximum separation distance possible between a separation
     * target and the character.
     * So it should be: separation sensor radius + max target radius */
    public float maxSepDist = 1f;

    private MovementAIRigidbody rb;

    void Awake()
    {
        rb = GetComponent<MovementAIRigidbody>();
    }

    public Vector3 getSteering(ICollection<MovementAIRigidbody> targets)
    {
        Vector3 acceleration = Vector3.zero;

        foreach (MovementAIRigidbody r in targets)
        {
            /* Get the direction and distance from the target */
            Vector3 direction = transform.position - r.position;
            float dist = direction.magnitude;

            if (dist < maxSepDist)
            {
                /* Calculate the separation strength (can be changed to use inverse square law rather than linear) */
                var strength = sepMaxAcceleration * (maxSepDist - dist) / (maxSepDist - rb.boundingRadius - r.boundingRadius);

                /* Added separation acceleration to the existing steering */
                direction.Normalize();
                acceleration += direction * strength;
            }
        }

        return acceleration;
    }
}
