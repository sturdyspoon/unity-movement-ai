using UnityEngine;

namespace UnityMovementAI
{
    public class EvadeUnit : MonoBehaviour
    {
        public MovementAIRigidbody target;

        private SteeringBasics steeringBasics;
        private Evade evade;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            evade = GetComponent<Evade>();
        }

        void FixedUpdate()
        {
            Vector3 accel = evade.GetSteering(target);

            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
        }
    }
}