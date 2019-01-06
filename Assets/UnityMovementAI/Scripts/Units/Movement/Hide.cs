using UnityEngine;
using System.Collections.Generic;

namespace UnityMovementAI
{
    [RequireComponent(typeof(SteeringBasics))]
    [RequireComponent(typeof(Evade))]
    public class Hide : MonoBehaviour
    {
        public float distanceFromBoundary = 0.6f;

        SteeringBasics steeringBasics;
        Evade evade;

        void Awake()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            evade = GetComponent<Evade>();
        }

        public Vector3 GetSteering(MovementAIRigidbody target, ICollection<MovementAIRigidbody> obstacles)
        {
            Vector3 bestHidingSpot;
            return GetSteering(target, obstacles, out bestHidingSpot);
        }

        public Vector3 GetSteering(MovementAIRigidbody target, ICollection<MovementAIRigidbody> obstacles, out Vector3 bestHidingSpot)
        {
            /* Find the closest hiding spot. */
            float distToClostest = Mathf.Infinity;
            bestHidingSpot = Vector3.zero;

            foreach (MovementAIRigidbody r in obstacles)
            {
                Vector3 hidingSpot = GetHidingPosition(r, target);

                float dist = Vector3.Distance(hidingSpot, transform.position);

                if (dist < distToClostest)
                {
                    distToClostest = dist;
                    bestHidingSpot = hidingSpot;
                }
            }

            /* If no hiding spot is found then just evade the enemy. */
            if (distToClostest == Mathf.Infinity)
            {
                return evade.GetSteering(target);
            }

            //Debug.DrawLine(transform.position, bestHidingSpot);

            return steeringBasics.Arrive(bestHidingSpot);
        }

        Vector3 GetHidingPosition(MovementAIRigidbody obstacle, MovementAIRigidbody target)
        {
            float distAway = obstacle.Radius + distanceFromBoundary;

            Vector3 dir = obstacle.Position - target.Position;
            dir.Normalize();

            return obstacle.Position + dir * distAway;
        }
    }
}