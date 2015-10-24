using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteeringBasics))]
public class Pursue : MonoBehaviour
{
    /* Maximum prediction time the pursue will predict in the future */
    public float maxPrediction = 1f;

    private Rigidbody rb;
    private SteeringBasics steeringBasics;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        steeringBasics = GetComponent<SteeringBasics>();
	}
	
	public Vector3 getSteering (Rigidbody target) {
        /* Calculate the distance to the target */
        Vector3 displacement = target.position - transform.position;
        float distance = displacement.magnitude;

        /* Get the character's speed */
        float speed = rb.velocity.magnitude;

        /* Calculate the prediction time */
        float prediction;
        if (speed <= distance / maxPrediction)
        {
            prediction = maxPrediction;
        }
        else
        {
            prediction = distance / speed;
        }

        /* Put the target together based on where we think the target will be */
        Vector3 explicitTarget = target.position + target.velocity*prediction;

        return steeringBasics.seek(explicitTarget);
    }
}
