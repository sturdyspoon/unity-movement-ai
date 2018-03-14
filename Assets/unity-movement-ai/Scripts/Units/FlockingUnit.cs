using UnityEngine;

namespace UnityMovementAI
{
    public class FlockingUnit : MonoBehaviour
    {
        public float cohesionWeight = 1.5f;
        public float separationWeight = 2f;
        public float velocityMatchWeight = 1f;

        private SteeringBasics steeringBasics;
        private Wander2 wander;
        private Cohesion cohesion;
        private Separation separation;
        private VelocityMatch velocityMatch;

        private NearSensor sensor;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            wander = GetComponent<Wander2>();
            cohesion = GetComponent<Cohesion>();
            separation = GetComponent<Separation>();
            velocityMatch = GetComponent<VelocityMatch>();

            sensor = transform.Find("Sensor").GetComponent<NearSensor>();
        }

        void FixedUpdate()
        {
            Vector3 accel = Vector3.zero;

            accel += cohesion.GetSteering(sensor.targets) * cohesionWeight;
            accel += separation.GetSteering(sensor.targets) * separationWeight;
            accel += velocityMatch.GetSteering(sensor.targets) * velocityMatchWeight;

            if (accel.magnitude < 0.005f)
            {
                accel = wander.GetSteering();
            }

            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
        }
    }
}