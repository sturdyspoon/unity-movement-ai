using UnityEngine;
using System.Collections;

public class InterposeUnit : MonoBehaviour {

    public GameObject target1;
    public GameObject target2;

    private MovementAIRigidbody targetRigidbody1;
    private MovementAIRigidbody targetRigidbody2;

    private SteeringBasics steeringBasics;

    // Use this for initialization
    void Start()
    {
        targetRigidbody1 = SteeringBasics.getGenericRigidbody(target1);
        targetRigidbody2 = SteeringBasics.getGenericRigidbody(target2);

        steeringBasics = GetComponent<SteeringBasics>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 accel = steeringBasics.interpose(targetRigidbody1, targetRigidbody2);

        steeringBasics.steer(accel);
        steeringBasics.lookWhereYoureGoing();
    }
}
