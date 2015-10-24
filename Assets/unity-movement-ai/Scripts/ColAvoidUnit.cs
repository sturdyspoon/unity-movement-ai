using UnityEngine;
using System.Collections;

public class ColAvoidUnit : MonoBehaviour {

    public LinePath path;

    private SteeringBasics steeringBasics;
    private FollowPath followPath;
    private CollisionAvoidance colAvoid;

    private NearSensor colAvoidSensor;

    // Use this for initialization
    void Start()
    {
        path.calcDistances();

        steeringBasics = GetComponent<SteeringBasics>();
        followPath = GetComponent<FollowPath>();
        colAvoid = GetComponent<CollisionAvoidance>();

        colAvoidSensor = transform.Find("ColAvoidSensor").GetComponent<NearSensor>();
    }

    // Update is called once per frame
    void Update()
    {
        path.draw();

        if (isAtEndOfPath())
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

    public bool isAtEndOfPath()
    {
        return Vector3.Distance(path.endNode, transform.position) < followPath.stopRadius;
    }
}
