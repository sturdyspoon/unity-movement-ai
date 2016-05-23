using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NearSensor : MonoBehaviour {

	public HashSet<MovementAIRigidbody> targets = new HashSet<MovementAIRigidbody>();

    private void tryToAdd(Component other)
    {
        MovementAIRigidbody rb = other.GetComponent<MovementAIRigidbody>();
        if(rb != null)
        {
            targets.Add(rb);
        }
    }

    private void tryToRemove(Component other)
    {
        MovementAIRigidbody rb = other.GetComponent<MovementAIRigidbody>();
        if (rb != null)
        {
            targets.Remove(rb);
        }
    }

    void OnTriggerEnter(Collider other) {
        tryToAdd(other);
	}
	
	void OnTriggerExit(Collider other) {
        tryToRemove(other);
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        tryToAdd(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        tryToRemove(other);
    }
}
