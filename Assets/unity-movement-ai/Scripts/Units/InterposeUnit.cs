using UnityEngine;
using System.Collections;

public class InterposeUnit : MonoBehaviour {

    public MovementAIRigidbody target1;
    public MovementAIRigidbody target2;

    private SteeringBasics steeringBasics;

    // Use this for initialization
    void Start()
    {
        steeringBasics = GetComponent<SteeringBasics>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 accel = steeringBasics.interpose(target1, target2);

        steeringBasics.steer(accel);
        steeringBasics.lookWhereYoureGoing();
    }
}
