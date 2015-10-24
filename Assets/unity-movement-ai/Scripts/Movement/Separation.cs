using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Separation : MonoBehaviour {

    /* The maximum acceleration for separation */
    public float sepMaxAcceleration = 25;

    /* This should be the maximum separation distance possible between a separation
     * target and the character.
     * So it should be: separation sensor radius + max target radius */
    public float maxSepDist = 1f;

    private float boundingRadius;

    // Use this for initialization
    void Start()
    {
        boundingRadius = SteeringBasics.getBoundingRadius(transform);
    }

    public Vector3 getSteering(ICollection<Rigidbody> targets)
    {
        Vector3 acceleration = Vector3.zero;

        foreach (Rigidbody r in targets)
        {
            /* Get the direction and distance from the target */
            Vector3 direction = transform.position - r.position;
            float dist = direction.magnitude;

            if (dist < maxSepDist)
            {
                float targetRadius = SteeringBasics.getBoundingRadius(r.transform);

                /* Calculate the separation strength (can be changed to use inverse square law rather than linear) */
                var strength = sepMaxAcceleration * (maxSepDist - dist) / (maxSepDist - boundingRadius - targetRadius);

                /* Added separation acceleration to the existing steering */
                direction.Normalize();
                acceleration += direction * strength;
            }
        }

        return acceleration;
    }
}
