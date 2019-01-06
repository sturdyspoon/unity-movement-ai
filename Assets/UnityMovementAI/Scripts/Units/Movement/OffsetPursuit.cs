using UnityEngine;

namespace UnityMovementAI
{
    [RequireComponent(typeof(SteeringBasics))]
    public class OffsetPursuit : MonoBehaviour
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

        public Vector3 GetSteering(MovementAIRigidbody target, Vector3 offset)
        {
            Vector3 targetPos;
            return GetSteering(target, offset, out targetPos);
        }

        public Vector3 GetSteering(MovementAIRigidbody target, Vector3 offset, out Vector3 targetPos)
        {
            Vector3 worldOffsetPos = target.Position + target.Transform.TransformDirection(offset);

            //Debug.DrawLine(transform.position, worldOffsetPos);

            /* Calculate the distance to the offset point */
            Vector3 displacement = worldOffsetPos - transform.position;
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
            targetPos = worldOffsetPos + target.Velocity * prediction;

            return steeringBasics.Arrive(targetPos);
        }
    }
}