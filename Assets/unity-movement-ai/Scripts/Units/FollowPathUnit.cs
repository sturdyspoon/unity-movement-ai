using UnityEngine;
using System.Collections;

public class FollowPathUnit : MonoBehaviour {

    public bool pathLoop = false;

    public bool reversePath = false;

    public LinePath path;

    private SteeringBasics steeringBasics;
    private MovementAIRigidbody rb;
    private FollowPath followPath;

    // Use this for initialization
    void Start () {
        path.calcDistances();

        steeringBasics = GetComponent<SteeringBasics>();
        rb = GetComponent<MovementAIRigidbody>();
        followPath = GetComponent<FollowPath>();
    }
	
	// Update is called once per frame
	void Update () {
        path.draw();

        if (reversePath && isAtEndOfPath())
        {
            path.reversePath();
        }

        Vector3 accel = followPath.getSteering(path, pathLoop);

        steeringBasics.steer(accel);
        steeringBasics.lookWhereYoureGoing();
    }

    public bool isAtEndOfPath()
    {
        Vector3 end = rb.convertVector(path.endNode);
        return Vector3.Distance(end, rb.position) < followPath.stopRadius;
    }
}
