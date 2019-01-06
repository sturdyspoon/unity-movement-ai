using UnityEngine;
using System.Collections.Generic;

namespace UnityMovementAI
{
    [RequireComponent(typeof(MovementAIRigidbody))]
    public class Separation : MonoBehaviour
    {
        /// <summary>
        /// The maximum acceleration for separation
        /// </summary>
        public float sepMaxAcceleration = 25;

        /// <summary>
        /// This should be the maximum separation distance possible between a
        /// separation target and the character. So it should be: separation
        /// sensor radius + max target radius
        /// </summary>
        public float maxSepDist = 1f;

        MovementAIRigidbody rb;

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
                Vector3 direction = rb.ColliderPosition - r.ColliderPosition;
                float dist = direction.magnitude;

                if (dist < maxSepDist)
                {
                    /* Calculate the separation strength (can be changed to use inverse square law rather than linear) */
                    var strength = sepMaxAcceleration * (maxSepDist - dist) / (maxSepDist - rb.Radius - r.Radius);

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