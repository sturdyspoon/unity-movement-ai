using UnityEngine;

namespace UnityMovementAI
{
    public class ColAvoidUnit : MonoBehaviour
    {

        public LinePath path;

        SteeringBasics steeringBasics;
        FollowPath followPath;
        CollisionAvoidance colAvoid;

        NearSensor colAvoidSensor;

        void Start()
        {
            path.CalcDistances();

            steeringBasics = GetComponent<SteeringBasics>();
            followPath = GetComponent<FollowPath>();
            colAvoid = GetComponent<CollisionAvoidance>();

            colAvoidSensor = transform.Find("ColAvoidSensor").GetComponent<NearSensor>();
        }

        void FixedUpdate()
        {
            path.Draw();

            if (followPath.IsAtEndOfPath(path))
            {
                path.ReversePath();
            }

            Vector3 accel = colAvoid.GetSteering(colAvoidSensor.targets);

            if (accel.magnitude < 0.005f)
            {
                accel = followPath.GetSteering(path);
            }

            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
        }
    }
}