using UnityEngine;

/* A helper class for steering a game object in 2D */
using System.Collections.Generic;


public class SteeringBasics : MonoBehaviour {
	
	public float maxVelocity = 3.5f;
	
	/* The maximum acceleration */
	public float maxAcceleration = 10f;

	/* The radius from the target that means we are close enough and have arrived */
	public float targetRadius = 0.005f;
	
	/* The radius from the target where we start to slow down  */
	public float slowRadius = 1f;
	
	/* The time in which we want to achieve the targetSpeed */
	public float timeToTarget = 0.1f;

	public float turnSpeed = 20f;

	private GenericRigidbody rb;

    /* Smoothing controls if the character's look direction should be an average of its previous directions (to smooth out momentary changes in directions) */
	public bool smoothing = true;
	public int numSamplesForSmoothing = 5;
	private Queue<Vector3> velocitySamples = new Queue<Vector3>();
	private Queue<Vector3> groundNormalSamples = new Queue<Vector3>();

    /* Controls how far a ray should try to reach to check for ground (for 3D characters only) */
    public float groundCheckDistance = 0.1f;

	// Use this for initialization
	void Start ()
    {
        rb = getGenericRigidbody(gameObject);
    }

    public static GenericRigidbody getGenericRigidbody(GameObject go)
    {
        GenericRigidbody result;

        Rigidbody rb = go.GetComponent<Rigidbody>();
        if (rb != null)
        {
            result = new GenericRigidbody(rb);
        }
        else
        {
            Rigidbody2D rb2D = go.GetComponent<Rigidbody2D>();
            result = new GenericRigidbody(rb2D);
        }

        return result;
    }

