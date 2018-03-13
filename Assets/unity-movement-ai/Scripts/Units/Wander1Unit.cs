using UnityEngine;

namespace UnityMovementAI
{
    public class Wander1Unit : MonoBehaviour
    {

        private SteeringBasics steeringBasics;
        private Wander1 wander;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            wander = GetComponent<Wander1>();
        }

        void FixedUpdate()
        {
            Vector3 accel = wander.getSteering();

            steeringBasics.steer(accel);
            steeringBasics.lookWhereYoureGoing();
        }
    }
}