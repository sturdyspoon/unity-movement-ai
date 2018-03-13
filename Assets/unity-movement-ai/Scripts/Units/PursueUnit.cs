using UnityEngine;

namespace UnityMovementAI
{
    public class PursueUnit : MonoBehaviour
    {

        public MovementAIRigidbody target;

        private SteeringBasics steeringBasics;
        private Pursue pursue;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            pursue = GetComponent<Pursue>();
        }

        void FixedUpdate()
        {
            Vector3 accel = pursue.getSteering(target);

            steeringBasics.steer(accel);
            steeringBasics.lookWhereYoureGoing();
        }
    }
}