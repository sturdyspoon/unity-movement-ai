using UnityEngine;

namespace UnityMovementAI
{
    public class EvadeUnit : MonoBehaviour
    {
        public MovementAIRigidbody target;

        private SteeringBasics steeringBasics;
        private Evade evade;

        // Use this for initialization
        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            evade = GetComponent<Evade>();
        }

        void FixedUpdate()
        {
            Vector3 accel = evade.getSteering(target);

            steeringBasics.steer(accel);
            steeringBasics.lookWhereYoureGoing();
        }
    }
}