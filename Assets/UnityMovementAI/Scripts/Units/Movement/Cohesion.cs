using UnityEngine;
using System.Collections.Generic;

namespace UnityMovementAI
{
    [RequireComponent(typeof(SteeringBasics))]
    public class Cohesion : MonoBehaviour
    {
        public float facingCosine = 120f;

        float facingCosineVal;

        SteeringBasics steeringBasics;

        void Awake()
        {
            facingCosineVal = Mathf.Cos(facingCosine * Mathf.Deg2Rad);
            steeringBasics = GetComponent<SteeringBasics>();
        }

        public Vector3 GetSteering(ICollection<MovementAIRigidbody> targets)
        {
            Vector3 centerOfMass = Vector3.zero;
            int count = 0;

            /* Sums up everyone's position who is close enough and in front of the character */
            foreach (MovementAIRigidbody r in targets)
            {
                if (steeringBasics.IsFacing(r.Position, facingCosineVal))
                {
                    centerOfMass += r.Position;
                    count++;
                }
            }

            if (count == 0)
            {
                return Vector3.zero;
            }
            else
            {
                centerOfMass = centerOfMass / count;

                return steeringBasics.Arrive(centerOfMass);
            }
        }
    }
}