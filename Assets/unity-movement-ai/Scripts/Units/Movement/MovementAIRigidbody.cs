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

    /* If the character should try to stay grounded */
    public bool stayGrounded = true;

    /* How far the character should look below him for ground to stay grounded to */
    public float fooGroundFollowDistance = 0.1f;

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

    private int countDebug = 0;

    private IEnumerator debugDraw()
    {
        yield return new WaitForFixedUpdate();

        Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (velocity.normalized), Color.red, 0f, false);
        Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (rb3D.velocity.normalized * 1.5f), Color.green, 0f, false);
        Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (wallNormal), Color.yellow, 0f, false);
        Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (movementNormal), Color.yellow, 0f, false);

        //Debug.Log("waitforfixedupdate " + transform.position.ToString("f4"));
        //Debug.Log(rb3D.velocity.magnitude);

        countDebug = 0;
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

            RaycastHit downHit;

            /* 
            Start the ray with a small offset of 0.1f from inside the character. The
            transform.position of the characer is assumed to be at the base of the character.
             */
            if (sphereCast(Vector3.down, out downHit, fooGroundFollowDistance))
            {
                if (isWall(downHit.normal))
                {
                    /* Get vector pointing down the wall */
                    Vector3 rightSlope = Vector3.Cross(downHit.normal, Vector3.down);
                    Vector3 downSlope = Vector3.Cross(rightSlope, downHit.normal);

                    float remainingDist = fooGroundFollowDistance - downHit.distance;

                    RaycastHit downWallHit;

                    /* If we found ground that we would have hit if not for the wall then follow it */
                    if (remainingDist > 0 && sphereCast(downSlope, out downWallHit, remainingDist, downHit.normal) && !isWall(downWallHit.normal))
                    {
                        Vector3 newPos = rb3D.position + (Vector3.down * downHit.distance) + (downSlope.normalized * downWallHit.distance);
                        foundGround(downWallHit.normal, newPos);
                    }

                    /* If we are close enough to the hit to be touching it then we are on the wall */
                    if (downHit.distance <= 0.01f)
                    {
                        wallNormal = downHit.normal;
                        //Debug.DrawRay(hitInfo.point, hitInfo.normal, new Color(1f, 0.38823f, 0.27843f), 1f, false);
                    }
                }
                /* Else we've found walkable ground */
                else
                {
                    Vector3 newPos = rb3D.position + (Vector3.down * downHit.distance);
                    foundGround(downHit.normal, newPos);
                    //SteeringBasics.debugCross(hitInfo.point + Vector3.up * (hitInfo.distance - 0.1f), 0.5f, Color.red, 0, false);
                }
            }

            limitMovementOnSteepSlopes();
        }
    }

    /* Make the spherecast offset slightly bigger than the max allowed collider overlap. This was
     * known as Physics.minPenetrationForPenalty and had a default value of 0.05f, but has since
     * been removed and supposedly replaced by Physics.defaultContactOffset/Collider.contactOffset.
     * My tests show that as of Unity 5.3.0f4 this is not %100 true and Unity still seems to be 
     * allowing overlaps of 0.05f somewhere internally. So I'm setting my spherecast offset to be
     * slightly bigger than 0.05f */
    private float spherecastOffset = 0.051f;

    private bool sphereCast(Vector3 dir, out RaycastHit hitInfo, float dist, Vector3 planeNormal = default(Vector3))
    {
        /* The position of the characer is assumed to be at the base of the character,
         * so make sure the sphere origin is truly in the middle of the character sphere.
         *
         * Also if we are given a planeNormal then raise the origin a tiny amount away
         * from the plane to avoid problems when the given dir is just barely moving  
         * into the plane (this occurs due to floating point inaccuracies when the dir
         * is calculated with cross products) */
        Vector3 origin = rb3D.position + (Vector3.up * boundingRadius) + (planeNormal * 0.001f);

        /* Start the ray with a small offset from inside the character, so it will
         * hit any colliders that the character is already touching. */
        origin += -spherecastOffset * dir;

        float maxDist = (spherecastOffset + dist);

        if(Physics.SphereCast(origin, boundingRadius, dir, out hitInfo, maxDist, groundCheckMask.value))
        {
            /* Remove the small offset from the distance before returning*/
            hitInfo.distance -= spherecastOffset;
            return true;
        } else
        {
            return false;
        }
    }

    private void foundGround(Vector3 normal, Vector3 newPos)
    {
        movementNormal = normal;
        rb3D.useGravity = false;
        rb3D.MovePosition(newPos);
        rb3D.velocity = Vector3.ProjectOnPlane(rb3D.velocity, movementNormal);
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
            Vector3 origin = rb3D.position + (Vector3.up * boundingRadius) + (direction * -spherecastOffset);
            float dist = spherecastOffset + rb3D.velocity.magnitude * Time.deltaTime;

            countDebug++;

            if(i == 0)
            {
                Debug.DrawRay(origin + Vector3.up * 0.05f * countDebug, direction, new Color(0.953f, 0.898f, 0.961f), 0f, false);
            }
            else if(i == 1)
            {
                Debug.DrawRay(origin + Vector3.up * 0.05f * countDebug, direction, new Color(0.612f, 0.153f, 0.69f), 0f, false);
            }
            else
            {
                Debug.DrawRay(origin + Vector3.up * 0.05f * countDebug, direction, new Color(0.29f, 0.078f, 0.549f), 0f, false);
            }

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
                    float moveUpDist = Mathf.Max(0, hitInfo.distance - spherecastOffset);
                    rb3D.MovePosition(rb3D.position + (direction * moveUpDist));

                    limitMovementUpPlane(hitInfo.normal);

                    wallNormals.Add(hitInfo.normal);
                    //Debug.DrawRay(hitInfo.point, hitInfo.normal, new Color(1f, 0.6471f, 0), 1, false);
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

            /* Don't move up the intersecting line if it is greater than our slope limit */
            if(Vector3.Angle(rb3D.velocity, Vector3.up) < 90f - slopeLimit)
            {
                rb3D.velocity = Vector3.zero;
            }
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

                    /* If the char is not on the ground then then we will move along the x/z
                     * plane and keep any y movement we already have */
                    if (rb3D.useGravity)
                    {
                        value.y = rb3D.velocity.y;
                        rb3D.velocity = value;
                    }
                    /* Else only move along the ground plane */
                    else
                    {
                        /* Assume the value is given as a vector on the x/z plane and rotate it to be
                         * on the ground plane */
                        rb3D.velocity = (Quaternion.FromToRotation(Vector3.up, movementNormal) * value);
                    }

                    //Debug.Log("Value Vel " + value.ToString("f4"));

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
