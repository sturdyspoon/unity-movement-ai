using UnityEngine;

namespace UnityMovementAI
{
    public class Wander2Unit : MonoBehaviour
    {

        private SteeringBasics steeringBasics;
        private Wander2 wander;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            wander = GetComponent<Wander2>();
        }

        void FixedUpdate()
        {
            Vector3 accel = wander.getSteering();

            steeringBasics.steer(accel);
            steeringBasics.lookWhereYoureGoing();
        }
    }
}