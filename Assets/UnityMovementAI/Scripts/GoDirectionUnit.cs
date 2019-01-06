using UnityEngine;

namespace UnityMovementAI
{
    [RequireComponent(typeof(SteeringBasics))]
    public class GoDirectionUnit : MonoBehaviour
    {
        public Vector3 direction;

        MovementAIRigidbody rb;
        SteeringBasics steeringBasics;

        void Start()
        {
            rb = GetComponent<MovementAIRigidbody>();
            steeringBasics = GetComponent<SteeringBasics>();
        }

        void FixedUpdate()
        {
            rb.Velocity = direction.normalized * steeringBasics.maxVelocity;

            steeringBasics.LookWhereYoureGoing();

            Debug.DrawLine(rb.ColliderPosition, rb.ColliderPosition + (direction.normalized), Color.cyan, 0f, false);
        }
    }
}