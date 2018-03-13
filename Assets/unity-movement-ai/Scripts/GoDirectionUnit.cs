using UnityEngine;

namespace UnityMovementAI
{
    [RequireComponent(typeof(SteeringBasics))]
    public class GoDirectionUnit : MonoBehaviour
    {
        public Vector3 direction;

        private MovementAIRigidbody rb;
        private SteeringBasics steeringBasics;

        void Start()
        {
            rb = GetComponent<MovementAIRigidbody>();
            steeringBasics = GetComponent<SteeringBasics>();
        }

        void FixedUpdate()
        {
            rb.velocity = direction.normalized * steeringBasics.maxVelocity;

            steeringBasics.lookWhereYoureGoing();

            Debug.DrawLine(rb.colliderPosition, rb.colliderPosition + (direction.normalized), Color.cyan, 0f, false);
        }
    }
}