using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// This is a wrapper class for either a Rigidbody or Rigidbody2D, so that either can be used with the Unity Movement AI code. 
/// </summary>
public class MovementAIRigidbody : MonoBehaviour {
    [Header("3D Settings")]

    /* Determines if the character should follow the ground or can fly any where in 3D space */
    public bool canFly = false;

    [Header("3D Grounded Settings")]

    /* Controls how close the ground can be before the character is considered to be on it */
    public float groundCheckDistance = 0.01f;

    /* If the character should try to stay grounded */
    public bool stayGrounded = true;

    /* How far the character should look below him for ground to stay grounded to */
    public float groundFollowDistance = 0.3f;

    public float groundFollowGravityMult = 4f;

    /* The sphere cast mask that determines what layers should be consider the ground */
    public LayerMask groundCheckMask = Physics.DefaultRaycastLayers;

    /* The maximum slope the character can climb in degrees */
    public float slopeLimit = 80f;


    /// <summary>
    /// This holds the bounding radius for the current game object (either the radius of a sphere
    /// or circle collider). If the game object does not have a sphere or circle collider this 
    /// will be set to -1.
    /// </summary>
    [System.NonSerialized]
    public float boundingRadius = -1f;

    [System.NonSerialized]
    public bool is3D;

    /// <summary>
    /// Holds the current ground normal for this character. This value is only used by 3D 
    /// characters who cannot fly.
    /// </summary>
    [System.NonSerialized]
    public Vector3 wallNormal = Vector3.zero;

    /// <summary>
    /// Holds the current movement plane normal for this character. This value is only
    /// used by 3D characters who cannot fly.
    /// </summary>
    [System.NonSerialized]
    public Vector3 movementNormal = Vector3.up;

    private Rigidbody rb3D;
    private Rigidbody2D rb2D;

    void Awake()
    {
        setUp();
    }

