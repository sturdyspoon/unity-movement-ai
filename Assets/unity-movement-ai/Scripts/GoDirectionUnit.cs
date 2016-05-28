using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SteeringBasics))]
public class GoDirectionUnit : MonoBehaviour {
    public Vector3 direction;

    private MovementAIRigidbody rb;
    private SteeringBasics steeringBasics;

    // Use this for initialization
    void Start () {
        rb = GetComponent<MovementAIRigidbody>();
        steeringBasics = GetComponent<SteeringBasics>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        rb.velocity = direction.normalized * steeringBasics.maxVelocity;

        steeringBasics.lookWhereYoureGoing();

        Debug.DrawLine(rb.colliderPosition, rb.colliderPosition + (direction.normalized), Color.cyan, 0f, false);
	}
}
