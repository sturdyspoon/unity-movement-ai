using UnityEngine;

namespace UnityMovementAI
{
    public class HideUnit : MonoBehaviour
    {
        public MovementAIRigidbody target;

        SteeringBasics steeringBasics;
        Hide hide;
        Spawner obstacleSpawner;

        WallAvoidance wallAvoid;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            hide = GetComponent<Hide>();
            obstacleSpawner = GameObject.Find("ObstacleSpawner").GetComponent<Spawner>();

            wallAvoid = GetComponent<WallAvoidance>();
        }

        void FixedUpdate()
        {
            Vector3 hidePosition;
            Vector3 hideAccel = hide.GetSteering(target, obstacleSpawner.objs, out hidePosition);

            Vector3 accel = wallAvoid.GetSteering(hidePosition - transform.position);

            if (accel.magnitude < 0.005f)
            {
                accel = hideAccel;
            }

            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
        }
    }
}