using UnityEngine;

namespace UnityMovementAI
{
    public class Wander2Unit : MonoBehaviour
    {
        SteeringBasics steeringBasics;
        Wander2 wander;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            wander = GetComponent<Wander2>();
        }

        void FixedUpdate()
        {
            Vector3 accel = wander.GetSteering();

            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
        }
    }
}