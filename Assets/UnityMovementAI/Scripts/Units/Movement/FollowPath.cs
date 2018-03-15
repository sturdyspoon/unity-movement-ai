using UnityEngine;

namespace UnityMovementAI
{
    [RequireComponent(typeof(SteeringBasics))]
    public class FollowPath : MonoBehaviour
    {
        public float stopRadius = 0.005f;

        public float pathOffset = 0.71f;

        public float pathDirection = 1f;

        private SteeringBasics steeringBasics;
        private MovementAIRigidbody rb;

        void Awake()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            rb = GetComponent<MovementAIRigidbody>();
        }

        public Vector3 GetSteering(LinePath path)
        {
            return GetSteering(path, false);
        }

        public Vector3 GetSteering(LinePath path, bool pathLoop)
        {
            Vector3 targetPosition;
            return GetSteering(path, pathLoop, out targetPosition);
        }

        public Vector3 GetSteering(LinePath path, bool pathLoop, out Vector3 targetPosition)
        {

            /* If the path has only one node then just go to that position. */
            if (path.size == 1)
            {
                targetPosition = path[0];
            }
            /* Else find the closest spot on the path to the character and go to that instead. */
            else
            {
                /* Get the param for the closest position point on the path given the character's position */
                float param = path.GetParam(transform.position, rb);

                //Debug.DrawLine(transform.position, path.getPosition(param, pathLoop), Color.red, 0, false);

                if (!pathLoop)
                {
                    Vector3 finalDestination;

                    /* If we are close enough to the final destination then stop moving */
                    if (IsAtEndOfPath(path, param, out finalDestination))
                    {
                        targetPosition = finalDestination;

                        rb.velocity = Vector3.zero;
                        return Vector3.zero;
                    }
                }

                /* Move down the path */
                param += pathDirection * pathOffset;

                /* Set the target position */
                targetPosition = path.GetPosition(param, pathLoop);

                //Debug.DrawLine(transform.position, targetPosition, Color.red, 0, false);
            }

            return steeringBasics.Arrive(targetPosition);
        }

        /// <summary> 
        /// Will return true if the character is at the end of the given path 
        /// </summary>
        public bool IsAtEndOfPath(LinePath path)
        {
            /* If the path has only one node then just check the distance to that node. */
            if (path.size == 1)
            {
                Vector3 endPos = rb.ConvertVector(path[0]);
                return Vector3.Distance(rb.position, endPos) < stopRadius;
            }
            /* Else see if the character is at the end of the path. */
            else
            {
                Vector3 finalDestination;

                /* Get the param for the closest position point on the path given the character's position */
                float param = path.GetParam(transform.position, rb);

                return IsAtEndOfPath(path, param, out finalDestination);
            }
        }

        private bool IsAtEndOfPath(LinePath path, float param, out Vector3 finalDestination)
        {
            bool result;

            /* Find the final destination of the character on this path */
            finalDestination = (pathDirection > 0) ? path[path.size - 1] : path[0];
            finalDestination = rb.ConvertVector(finalDestination);

            /* If the param is closest to the last segment then check if we are at the final destination */
            if (param >= path.distances[path.size - 2])
            {
                result = Vector3.Distance(rb.position, finalDestination) < stopRadius;
            }
            /* Else we are not at the end of the path */
            else
            {
                result = false;
            }

            return result;
        }
    }
}