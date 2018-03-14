using UnityEngine;

namespace UnityMovementAI
{
    public class SeekUnit : MonoBehaviour
    {
        public Transform target;

        private SteeringBasics steeringBasics;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
        }

        void FixedUpdate()
        {
            Vector3 accel = steeringBasics.Seek(target.position);

            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
        }
    }
}