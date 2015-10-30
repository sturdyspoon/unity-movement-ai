using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteeringBasics))]
public class WallAvoidance : MonoBehaviour {

    /* How far ahead the ray should extend */
    public float mainWhiskerLen = 1.25f;

    /* The distance away from the collision that we wish go */
    public float wallAvoidDistance = 0.5f;

    public float sideWhiskerLen = 0.701f;

    public float sideWhiskerAngle = 45f;

    public float maxAcceleration = 40f;

    private GenericRigidbody rb;
    private SteeringBasics steeringBasics;

    // Use this for initialization
    void Start () {
        rb = SteeringBasics.getGenericRigidbody(gameObject);
        steeringBasics = GetComponent<SteeringBasics>();
    }

    public Vector3 getSteering()
    {
        if (rb.velocity.magnitude > 0.005f)
        {
            return getSteering(rb.velocity);
        } else
        {
            return getSteering(rb.rotationAsVector);
        }
    }

    public Vector3 getSteering(Vector3 facingDir)
    {
        Vector3 acceleration = Vector3.zero;

        facingDir.Normalize();

        /* Creates the ray direction vector */
        Vector3[] rayDirs = new Vector3[3];
        rayDirs[0] = facingDir;

        float orientation = SteeringBasics.vectorToOrientation(facingDir, rb.is3D);

        rayDirs[1] = SteeringBasics.orientationToVector(orientation + sideWhiskerAngle * Mathf.Deg2Rad, rb.is3D);
        rayDirs[2] = SteeringBasics.orientationToVector(orientation - sideWhiskerAngle * Mathf.Deg2Rad, rb.is3D);

        GenericRayHit hit;

        /* If no collision do nothing */
        if (!findObstacle(rayDirs, out hit))
        {
            return acceleration;
        }

        /* Create a target away from the wall to seek */
        Vector3 targetPostition = hit.point + hit.normal * wallAvoidDistance;

        /* If velocity and the collision normal are parallel then move the target a bit to
         the left or right of the normal */
        Vector3 cross = Vector3.Cross(rb.velocity, hit.normal);
        if (cross.magnitude < 0.005f)
        {
            Vector3 perp;

            if(rb.is3D)
            {
                perp = new Vector3(-hit.normal.z, hit.normal.y, hit.normal.x);
            } else
            {
                perp = new Vector3(-hit.normal.y, hit.normal.x, hit.normal.z);
            }

            targetPostition = targetPostition + perp;
        }

        return steeringBasics.seek(targetPostition, maxAcceleration);
    }

    private bool findObstacle(Vector3[] rayDirs, out GenericRayHit firstHit)
    {
        firstHit = new GenericRayHit();
        bool foundObs = false;

        for (int i = 0; i < rayDirs.Length; i++)
        {
            float rayDist = (i == 0) ? mainWhiskerLen : sideWhiskerLen;

            GenericRayHit hit;

            if (genericRaycast(transform.position, rayDirs[i], out hit, rayDist))
            {
                foundObs = true;
                firstHit = hit;
                break;
            }

            //Debug.DrawLine(transform.position, transform.position + rayDirs[i] * rayDist);
        }

        return foundObs;
    }

    private bool genericRaycast(Vector3 origin, Vector3 direction, out GenericRayHit hit, float distance = Mathf.Infinity)
    {
        bool result = false;

        if (rb.is3D)
        {
            RaycastHit h;
            result = Physics.Raycast(origin, direction, out h, distance);
            hit = new GenericRayHit(h);
        }
        else
        {
            RaycastHit2D h = Physics2D.Raycast(origin, direction, distance);
            result = (h.collider != null); //RaycastHit2D auto evaluates to true or false evidently
            hit = new GenericRayHit(h);
        }

        return result;
    }

    private struct GenericRayHit
    {
        public Vector3 point;
        public Vector3 normal;

        public GenericRayHit(RaycastHit h)
        {
            this.point = h.point;
            this.normal = h.normal;
        }

        public GenericRayHit(RaycastHit2D h)
        {
            this.point = h.point;
            this.normal = h.normal;
        }
    }
}

