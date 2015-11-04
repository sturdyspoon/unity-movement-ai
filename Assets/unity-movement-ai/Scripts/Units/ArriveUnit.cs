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

    // Update is called once per frame
    void Update()
    {
        Vector3 accel = steeringBasics.arrive(targetPosition);

        steeringBasics.steer(accel);
        steeringBasics.lookWhereYoureGoing();
    }
}
