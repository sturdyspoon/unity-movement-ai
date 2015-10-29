using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteeringBasics))]
public class Wander1 : MonoBehaviour {
	
	/* The forward offset of the wander square */
	public float wanderOffset = 1.5f;
	
	/* The radius of the wander square */
	public float wanderRadius = 4;
	
	/* The rate at which the wander orientation can change */
	public float wanderRate = 0.4f;
	
	private float wanderOrientation = 0;
	
	private SteeringBasics steeringBasics;

    private bool is3D;

    //private GameObject debugRing;

    void Start() {
		//		DebugDraw debugDraw = gameObject.GetComponent<DebugDraw> ();
		//		debugRing = debugDraw.createRing (Vector3.zero, wanderRadius);
		
		steeringBasics = GetComponent<SteeringBasics> ();

        is3D = SteeringBasics.getGenericRigidbody(gameObject).is3D;
	}

    public Vector3 getSteering() {
		float characterOrientation = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;

        /* Update the wander orientation */
        wanderOrientation += randomBinomial() * wanderRate;

        /* Calculate the combined target orientation */
        float targetOrientation = wanderOrientation + characterOrientation;
		
		/* Calculate the center of the wander circle */
		Vector3 targetPosition = transform.position + (SteeringBasics.orientationToVector (characterOrientation, is3D) * wanderOffset);
		
		//debugRing.transform.position = targetPosition;
		
		/* Calculate the target position */
		targetPosition = targetPosition + (SteeringBasics.orientationToVector(targetOrientation, is3D) * wanderRadius);
		
		//Debug.DrawLine (transform.position, targetPosition);
		
		return steeringBasics.seek (targetPosition);
	}
	
	/* Returns a random number between -1 and 1. Values around zero are more likely. */
	float randomBinomial() {
		return Random.value - Random.value;
	}

}
