using UnityEngine;
using System.Collections;

public class FollowPathUnit : MonoBehaviour {

    public bool pathLoop = false;

    public bool reversePath = false;

    public LinePath path;

    private SteeringBasics steeringBasics;
    private FollowPath followPath;

    private float startTime;
    private int count = 0;
    private Vector3 lastPos;
    private float distTraveled = 0;

    // Use this for initialization
    void Start () {
        path.calcDistances();

        steeringBasics = GetComponent<SteeringBasics>();
        followPath = GetComponent<FollowPath>();

        startTime = Time.time;
        lastPos = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        path.draw();

        distTraveled += Vector3.Distance(lastPos, transform.position);
        lastPos = transform.position;
        if (reversePath && isAtEndOfPath())
        {
            path.reversePath();
            Debug.Log(gameObject.name + " " + (Time.time - startTime) + " " + distTraveled + " " + (distTraveled / (Time.time - startTime)) + " " + count);
            startTime = Time.time;
            count++;
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