    public void setUp()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            this.rb3D = rb;
            is3D = true;
        }
        else
        {
            this.rb2D = GetComponent<Rigidbody2D>();
            is3D = false;
        }

        setBoundingRadius();
    }

    void Start()
    {
        StartCoroutine(debugDraw());

        setBoundingRadius();

        /* Call fixed update for 3D grounded characters to make sure they get proper 
         * ground / movement normals before their velocity is set */
        FixedUpdate();
    }

    private IEnumerator debugDraw()
    {
        yield return new WaitForFixedUpdate();

        Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (velocity.normalized), Color.red, 0f, false);
        Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (rb3D.velocity.normalized * 1.5f), Color.green, 0f, false);
        Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (wallNormal), Color.yellow, 0f, false);
        Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (movementNormal), Color.yellow, 0f, false);

        //Debug.Log("waitforfixedupdate " + transform.position.ToString("f4"));
        StartCoroutine(debugDraw());
    }

    private void setBoundingRadius()
    {
        if (is3D)
        {
            SphereCollider col = rb3D.GetComponent<SphereCollider>();

            if (col != null)
            {
                boundingRadius = Mathf.Max(rb3D.transform.localScale.x, rb3D.transform.localScale.y, rb3D.transform.localScale.z) * col.radius;
            } else
            {
                CapsuleCollider capCol = rb3D.GetComponent<CapsuleCollider>();

                if(capCol != null)
                {
                    boundingRadius = Mathf.Max(rb3D.transform.localScale.x, rb3D.transform.localScale.z) * capCol.radius;
                }
            }
        }
        else
        {
            CircleCollider2D col = rb2D.GetComponent<CircleCollider2D>();

            if (col != null)
            {
                boundingRadius = Mathf.Max(rb2D.transform.localScale.x, rb2D.transform.localScale.y) * col.radius;
            }
        }
    }

    void FixedUpdate()
    {
        //Debug.Log("fixed " + transform.position.ToString("f4"));
        /* If the character can't fly then find the current the ground normal */
        if (is3D && !canFly)
        {
            /* Reset to default values */
            wallNormal = Vector3.zero;
            movementNormal = Vector3.up;
            rb3D.useGravity = true;

            RaycastHit hitInfo;

            /* Make the sphere cast max distance equal to the ground check distance or the ground follow distance if the character is trying to stay grounded */
            float maxOnGroundDist = (0.1f + groundCheckDistance);

            float maxDist = maxOnGroundDist;

            if (stayGrounded && groundCheckDistance < groundFollowDistance)
            {
                maxDist = (0.1f + groundFollowDistance);
            }

            /* 
            Start the ray with a small offset of 0.1f from inside the character. The
            transform.position of the characer is assumed to be at the base of the character.
             */
            if (Physics.SphereCast(transform.position + (Vector3.up * (0.1f + boundingRadius)), boundingRadius, Vector3.down, out hitInfo, maxDist, groundCheckMask.value))
            {
                if (isWall(hitInfo.normal))
                {
                    /* Get vector pointing down the wall */
                    Vector3 rightSlope = Vector3.Cross(hitInfo.normal, Vector3.down);
                    Vector3 downSlope = Vector3.Cross(rightSlope, hitInfo.normal);

                    RaycastHit rayHitInfo;

                    /* If we found ground that we would have hit if not for the wall then follow it */
                    if (Physics.Raycast(hitInfo.point, downSlope, out rayHitInfo) && !isWall(rayHitInfo.normal))
                    {
                        //SteeringBasics.debugCross(rayHitInfo.point, 0.5f, Color.magenta);

                        Vector3 contactPointOnChar = (transform.position + (Vector3.up * boundingRadius)) - (rayHitInfo.normal * boundingRadius);

                        //SteeringBasics.debugCross(contactPointOnChar, 0.5f, Color.magenta);

                        float distToPlane = Vector3.Dot((contactPointOnChar - rayHitInfo.point), rayHitInfo.normal);

                        float downwardDistToPlane = Mathf.Abs(distToPlane / Vector3.Dot(rayHitInfo.normal, Vector3.down));

                        if(downwardDistToPlane + hitInfo.distance < maxDist)
                        {
                            foundGround(rayHitInfo.normal, downwardDistToPlane + hitInfo.distance <= maxOnGroundDist);
                        }
                    }

                    /* If we are close enough to the hit to be touching it then we are on the wall */
                    if (hitInfo.distance <= maxOnGroundDist)
                    {
                        wallNormal = hitInfo.normal;
                    }
                }
                /* Else we've found walkable ground */
                else
                {
                    foundGround(hitInfo.normal, hitInfo.distance <= maxOnGroundDist);
                }
            }

            limitMovementOnSteepSlopes();
        }
    }

    private void foundGround(Vector3 normal, bool isCloseEnough)
    {
        movementNormal = normal;

        /* If we are close enough to the hit to be touching it then turn off the gravity */
        if (isCloseEnough)
        {
            rb3D.useGravity = false;
        }
        /* Else we are close enough to see ground that we want to follow but not as close as we want, so apply an acceleration force downwards */
        else
        {
            rb3D.velocity += Physics.gravity * groundFollowGravityMult * Time.deltaTime;
        }
    }

    private bool isWall(Vector3 surfNormal)
    {
        /* If the normal of the surface is greater then our slope limit then its a wall */
        return Vector3.Angle(Vector3.up, surfNormal) > slopeLimit;
    }

    private void limitMovementOnSteepSlopes()
    {
        HashSet<Vector3> wallNormals = new HashSet<Vector3>();

        /* If we are currently on a wall then limit our movement */
        if (wallNormal != Vector3.zero && isMovingInto(rb3D.velocity, wallNormal))
        {
            limitMovementUpPlane(wallNormal);
            wallNormals.Add(wallNormal);
        }

        /* Check if we are moving into a wall */
        for (int i = 0; i < 3; i++)
        {
            Vector3 direction = rb3D.velocity.normalized;
            Vector3 origin = rb3D.position + (Vector3.up * boundingRadius) + (direction * -0.1f);
            float dist = 0.1f + rb3D.velocity.magnitude * Time.deltaTime;

            RaycastHit hitInfo;

            /* Spherecast in the direction we are moving and check if we will hit a wall. Also check that we are
             * in fact moving into the wall (it seems that it is possible to clip the corner of a wall even 
             * though the char/spherecast is moving away from the wall) */
            if (Physics.SphereCast(origin, boundingRadius, direction, out hitInfo, dist, groundCheckMask.value) 
                && isWall(hitInfo.normal) && isMovingInto(direction, hitInfo.normal))
            {
                if(wallNormals.Contains(hitInfo.normal))
                {
                    Vector3 vel = Vector3.zero;
                    if(rb3D.useGravity)
                    {
                        vel.y = rb3D.velocity.y;
                    }
                    rb3D.velocity = vel;

                    break;
                } else
                {   
                    /* Move up to the on coming wall */
                    float moveUpDist = Mathf.Max(0, hitInfo.distance - 0.1f);
                    rb3D.MovePosition(rb3D.position + (direction * moveUpDist));

                    limitMovementUpPlane(hitInfo.normal);

                    wallNormals.Add(hitInfo.normal);
                }
            } else
            {
                break;
            }
        }
    }

    private bool isMovingInto(Vector3 dir, Vector3 normal)
    {
        return Vector3.Angle(dir, normal) > 90f;
    }

    private void limitMovementUpPlane(Vector3 planeNormal)
    {
        if(!rb3D.useGravity)
        {
            Vector3 groundPlaneIntersection = Vector3.Cross(movementNormal, planeNormal);

            rb3D.velocity = Vector3.Project(rb3D.velocity, groundPlaneIntersection);
        } else
        {
            /* Get vector pointing down the slope) */
            Vector3 rightSlope = Vector3.Cross(planeNormal, Vector3.down);
            Vector3 downSlope = Vector3.Cross(rightSlope, planeNormal);

            /* Keep any downward movement (like gravity) */
            float yComponent = Mathf.Min(0f, rb3D.velocity.y);

            /* Project the remaining movement on to the wall */
            Vector3 newVel = rb3D.velocity;
            newVel.y = 0;
            newVel = Vector3.ProjectOnPlane(newVel, planeNormal);

            /* If the remaining movement is moving up the wall then make it only go left/right.
                * I believe this will be true for all  ramp walls but false for all ceiling walls */
            if (Vector3.Angle(downSlope, newVel) > 90f)
            {
                newVel = Vector3.Project(newVel, rightSlope);
            }

            /* Add the downward movement back in and make sure we are still moving along the wall
                * so future sphere casts won't hit this wall */
            newVel.y = yComponent;
            newVel = Vector3.ProjectOnPlane(newVel, planeNormal);

            rb3D.velocity = newVel;

            //Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (Vector3.up), Color.blue);
            //Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (planeMovement.normalized), Color.magenta);
        }
    }

    public Vector3 position
    {
        get
        {
            if(is3D)
            {
                if (canFly)
                {
                    return rb3D.position;
                } else
                {
                    return new Vector3(rb3D.position.x, 0, rb3D.position.z);
                }
            } else
            {
                return rb2D.position;
            }
        }
    }

    /* The following are only used for non flying 3D characters */
    private Vector3 groundedCharVel = Vector3.zero;
    private Vector3 lastVel = Vector3.zero;
    private Vector3 lastMovementNormal = Vector3.up;

    private Vector3 getGroundedVelocity()
    {
        if (movementNormal != lastMovementNormal || lastVel != rb3D.velocity)
        {
            groundedCharVel = Vector3.ProjectOnPlane(rb3D.velocity, movementNormal);
            groundedCharVel = Quaternion.FromToRotation(movementNormal, Vector3.up) * groundedCharVel;
            lastMovementNormal = movementNormal;
            lastVel = rb3D.velocity;
        }

        return groundedCharVel;
    }

    private int count = 0; 

    public Vector3 velocity
    {
        get
        {
            if (is3D)
            {
                if(canFly)
                {
                    return rb3D.velocity;
                } else
                {
                    return getGroundedVelocity();
                }
            }
            else
            {
                return rb2D.velocity;
            }
        }

        set
        {
            if (is3D)
            {
                if(canFly)
                {
                    rb3D.velocity = value;
                } else
                {
                    //Debug.Log("setvelocity " + transform.position.ToString("f4"));
                    count++;
                    //Debug.Log(count + " " + rb3D.velocity.ToString("f4"));

                    Vector3 nonGroundVel = rb3D.velocity - Vector3.ProjectOnPlane(rb3D.velocity, movementNormal);
                    rb3D.velocity = nonGroundVel + (Quaternion.FromToRotation(Vector3.up, movementNormal) * value);

                    limitMovementOnSteepSlopes();
                }
            }
            else
            {
                rb2D.velocity = value;
            }
        }
    }

    public new Transform transform
    {
        get
        {
            if (is3D)
            {
                return rb3D.transform;
            }
            else
            {
                return rb2D.transform;
            }
        }
    }

    public Quaternion rotation
    {
        get
        {
            if (is3D)
            {
                return rb3D.rotation;
            }
            else
            {
                Quaternion r = Quaternion.identity;
                r.eulerAngles = new Vector3(0, 0, rb2D.rotation);
                return r;
            }
        }
    }

    /// <summary>
    /// Rotates the rigidbody to angle (given in degrees)
    /// </summary>
    /// <param name="angle"></param>
    public void MoveRotation(float angle)
    {
        if (is3D)
        {
            Quaternion rot = Quaternion.Euler((new Vector3(0f, angle, 0f)));
            rb3D.MoveRotation(rot);
        }
        else
        {
            rb2D.MoveRotation(angle);
        }
    }

    public float rotationInRadians
    {
        get
        {
            if (is3D)
            {
                return rb3D.rotation.eulerAngles.y * Mathf.Deg2Rad;
            }
            else
            {
                return rb2D.rotation * Mathf.Deg2Rad;
            }
        }
    }

    public Vector3 rotationAsVector
    {
        get
        {
            return SteeringBasics.orientationToVector(rotationInRadians, is3D);
        }
    }

    /// <summary>
    /// Converts the given vector to a vector that is appropriate for the kind of 
    /// character this rigidbody is on. If the character is a 2D character then
    /// the z component will be zeroed out. If the character is a grounded 3D 
    /// character then the y component will be zeroed out. And if the character is 
    /// flying 3D character no changes will be made to the vector.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public Vector3 convertVector(Vector3 v)
    {
        /* If the character is a 2D character then ignore the z component */
        if (!is3D)
        {
            v.z = 0;
        }
        /* Else if the charater is a 3D character who can't fly then ignore the y component */
        else if(!canFly)
        {
            v.y = 0;
        }

        return v;
    }

    public override bool Equals(System.Object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        MovementAIRigidbody p = obj as MovementAIRigidbody;
        if ((System.Object)p == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (rb3D == p.rb3D) && (rb2D == p.rb2D);
    }

    public bool Equals(MovementAIRigidbody p)
    {
        // If parameter is null return false:
        if ((object)p == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (rb3D == p.rb3D) && (rb2D == p.rb2D);
    }

    public override int GetHashCode()
    {
        if(is3D)
        {
            return rb3D.GetHashCode();
        } else
        {
            return rb2D.GetHashCode();
        }
    }

    public static bool operator ==(MovementAIRigidbody a, MovementAIRigidbody b)
    {
        // If both are null, or both are same instance, return true.
        if (System.Object.ReferenceEquals(a, b))
        {
            return true;
        }

        // If one is null, but not both, return false.
        if (((object)a == null) || ((object)b == null))
        {
            return false;
        }

        // Return true if the fields match:
        return a.rb3D == b.rb3D && a.rb2D == b.rb2D;
    }

    public static bool operator !=(MovementAIRigidbody a, MovementAIRigidbody b)
    {
        return !(a == b);
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
