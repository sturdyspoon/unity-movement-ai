using UnityEngine;
using System.Collections.Generic;

namespace UnityMovementAI
{
    [RequireComponent(typeof(MovementAIRigidbody))]
    public class Separation : MonoBehaviour
    {
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

        public Vector3 GetSteering(ICollection<MovementAIRigidbody> targets)
        {
            Vector3 acceleration = Vector3.zero;

            foreach (MovementAIRigidbody r in targets)
            {
                /* Get the direction and distance from the target */
                Vector3 direction = rb.colliderPosition - r.colliderPosition;
                float dist = direction.magnitude;

                if (dist < maxSepDist)
                {
                    /* Calculate the separation strength (can be changed to use inverse square law rather than linear) */
                    var strength = sepMaxAcceleration * (maxSepDist - dist) / (maxSepDist - rb.radius - r.radius);

                    /* Added separation acceleration to the existing steering */
                    direction = rb.ConvertVector(direction);
                    direction.Normalize();
                    acceleration += direction * strength;
                }
            }

            return acceleration;
        }
    }
}