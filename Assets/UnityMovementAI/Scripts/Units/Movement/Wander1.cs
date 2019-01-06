using UnityEngine;

namespace UnityMovementAI
{
    [RequireComponent(typeof(SteeringBasics))]
    public class Wander1 : MonoBehaviour
    {
        /// <summary>
        /// The forward offset of the wander square
        /// </summary>
        public float wanderOffset = 1.5f;

        /// <summary>
        /// The radius of the wander square
        /// </summary>
        public float wanderRadius = 4;

        /// <summary>
        /// The rate at which the wander orientation can change in radians
        /// </summary>
        public float wanderRate = 0.4f;

        float wanderOrientation = 0;

        SteeringBasics steeringBasics;

        MovementAIRigidbody rb;

        //GameObject debugRing;

        void Awake()
        {
            //		DebugDraw debugDraw = gameObject.GetComponent<DebugDraw> ();
            //		debugRing = debugDraw.createRing (Vector3.zero, wanderRadius);

            steeringBasics = GetComponent<SteeringBasics>();

            rb = GetComponent<MovementAIRigidbody>();
        }

        public Vector3 GetSteering()
        {
            float characterOrientation = rb.RotationInRadians;

            /* Update the wander orientation */
            wanderOrientation += RandomBinomial() * wanderRate;

            /* Calculate the combined target orientation */
            float targetOrientation = wanderOrientation + characterOrientation;

            /* Calculate the center of the wander circle */
            Vector3 targetPosition = transform.position + (SteeringBasics.OrientationToVector(characterOrientation, rb.is3D) * wanderOffset);

            //debugRing.transform.position = targetPosition;

            /* Calculate the target position */
            targetPosition = targetPosition + (SteeringBasics.OrientationToVector(targetOrientation, rb.is3D) * wanderRadius);

            //Debug.DrawLine (transform.position, targetPosition);

            return steeringBasics.Seek(targetPosition);
        }

        /* Returns a random number between -1 and 1. Values around zero are more likely. */
        float RandomBinomial()
        {
            return Random.value - Random.value;
        }
    }
}