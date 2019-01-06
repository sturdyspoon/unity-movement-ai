using UnityEngine;
using System.Collections.Generic;

namespace UnityMovementAI
{
    [RequireComponent(typeof(MovementAIRigidbody))]
    public class CollisionAvoidance : MonoBehaviour
    {
        public float maxAcceleration = 15f;

        [Tooltip("How much space can be between two characters before they are considered colliding")]
        public float distanceBetween;

        MovementAIRigidbody rb;

        void Awake()
        {
            rb = GetComponent<MovementAIRigidbody>();
        }

        public Vector3 GetSteering(ICollection<MovementAIRigidbody> targets)
        {
            Vector3 acceleration = Vector3.zero;

            /* 1. Find the target that the character will collide with first */

            /* The first collision time */
            float shortestTime = float.PositiveInfinity;

            /* The first target that will collide and other data that
             * we will need and can avoid recalculating */
            MovementAIRigidbody firstTarget = null;
            float firstMinSeparation = 0, firstDistance = 0, firstRadius = 0;
            Vector3 firstRelativePos = Vector3.zero, firstRelativeVel = Vector3.zero;

            foreach (MovementAIRigidbody r in targets)
            {
                /* Calculate the time to collision */
                Vector3 relativePos = rb.ColliderPosition - r.ColliderPosition;
                Vector3 relativeVel = rb.RealVelocity - r.RealVelocity;
                float distance = relativePos.magnitude;
                float relativeSpeed = relativeVel.magnitude;

                if (relativeSpeed == 0)
                {
                    continue;
                }

                float timeToCollision = -1 * Vector3.Dot(relativePos, relativeVel) / (relativeSpeed * relativeSpeed);

                /* Check if they will collide at all */
                Vector3 separation = relativePos + relativeVel * timeToCollision;
                float minSeparation = separation.magnitude;

                if (minSeparation > rb.Radius + r.Radius + distanceBetween)
                {
                    continue;
                }

                /* Check if its the shortest */
                if (timeToCollision > 0 && timeToCollision < shortestTime)
                {
                    shortestTime = timeToCollision;
                    firstTarget = r;
                    firstMinSeparation = minSeparation;
                    firstDistance = distance;
                    firstRelativePos = relativePos;
                    firstRelativeVel = relativeVel;
                    firstRadius = r.Radius;
                }
            }

            /* 2. Calculate the steering */

            /* If we have no target then exit */
            if (firstTarget == null)
            {
                return acceleration;
            }

            /* If we are going to collide with no separation or if we are already colliding then 
             * steer based on current position */
            if (firstMinSeparation <= 0 || firstDistance < rb.Radius + firstRadius + distanceBetween)
            {
                acceleration = rb.ColliderPosition - firstTarget.ColliderPosition;
            }
            /* Else calculate the future relative position */
            else
            {
                acceleration = firstRelativePos + firstRelativeVel * shortestTime;
            }

            /* Avoid the target */
            acceleration = rb.ConvertVector(acceleration);
            acceleration.Normalize();
            acceleration *= maxAcceleration;

            return acceleration;
        }
    }
}