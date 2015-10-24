using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Flee))]
public class Evade : MonoBehaviour
{
    /* Maximum prediction time the pursue will predict in the future */
    public float maxPrediction = 1f;

    private Flee flee;

    // Use this for initialization
    void Start()
    {
        flee = GetComponent<Flee>();
    }

    public Vector3 getSteering(Rigidbody target)
    {
        /* Calculate the distance to the target */
        Vector3 displacement = target.position - transform.position;
        float distance = displacement.magnitude;

        /* Get the targets's speed */
        float speed = target.velocity.magnitude;

        /* Calculate the prediction time */
        float prediction;
        if (speed <= distance / maxPrediction)
        {
            prediction = maxPrediction;
        }
        else
        {
            prediction = distance / speed;
            //Place the predicted position a little before the target reaches the character
            prediction *= 0.9f;
        }

        /* Put the target together based on where we think the target will be */
        Vector3 explicitTarget = target.position + target.velocity * prediction;

        return flee.getSteering(explicitTarget);
    }
}
