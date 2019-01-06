using UnityEngine;

namespace UnityMovementAI
{
    [RequireComponent(typeof(SteeringBasics))]
    public class Pursue : MonoBehaviour
    {
        /// <summary>
        /// Maximum prediction time the pursue will predict in the future
        /// </summary>
        public float maxPrediction = 1f;

        MovementAIRigidbody rb;
        SteeringBasics steeringBasics;

        void Awake()
        {
            rb = GetComponent<MovementAIRigidbody>();
            steeringBasics = GetComponent<SteeringBasics>();
        }

        public Vector3 GetSteering(MovementAIRigidbody target)
        {
            /* Calculate the distance to the target */
            Vector3 displacement = target.Position - transform.position;
            float distance = displacement.magnitude;

            /* Get the character's speed */
            float speed = rb.Velocity.magnitude;

            /* Calculate the prediction time */
            float prediction;
            if (speed <= distance / maxPrediction)
            {
                prediction = maxPrediction;
            }
            else
            {
                prediction = distance / speed;
            }

            /* Put the target together based on where we think the target will be */
            Vector3 explicitTarget = target.Position + target.Velocity * prediction;

            //Debug.DrawLine(transform.position, explicitTarget);

            return steeringBasics.Seek(explicitTarget);
        }
    }
}