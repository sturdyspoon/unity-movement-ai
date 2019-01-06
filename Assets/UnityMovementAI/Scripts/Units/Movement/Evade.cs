using UnityEngine;

namespace UnityMovementAI
{
    [RequireComponent(typeof(Flee))]
    public class Evade : MonoBehaviour
    {
        /// <summary>
        /// Maximum prediction time the pursue will predict in the future
        /// </summary>
        public float maxPrediction = 1f;

        Flee flee;

        void Awake()
        {
            flee = GetComponent<Flee>();
        }

        public Vector3 GetSteering(MovementAIRigidbody target)
        {
            /* Calculate the distance to the target */
            Vector3 displacement = target.Position - transform.position;
            float distance = displacement.magnitude;

            /* Get the targets's speed */
            float speed = target.Velocity.magnitude;

            /* Calculate the prediction time */
            float prediction;
            if (speed <= distance / maxPrediction)
            {
                prediction = maxPrediction;
            }
            else
            {
                prediction = distance / speed;
                //Place the predicted position a little before the target reaches the character
                prediction *= 0.9f;
            }

            /* Put the target together based on where we think the target will be */
            Vector3 explicitTarget = target.Position + target.Velocity * prediction;

            return flee.GetSteering(explicitTarget);
        }
    }
}