    /* Updates the velocity of the current game object by the given linear acceleration */
    public void steer(Vector3 linearAcceleration) {
        //If the character is moving in 3D then make sure we fully accelerate along the X/Z plane to help keep us moving up and down slopes (I assume this works because the character will move in the X/Z direction and if we move into the ground then Unity's physics will move us in the up direction out of the ground retaining our X/Z movement)
        if(rb.is3D)
        {
            //RaycastHit hitInfo;
            //Vector3 m_GroundNormal;

            //// helper to visualise the ground check ray in the scene view
            //Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * 1f));

            //// 0.1f is a small offset to start the ray from inside the character
            //// it is also good to note that the transform position in the sample assets is at the base of the character
            //if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, 1f))
            ////if(Physics.SphereCast(transform.position + (Vector3.up *(0.1f + rb.boundingRadius)), rb.boundingRadius, Vector3.down, out hitInfo))
            //{
            //    m_GroundNormal = hitInfo.normal;
            //}
            //else
            //{
            //    m_GroundNormal = Vector3.up;
            //}

            //Vector3 temp = linearAcceleration;
            //Vector3 xzTemp = new Vector3(linearAcceleration.x, 0, linearAcceleration.z);

            //linearAcceleration = Vector3.ProjectOnPlane(linearAcceleration, m_GroundNormal).normalized * linearAcceleration.magnitude;
            //Debug.Log(linearAcceleration.ToString("F4") + " " + temp.ToString("F4") + " " + m_GroundNormal.ToString("F4") + " " + Vector3.Angle(temp, linearAcceleration) + " " + Vector3.Angle(temp, -1*linearAcceleration));

            //Vector3 xzLinearAccel = new Vector3(linearAcceleration.x, 0, linearAcceleration.z);
            //if (Vector3.Angle(xzTemp, xzLinearAccel) > Vector3.Angle(xzTemp, -1*xzLinearAccel))
            //{
            //    linearAcceleration = -1 * linearAcceleration;
            //}

            //Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (m_GroundNormal.normalized), Color.cyan);
            //Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (temp.normalized), Color.green);
            //Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (linearAcceleration.normalized), Color.magenta);

            //linearAcceleration = projectAndMagnifyOnGroundPlane(linearAcceleration);
            linearAcceleration = projectAndMagnifyOnXZPlane(linearAcceleration);

            //Vector3 xzDir = new Vector3(linearAcceleration.x, 0, linearAcceleration.z);
            //linearAcceleration = xzDir.normalized * linearAcceleration.magnitude;
        }

        rb.velocity += linearAcceleration * Time.deltaTime;

        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }
    }

    public float fooGroundCheckDistance = 1f;

    public Vector3 projectAndMagnifyOnGroundPlane(Vector3 v)
    {
        Vector3 originalXZ = new Vector3(v.x, 0, v.z);

        Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (v.normalized), Color.green);
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * fooGroundCheckDistance));
        Debug.DrawLine(transform.position + (Vector3.up * rb.boundingRadius), transform.position + (Vector3.up * rb.boundingRadius) + (transform.right * fooGroundCheckDistance));
        //Debug.DrawLine(transform.position + (Vector3.up * rb.boundingRadius), transform.position + (Vector3.up * rb.boundingRadius) + (originalXZ.normalized * fooGroundCheckDistance));

        RaycastHit hitInfo;
        Vector3 m_GroundNormal = Vector3.up;
        bool foobar = false;

        //if (Physics.SphereCast(transform.position + (Vector3.up * (0.1f + rb.boundingRadius)), rb.boundingRadius, Vector3.down, out hitInfo, fooGroundCheckDistance))
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, fooGroundCheckDistance))
        {
            m_GroundNormal = hitInfo.normal;
            foobar = true;
        }

        //if (Physics.SphereCast(transform.position + (Vector3.up * (0.1f + rb.boundingRadius)), rb.boundingRadius, transform.right, out hitInfo, fooGroundCheckDistance))
        if (Physics.Raycast(transform.position + (Vector3.up * rb.boundingRadius), transform.right, out hitInfo, fooGroundCheckDistance))
        //if (Physics.Raycast(transform.position + (Vector3.up * rb.boundingRadius), originalXZ.normalized, out hitInfo, fooGroundCheckDistance))
        {
            if(foobar)
            {
                m_GroundNormal = (m_GroundNormal + hitInfo.normal) / 2;
                m_GroundNormal.Normalize();
            } else
            {
                m_GroundNormal = hitInfo.normal;
            }
        }

        //// 0.1f is a small offset to start the ray from inside the character
        //// it is also good to note that the transform position in the sample prefabs is at the base of the character
        //if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, fooGroundCheckDistance))
        ////if(Physics.SphereCast(transform.position + (Vector3.up *(0.1f + rb.boundingRadius)), rb.boundingRadius, Vector3.down, out hitInfo))
        //{
        //    m_GroundNormal = hitInfo.normal;
        //}
        //else
        //{
        //    m_GroundNormal = Vector3.up;
        //}

        Debug.Log(Vector3.Angle(v, m_GroundNormal));

        //Vector3 originalXZ = new Vector3(v.x, 0, v.z);

        if(Vector3.Angle(v, m_GroundNormal) > 171f)
        {
            v = Vector3.ProjectOnPlane(v, m_GroundNormal).normalized * v.magnitude;
        } else
        {
            v = Vector3.ProjectOnPlane(originalXZ, m_GroundNormal).normalized * v.magnitude;
        }

        Vector3 projectedXZ = new Vector3(v.x, 0, v.z);

        /* 
          Check if we should inverse the projected vector by seeing if the inverse vector
          is closer to the original vector along the X/Z plane.
         */
        if (Vector3.Angle(originalXZ, projectedXZ) > Vector3.Angle(originalXZ, -1 * projectedXZ))
        {
            v = -1 * v;
        }



        //float vSize = v.magnitude;

        //if (groundNormalSamples.Count > 5)
        //{
        //    groundNormalSamples.Dequeue();
        //}

        //groundNormalSamples.Enqueue(v.normalized);

        //v = Vector3.zero;

        //foreach (Vector3 n in groundNormalSamples)
        //{
        //    v += n;
        //}

        //v /= groundNormalSamples.Count;
        //v = v.normalized * vSize;



        Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (m_GroundNormal.normalized), Color.cyan);
        Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (v.normalized), Color.magenta);

        return v;
    }

    public Vector3 projectAndMagnifyOnXZPlane(Vector3 v)
    {
        Vector3 xzDir = new Vector3(v.x, 0, v.z);
        return xzDir.normalized * v.magnitude;
    }

    /* A seek steering behavior. Will return the steering for the current game object to seek a given position */
    public Vector3 seek(Vector3 targetPosition, float maxSeekAccel) {
		//Get the direction
		Vector3 acceleration = targetPosition - transform.position;
		
		acceleration.Normalize ();
		
		//Accelerate to the target
		acceleration *= maxSeekAccel;
		
		return acceleration;
	}

    public Vector3 seek(Vector3 targetPosition)
    {
        return seek(targetPosition, maxAcceleration);
    }

    /* Makes the current game object look where he is going */
    public void lookWhereYoureGoing() {
		Vector3 direction = rb.velocity;

		if (smoothing) {
			if (velocitySamples.Count == numSamplesForSmoothing) {
				velocitySamples.Dequeue ();
			}

			velocitySamples.Enqueue (rb.velocity);

			direction = Vector3.zero;

			foreach (Vector3 v in velocitySamples) {
				direction += v;
			}

			direction /= velocitySamples.Count;
		}

		lookAtDirection (direction);
	}

	public void lookAtDirection(Vector3 direction) {
		direction.Normalize();
		
		// If we have a non-zero direction then look towards that direciton otherwise do nothing
		if (direction.sqrMagnitude > 0.001f) {
            if(rb.is3D)
            {
                // Mulitply by -1 because counter clockwise on the y-axis is in the negative direction
                float toRotation = -1*(Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg);
                float rotation = Mathf.LerpAngle(transform.rotation.eulerAngles.y, toRotation, Time.deltaTime * turnSpeed);

                transform.rotation = Quaternion.Euler(0, rotation, 0);
            } else {
                float toRotation = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                float rotation = Mathf.LerpAngle(transform.rotation.eulerAngles.z, toRotation, Time.deltaTime * turnSpeed);

                transform.rotation = Quaternion.Euler(0, 0, rotation);
            }
		}
	}

    public void lookAtDirection(Quaternion toRotation)
    {
        if(rb.is3D)
        {
            lookAtDirection(toRotation.eulerAngles.y);
        } else
        {
            lookAtDirection(toRotation.eulerAngles.z);
        }
    }

    /// <summary>
    /// Makes the character's rotation lerp closer to the given target rotation (in degrees).
    /// </summary>
    /// <param name="toRotation">the desired rotation to be looking at in degrees</param>
    public void lookAtDirection(float toRotation)
    {
        if (rb.is3D)
        {
            float rotation = Mathf.LerpAngle(transform.rotation.eulerAngles.y, toRotation, Time.deltaTime * turnSpeed);

            transform.rotation = Quaternion.Euler(0, rotation, 0);
        }
        else
        {
            float rotation = Mathf.LerpAngle(transform.rotation.eulerAngles.z, toRotation, Time.deltaTime * turnSpeed);

            transform.rotation = Quaternion.Euler(0, 0, rotation);
        }
    }

    /* Returns the steering for a character so it arrives at the target */
    public Vector3 arrive(Vector3 targetPosition) {
		/* Get the right direction for the linear acceleration */
		Vector3 targetVelocity = targetPosition - transform.position;
		
		/* Get the distance to the target */
		float dist = targetVelocity.magnitude;

        //targetVelocity.y = 0;

        /* If we are within the stopping radius then stop */
        if (dist < targetRadius) {
			rb.velocity = Vector3.zero;
			return Vector3.zero;
		}
		
		/* Calculate the target speed, full speed at slowRadius distance and 0 speed at 0 distance */
		float targetSpeed;
		if(dist > slowRadius) {
			targetSpeed = maxVelocity;
		} else {
			targetSpeed = maxVelocity * (dist / slowRadius);
		}
		
		/* Give targetVelocity the correct speed */
		targetVelocity.Normalize();
		targetVelocity *= targetSpeed;
		
		/* Calculate the linear acceleration we want */
		Vector3 acceleration = targetVelocity - rb.velocity;
		/*
		 Rather than accelerate the character to the correct speed in 1 second, 
		 accelerate so we reach the desired speed in timeToTarget seconds 
		 (if we were to actually accelerate for the full timeToTarget seconds).
		*/
		acceleration *= 1/timeToTarget;
		
		/* Make sure we are accelerating at max acceleration */
		if(acceleration.magnitude > maxAcceleration) {
			acceleration.Normalize();
			acceleration *= maxAcceleration;
		}

		return acceleration;
	}

    public Vector3 interpose(GenericRigidbody target1, GenericRigidbody target2)
    {
        Vector3 midPoint = (target1.position + target2.position) / 2;

        float timeToReachMidPoint = Vector3.Distance(midPoint, transform.position) / maxVelocity;

        Vector3 futureTarget1Pos = target1.position + target1.velocity * timeToReachMidPoint;
        Vector3 futureTarget2Pos = target2.position + target2.velocity * timeToReachMidPoint;

        midPoint = (futureTarget1Pos + futureTarget2Pos) / 2;

        return arrive(midPoint);
    }

    /* Checks to see if the target is in front of the character */
    public bool isInFront(Vector3 target)
    {
        return isFacing(target, 0);
    }

    public bool isFacing(Vector3 target, float cosineValue) { 
        Vector3 facing = transform.right.normalized;

        Vector3 directionToTarget = (target - transform.position);
        directionToTarget.Normalize();

        return Vector3.Dot(facing, directionToTarget) >= cosineValue;
    }

    /// <summary>
    /// Returns the given orientation (in radians) as a unit vector
    /// </summary>
    /// <param name="orientation">the orientation in radians</param>
    /// <param name="is3DGameObj">is the orientation for a 3D game object or a 2D game object</param>
    /// <returns></returns>
    public static Vector3 orientationToVector(float orientation, bool is3DGameObj)
    {
        if(is3DGameObj)
        {
            // Mulitply the orientation by -1 because counter clockwise on the y-axis is in the negative
            // direction, but Cos And Sin expect clockwise orientation to be the positive direction
            return new Vector3(Mathf.Cos(-orientation), 0, Mathf.Sin(-orientation));
        } else
        {
            return new Vector3(Mathf.Cos(orientation), Mathf.Sin(orientation), 0);
        }
    }

    /// <summary>
    /// Gets the orientation of a vector as radians. For 3D it gives the orienation around the Y axis.
    /// For 2D it gaves the orienation around the Z axis.
    /// </summary>
    /// <param name="direction">the direction vector</param>
    /// <param name="is3DGameObj">is the direction vector for a 3D game object or a 2D game object</param>
    /// <returns>orientation in radians</returns>
    public static float vectorToOrientation(Vector3 direction, bool is3DGameObj)
    {
        if (is3DGameObj)
        {
            // Mulitply by -1 because counter clockwise on the y-axis is in the negative direction
            return -1*Mathf.Atan2(direction.z, direction.x);
        }
        else
        {
            return Mathf.Atan2(direction.y, direction.x);
        }
    }


    /* This function is here to ensure we have a rigidbody (2D or 3D) */

    //Since we use editor calls we omit this function on build time
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void Reset()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Rigidbody2D rb2D = GetComponent<Rigidbody2D>();

        if (rb == null && rb2D == null)
        {
            if (UnityEditor.EditorUtility.DisplayDialog("Choose a Component", "You are missing one of the required componets. Please choose one to add", "Rigidbody", "Rigidbody2D"))
            {
                gameObject.AddComponent<Rigidbody>();
            }
            else
            {
                gameObject.AddComponent<Rigidbody2D>();
            }
        }
    }
}