using UnityEngine;
using System.Collections;

public class PursueUnit : MonoBehaviour {

    public GameObject target;
    private GenericRigidbody targetRigidbody;

    private SteeringBasics steeringBasics;
    private Pursue pursue;

    // Use this for initialization
    void Start () {
        targetRigidbody = SteeringBasics.getGenericRigidbody(target);

        steeringBasics = GetComponent<SteeringBasics>();
        pursue = GetComponent<Pursue>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 accel = pursue.getSteering(targetRigidbody);

        steeringBasics.steer(accel);
        steeringBasics.lookWhereYoureGoing();
	}
}
