using UnityEngine;
using System.Collections;

public class FollowPathUnit : MonoBehaviour {

    public bool pathLoop = false;

    public bool reversePath = false;

    public LinePath path;

    private SteeringBasics steeringBasics;
    private MovementAIRigidbody rb;
    private FollowPath followPath;

    private float startTime;
    private float distTraveled;
    private Vector3 lastPos;

    // Use this for initialization
    void Start () {
        path.calcDistances();

        steeringBasics = GetComponent<SteeringBasics>();
        rb = GetComponent<MovementAIRigidbody>();
        followPath = GetComponent<FollowPath>();

        startTime = Time.time;
        distTraveled = 0;
        lastPos = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        path.draw();

        distTraveled += Vector3.Distance(transform.position, lastPos);
        lastPos = transform.position;

        if (reversePath && isAtEndOfPath())
        {
            path.reversePath();

            Debug.Log("IT TOOK " + (Time.time - startTime) + " " + distTraveled + " " + (distTraveled/ (Time.time - startTime)));
            startTime = Time.time;
            distTraveled = 0;
        }

        Vector3 accel = followPath.getSteering(path, pathLoop);

        steeringBasics.steer(accel);
        steeringBasics.lookWhereYoureGoing();
    }

    public bool isAtEndOfPath()
    {
        Vector3 end = rb.convertVector(path.endNode);
        //Debug.Log(Vector3.Distance(end, rb.position));
        return Vector3.Distance(end, rb.position) < followPath.stopRadius;
    }
}
