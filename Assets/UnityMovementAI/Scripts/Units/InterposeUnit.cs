using UnityEngine;

namespace UnityMovementAI
{
    public class InterposeUnit : MonoBehaviour
    {
        public MovementAIRigidbody target1;
        public MovementAIRigidbody target2;

        SteeringBasics steeringBasics;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
        }

        void FixedUpdate()
        {
            Vector3 accel = steeringBasics.Interpose(target1, target2);

            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
        }
    }
}