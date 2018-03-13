using UnityEngine;

namespace UnityMovementAI
{
    public class FleeUnit : MonoBehaviour
    {
        public Transform target;

        private SteeringBasics steeringBasics;
        private Flee flee;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            flee = GetComponent<Flee>();
        }

        void FixedUpdate()
        {
            Vector3 accel = flee.getSteering(target.position);

            steeringBasics.steer(accel);
            steeringBasics.lookWhereYoureGoing();
        }
    }
}