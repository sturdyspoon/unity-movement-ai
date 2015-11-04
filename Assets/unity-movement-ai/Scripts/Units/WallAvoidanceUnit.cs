using UnityEngine;
using System.Collections;

public class WallAvoidanceUnit: MonoBehaviour
{

    public LinePath path;

    private SteeringBasics steeringBasics;
    private WallAvoidance wallAvoidance;
    private FollowPath followPath;

    // Use this for initialization
    void Start()
    {
        path.calcDistances();

        steeringBasics = GetComponent<SteeringBasics>();
        wallAvoidance = GetComponent<WallAvoidance>();
        followPath = GetComponent<FollowPath>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAtEndOfPath())
        {
            path.reversePath();
        }

        Vector3 accel = wallAvoidance.getSteering();

        if (accel.magnitude < 0.005f)
        {
            accel = followPath.getSteering(path);
        }

        steeringBasics.steer(accel);
        steeringBasics.lookWhereYoureGoing();

        path.draw();
    }

    public bool isAtEndOfPath()
    {
        return Vector3.Distance(path.endNode, transform.position) < followPath.stopRadius;
    }
}
