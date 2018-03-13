using UnityEngine;

namespace UnityMovementAI
{
    public class ColAvoidUnit : MonoBehaviour
    {

        public LinePath path;

        private SteeringBasics steeringBasics;
        private FollowPath followPath;
        private CollisionAvoidance colAvoid;

        private NearSensor colAvoidSensor;

        void Start()
        {
            path.calcDistances();

            steeringBasics = GetComponent<SteeringBasics>();
            followPath = GetComponent<FollowPath>();
            colAvoid = GetComponent<CollisionAvoidance>();

            colAvoidSensor = transform.Find("ColAvoidSensor").GetComponent<NearSensor>();
        }

        void FixedUpdate()
        {
            path.draw();

            if (followPath.isAtEndOfPath(path))
            {
                path.reversePath();
            }

            Vector3 accel = colAvoid.getSteering(colAvoidSensor.targets);

            if (accel.magnitude < 0.005f)
            {
                accel = followPath.getSteering(path);
            }

            steeringBasics.steer(accel);
            steeringBasics.lookWhereYoureGoing();
        }
    }
}