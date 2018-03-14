using UnityEngine;

namespace UnityMovementAI
{
    public class ArriveUnit : MonoBehaviour
    {

        public Vector3 targetPosition;

        private SteeringBasics steeringBasics;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
        }

        void FixedUpdate()
        {
            Vector3 accel = steeringBasics.Arrive(targetPosition);

            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
        }
    }
}