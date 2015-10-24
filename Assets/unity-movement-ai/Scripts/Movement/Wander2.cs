using UnityEngine;
using System.Collections;

public class Wander2 : MonoBehaviour {

    public float wanderRadius = 1.2f;
    
    public float wanderDistance = 2f;

    //maximum amount of random displacement a second
    public float wanderJitter = 40f;

    private Vector3 wanderTarget;

    private SteeringBasics steeringBasics;

    void Start()
    {
        //stuff for the wander behavior
        float theta = Random.value * 2 * Mathf.PI;

        //create a vector to a target position on the wander circle
        wanderTarget = new Vector3(wanderRadius * Mathf.Cos(theta), wanderRadius * Mathf.Sin(theta), 0f);

        steeringBasics = GetComponent<SteeringBasics>();
    }

    public Vector3 getSteering()
    {
        //get the jitter for this time frame
        float jitter = wanderJitter * Time.deltaTime;

        //add a small random vector to the target's position
        wanderTarget += new Vector3(Random.Range(-1f, 1f) * jitter, Random.Range(-1f, 1f) * jitter, 0f);

        //make the wanderTarget fit on the wander circle again
        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        //move the target in front of the character
        Vector3 targetPosition = transform.position + transform.right * wanderDistance + wanderTarget;

        //Debug.DrawLine(transform.position, targetPosition);

        return steeringBasics.seek(targetPosition);
    }


}
