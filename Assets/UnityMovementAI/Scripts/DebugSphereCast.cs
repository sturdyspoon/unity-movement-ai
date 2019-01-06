using UnityEngine;

namespace UnityMovementAI
{
    public class DebugSphereCast : MonoBehaviour
    {
        public float radius;
        public Vector3 hitPosition;
        public Vector3 hitSphereCenter;
        public float slopeLimit = 80f;

        void Start()
        {
            SphereCollider col = GetComponent<SphereCollider>();
            radius = Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z) * col.radius;
        }

        void Update()
        {
            hitPosition = Vector3.zero;
            hitSphereCenter = Vector3.zero;

            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, radius, Vector3.down, out hitInfo))
            {
                hitPosition = hitInfo.point;
                hitSphereCenter = transform.position + (Vector3.down * (hitInfo.distance + radius));

                SteeringBasics.DebugCross(hitPosition, 0.5f, Color.yellow);
                SteeringBasics.DebugCross(hitSphereCenter, 0.5f, Color.red);

                if (IsWall(hitInfo.normal))
                {
                    /* Get vector pointing down the slope) */
                    Vector3 rightSlope = Vector3.Cross(hitInfo.normal, Vector3.down);
                    Vector3 downSlope = Vector3.Cross(rightSlope, hitInfo.normal);

                    RaycastHit rayHitInfo;

                    if (Physics.Raycast(hitInfo.point, downSlope, out rayHitInfo))
                    {
                        SteeringBasics.DebugCross(rayHitInfo.point, 0.5f, Color.magenta);
                    }
                }
            }
        }

        bool IsWall(Vector3 surfNormal)
        {
            /* If the normal of the surface is greater then our slope limit then its a wall */
            return Vector3.Angle(Vector3.up, surfNormal) > slopeLimit;
        }
    }
}