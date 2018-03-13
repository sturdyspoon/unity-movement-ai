using UnityEngine;

namespace UnityMovementAI
{
    public class InterposeUnit : MonoBehaviour
    {

        public MovementAIRigidbody target1;
        public MovementAIRigidbody target2;

        private SteeringBasics steeringBasics;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
        }

        void FixedUpdate()
        {
            Vector3 accel = steeringBasics.interpose(target1, target2);

            steeringBasics.steer(accel);
            steeringBasics.lookWhereYoureGoing();
        }
    }
}