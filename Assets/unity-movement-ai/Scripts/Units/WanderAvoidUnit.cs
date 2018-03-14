using UnityEngine;

namespace UnityMovementAI
{
    public class WanderAvoidUnit : MonoBehaviour
    {
        private SteeringBasics steeringBasics;
        private Wander2 wander;
        private CollisionAvoidance colAvoid;

        private NearSensor colAvoidSensor;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            wander = GetComponent<Wander2>();
            colAvoid = GetComponent<CollisionAvoidance>();

            colAvoidSensor = transform.Find("ColAvoidSensor").GetComponent<NearSensor>();
        }

        void FixedUpdate()
        {
            Vector3 accel = colAvoid.GetSteering(colAvoidSensor.targets);

            if (accel.magnitude < 0.005f)
            {
                accel = wander.GetSteering();
            }

            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
        }
    }
}