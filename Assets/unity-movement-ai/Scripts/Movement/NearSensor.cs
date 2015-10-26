using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NearSensor : MonoBehaviour {

	public HashSet<GenericRigidbody> targets = new HashSet<GenericRigidbody>();

	void OnTriggerEnter(Collider other) {
		targets.Add (SteeringBasics.getGenericRigidbody(other.gameObject));
	}
	
	void OnTriggerExit(Collider other) {
		targets.Remove (SteeringBasics.getGenericRigidbody(other.gameObject));
	}
}
