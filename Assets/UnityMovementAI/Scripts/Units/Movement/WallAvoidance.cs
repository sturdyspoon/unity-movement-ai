using UnityEngine;
using System.Collections;

namespace UnityMovementAI
{
    [RequireComponent(typeof(SteeringBasics))]
    public class WallAvoidance : MonoBehaviour
    {
        public float maxAcceleration = 40f;

        public enum WallDetection { Raycast, Spherecast }

        public WallDetection wallDetection = WallDetection.Spherecast;

        public LayerMask castMask = Physics.DefaultRaycastLayers;

        /* The distance away from the collision that we wish go */
        public float wallAvoidDistance = 0.5f;

        /* How far ahead the ray should extend */
        public float mainWhiskerLen = 1.25f;

        public float sideWhiskerLen = 0.701f;

        public float sideWhiskerAngle = 45f;


        private MovementAIRigidbody rb;
        private SteeringBasics steeringBasics;

        void Awake()
        {
            rb = GetComponent<MovementAIRigidbody>();
            steeringBasics = GetComponent<SteeringBasics>();
        }

        public Vector3 GetSteering()
        {
            if (rb.velocity.magnitude > 0.005f)
            {
                return GetSteering(rb.velocity);
            }
            else
            {
                return GetSteering(rb.rotationAsVector);
            }
        }

        public Vector3 GetSteering(Vector3 facingDir)
        {
            Vector3 acceleration = Vector3.zero;

            GenericCastHit hit;

            /* If no collision do nothing */
            if (!FindObstacle(facingDir, out hit))
            {
                return acceleration;
            }

            /* Create a target away from the wall to seek */
            Vector3 targetPostition = hit.point + hit.normal * wallAvoidDistance;

            /* If velocity and the collision normal are parallel then move the target a bit to
             * the left or right of the normal */
            float angle = Vector3.Angle(rb.velocity, hit.normal);
            if (angle > 165f)
            {
                Vector3 perp;

                if (rb.is3D)
                {
                    perp = new Vector3(-hit.normal.z, hit.normal.y, hit.normal.x);
                }
                else
                {
                    perp = new Vector3(-hit.normal.y, hit.normal.x, hit.normal.z);
                }

                /* Add some perp displacement to the target position propotional to the angle between the wall normal
                 * and facing dir and propotional to the wall avoidance distance (with 2f being a magic constant that
                 * feels good) */
                targetPostition = targetPostition + (perp * Mathf.Sin((angle - 165f) * Mathf.Deg2Rad) * 2f * wallAvoidDistance);
            }

            //SteeringBasics.debugCross(targetPostition, 0.5f, new Color(0.612f, 0.153f, 0.69f), 0.5f, false);

            return steeringBasics.Seek(targetPostition, maxAcceleration);
        }

        private bool FindObstacle(Vector3 facingDir, out GenericCastHit firstHit)
        {
            facingDir = rb.ConvertVector(facingDir).normalized;

            /* Create the direction vectors */
            Vector3[] dirs = new Vector3[3];
            dirs[0] = facingDir;

            float orientation = SteeringBasics.VectorToOrientation(facingDir, rb.is3D);

            dirs[1] = SteeringBasics.OrientationToVector(orientation + sideWhiskerAngle * Mathf.Deg2Rad, rb.is3D);
            dirs[2] = SteeringBasics.OrientationToVector(orientation - sideWhiskerAngle * Mathf.Deg2Rad, rb.is3D);

            return CastWhiskers(dirs, out firstHit);
        }

        private bool CastWhiskers(Vector3[] dirs, out GenericCastHit firstHit)
        {
            firstHit = new GenericCastHit();
            bool foundObs = false;

            for (int i = 0; i < dirs.Length; i++)
            {
                float dist = (i == 0) ? mainWhiskerLen : sideWhiskerLen;

                GenericCastHit hit;

                if (GenericCast(dirs[i], out hit, dist))
                {
                    foundObs = true;
                    firstHit = hit;
                    break;
                }
            }

            return foundObs;
        }

        private bool GenericCast(Vector3 direction, out GenericCastHit hit, float distance = Mathf.Infinity)
        {
            bool result = false;
            Vector3 origin = rb.colliderPosition;

            if (rb.is3D)
            {
                RaycastHit h;

                if (wallDetection == WallDetection.Raycast)
                {
                    result = Physics.Raycast(origin, direction, out h, distance, castMask.value);
                }
                else
                {
                    result = Physics.SphereCast(origin, (rb.radius * 0.5f), direction, out h, distance, castMask.value);
                }

                hit = new GenericCastHit(h);

                /* If the character is grounded and we have a result check that we've hit a wall */
                if (!rb.canFly && result)
                {
                    /* If the normal is less than our slope limit then we've hit the ground and not a wall */
                    float angle = Vector3.Angle(Vector3.up, hit.normal);

                    if (angle < rb.slopeLimit)
                    {
                        hit.normal = rb.ConvertVector(hit.normal);
                        result = false;
                    }
                }
            }
            else
            {
                RaycastHit2D h;

                if (wallDetection == WallDetection.Raycast)
                {
                    h = Physics2D.Raycast(origin, direction, distance, castMask.value);
                }
                else
                {
                    h = Physics2D.CircleCast(origin, (rb.radius * 0.5f), direction, distance, castMask.value);
                }

                /* RaycastHit2D auto evaluates to true or false evidently */
                result = (h.collider != null);
                hit = new GenericCastHit(h);
            }

            //Debug.DrawLine(origin, origin + direction * distance, Color.cyan, 0f, false);

            return result;
        }

        private struct GenericCastHit
        {
            public Vector3 point;
            public Vector3 normal;

            public GenericCastHit(RaycastHit h)
            {
                this.point = h.point;
                this.normal = h.normal;
            }

            public GenericCastHit(RaycastHit2D h)
            {
                this.point = h.point;
                this.normal = h.normal;
            }
        }
    }
}