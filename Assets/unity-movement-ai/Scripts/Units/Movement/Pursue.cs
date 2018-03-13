using UnityEngine;

namespace UnityMovementAI
{
    [RequireComponent(typeof(SteeringBasics))]
    public class Pursue : MonoBehaviour
    {
        /* Maximum prediction time the pursue will predict in the future */
        public float maxPrediction = 1f;

        private MovementAIRigidbody rb;
        private SteeringBasics steeringBasics;

        void Awake()
        {
            rb = GetComponent<MovementAIRigidbody>();
            steeringBasics = GetComponent<SteeringBasics>();
        }

        public Vector3 getSteering(MovementAIRigidbody target)
        {
            /* Calculate the distance to the target */
            Vector3 displacement = target.position - transform.position;
            float distance = displacement.magnitude;

            /* Get the character's speed */
            float speed = rb.velocity.magnitude;

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
            Vector3 explicitTarget = target.position + target.velocity * prediction;

            //Debug.DrawLine(transform.position, explicitTarget);

            return steeringBasics.seek(explicitTarget);
        }
    }
}