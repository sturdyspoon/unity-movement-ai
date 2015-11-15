using UnityEngine;
using System.Collections;

public class FollowPathUnit : MonoBehaviour {

    public bool pathLoop = false;

    public bool reversePath = false;

    public LinePath path;

    private SteeringBasics steeringBasics;
    private FollowPath followPath;

    // Use this for initialization
    void Start () {
        path.calcDistances();

        steeringBasics = GetComponent<SteeringBasics>();
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
        return Vector3.Distance(path.endNode, transform.position) < followPath.stopRadius;
    }
}
