using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NearSensor : MonoBehaviour {

	public HashSet<MovementAIRigidbody> targets = new HashSet<MovementAIRigidbody>();

	void OnTriggerEnter(Collider other) {
		targets.Add (other.GetComponent<MovementAIRigidbody>());
	}
	
	void OnTriggerExit(Collider other) {
		targets.Remove (other.GetComponent<MovementAIRigidbody>());
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        targets.Add(other.GetComponent<MovementAIRigidbody>());
    }

    void OnTriggerExit2D(Collider2D other)
    {
        targets.Remove(other.GetComponent<MovementAIRigidbody>());
    }
}
