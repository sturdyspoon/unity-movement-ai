using UnityEngine;
using System.Collections;

public class OffsetPursuitUnit : MonoBehaviour {

    public GameObject target;
    private MovementAIRigidbody targetRigidBody;

    public Vector3 offset;
    public float groupLookDist = 1.5f;

    private SteeringBasics steeringBasics;
    private OffsetPursuit offsetPursuit;
    private Separation separation;

    private NearSensor sensor;

    // Use this for initialization
    void Start()
    {
        targetRigidBody = SteeringBasics.getGenericRigidbody(target);

        steeringBasics = GetComponent<SteeringBasics>();
        offsetPursuit = GetComponent<OffsetPursuit>();
        separation = GetComponent<Separation>();

        sensor = transform.Find("SeparationSensor").GetComponent<NearSensor>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 targetPos;
        Vector3 offsetAccel = offsetPursuit.getSteering(targetRigidBody, offset, out targetPos);
        Vector3 sepAccel = separation.getSteering(sensor.targets);

        steeringBasics.steer(offsetAccel + sepAccel);

        /* If we are still arriving then look where we are going, else look the same direction as our formation target */
        if (Vector3.Distance(transform.position, targetPos) > groupLookDist)
        {
            steeringBasics.lookWhereYoureGoing();
        } else
        {
            steeringBasics.lookAtDirection(targetRigidBody.rotation);
        }
    }
}
