using UnityEngine;

namespace UnityMovementAI
{
    public class OffsetPursuitUnit : MonoBehaviour
    {
        public MovementAIRigidbody target;

        public Vector3 offset;
        public float groupLookDist = 1.5f;

        SteeringBasics steeringBasics;
        OffsetPursuit offsetPursuit;
        Separation separation;

        NearSensor sensor;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            offsetPursuit = GetComponent<OffsetPursuit>();
            separation = GetComponent<Separation>();

            sensor = transform.Find("SeparationSensor").GetComponent<NearSensor>();
        }

        void LateUpdate()
        {
            Vector3 targetPos;
            Vector3 offsetAccel = offsetPursuit.GetSteering(target, offset, out targetPos);
            Vector3 sepAccel = separation.GetSteering(sensor.targets);

            steeringBasics.Steer(offsetAccel + sepAccel);

            /* If we are still arriving then look where we are going, else look the same direction as our formation target */
            if (Vector3.Distance(transform.position, targetPos) > groupLookDist)
            {
                steeringBasics.LookWhereYoureGoing();
            }
            else
            {
                steeringBasics.LookAtDirection(target.Rotation);
            }
        }
    }
}