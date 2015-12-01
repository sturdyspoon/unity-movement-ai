using UnityEngine;
using System.Collections;

public class ArriveUnit : MonoBehaviour {

    public Vector3 targetPosition;

    private SteeringBasics steeringBasics;

    // Use this for initialization
    void Start()
    {
        steeringBasics = GetComponent<SteeringBasics>();
    }

    void FixedUpdate()
    {
        Vector3 accel = steeringBasics.arrive(targetPosition);

        steeringBasics.steer(accel);
        steeringBasics.lookWhereYoureGoing();
    }
}
