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

    private Rigidbody rb;
    private SteeringBasics steeringBasics;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        steeringBasics = GetComponent<SteeringBasics>();
    }

    public Vector3 getSteering()
    {
        return getSteering(rb.velocity);
    }

    public Vector3 getSteering(Vector3 facingDir)
    {
        Vector3 acceleration = Vector3.zero;

        /* Creates the ray direction vector */
        Vector3[] rayDirs = new Vector3[3];
        rayDirs[0] = facingDir.normalized;

        float orientation = Mathf.Atan2(rb.velocity.y, rb.velocity.x);

        rayDirs[1] = orientationToVector(orientation + sideWhiskerAngle * Mathf.Deg2Rad);
        rayDirs[2] = orientationToVector(orientation - sideWhiskerAngle * Mathf.Deg2Rad);

        RaycastHit hit;

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
            targetPostition = targetPostition + new Vector3(-hit.normal.y, hit.normal.x, hit.normal.z);
        }

        return steeringBasics.seek(targetPostition, maxAcceleration);
    }

    /* Returns the orientation as a unit vector */
    private Vector3 orientationToVector(float orientation)
    {
        return new Vector3(Mathf.Cos(orientation), Mathf.Sin(orientation), 0);
    }

    private bool findObstacle(Vector3[] rayDirs, out RaycastHit firstHit)
    {
        firstHit = new RaycastHit();
        bool foundObs = false;

        for (int i = 0; i < rayDirs.Length; i++)
        {
            float rayDist = (i == 0) ? mainWhiskerLen : sideWhiskerLen;

            RaycastHit hit;

            if (Physics.Raycast(transform.position, rayDirs[i], out hit, rayDist))
            {
                foundObs = true;
                firstHit = hit;
                break;
            }

            //Debug.DrawLine(transform.position, transform.position + rayDirs[i] * rayDist);
        }

        return foundObs;
    }
}
