using UnityEngine;
using System.Collections;

[RequireComponent (typeof (SteeringBasics))]
public class FollowPath : MonoBehaviour {
	public float stopRadius = 0.005f;
	
	public float pathOffset = 0.71f;

	public float pathDirection = 1f;

	private SteeringBasics steeringBasics;
	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		steeringBasics = GetComponent<SteeringBasics> ();
		rb = GetComponent<Rigidbody> ();
	}

	public Vector3 getSteering (LinePath path) {
		return getSteering (path, false);
	}

	public Vector3 getSteering (LinePath path, bool pathLoop) {
		Vector3 targetPosition;
		return getSteering(path, pathLoop, out targetPosition);
	}

	public Vector3 getSteering (LinePath path, bool pathLoop, out Vector3 targetPosition) {

		// If the path has only one node then just go to that position;
		if (path.Length == 1) {
			targetPosition = path[0];
		}
		// Else find the closest spot on the path to the character and go to that instead.
		else {
            if (!pathLoop)
            {
                /* Find the final destination of the character on this path */
                Vector2 finalDestination = (pathDirection > 0) ? path[path.Length - 1] : path[0];

                /* If we are close enough to the final destination then either stop moving or reverse if 
                 * the character is set to loop on paths */
                if (Vector2.Distance(transform.position, finalDestination) < stopRadius)
                {
                    targetPosition = finalDestination;

                    rb.velocity = Vector2.zero;
                    return Vector2.zero;
                }
            }
			
			/* Get the param for the closest position point on the path given the character's position */
			float param = path.getParam(transform.position);
			
			/* Move down the path */
			param += pathDirection * pathOffset;
			
			/* Set the target position */
			targetPosition = path.getPosition(param, pathLoop);
		}
		
		return steeringBasics.arrive(targetPosition);
	}
}
