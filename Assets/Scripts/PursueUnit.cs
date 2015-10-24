using UnityEngine;
using System.Collections;

public class PursueUnit : MonoBehaviour {

    public Rigidbody target;

    private SteeringBasics steeringBasics;
    private Pursue pursue;

    // Use this for initialization
    void Start () {
        steeringBasics = GetComponent<SteeringBasics>();
        pursue = GetComponent<Pursue>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 accel = pursue.getSteering(target);

        steeringBasics.steer(accel);
        steeringBasics.lookWhereYoureGoing();
	}
}
