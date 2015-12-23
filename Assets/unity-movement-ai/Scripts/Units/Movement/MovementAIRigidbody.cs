using UnityEngine;
using System.Collections;

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
    public Vector3 groundNormal = Vector3.zero;

    private bool isOnWall = false;

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
        setBoundingRadius();
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
        /* If the character can't fly then find the current the ground normal */
        if(is3D && !canFly)
        {
            rb3D.useGravity = true;

            RaycastHit hitInfo;

            /* Make the sphere cast max distance equal to the ground check distance or the ground follow distance if the character is trying to stay grounded */
            float maxOnGroundDist = (0.1f + groundCheckDistance);

            float maxDist = maxOnGroundDist;

            if(stayGrounded && groundCheckDistance < groundFollowDistance)
            {
                maxDist = (0.1f + groundFollowDistance);
            }

            /* 
            Start the ray with a small offset of 0.1f from inside the character. The
            transform.position of the characer is assumed to be at the base of the character.
             */
            if (Physics.SphereCast(transform.position + (Vector3.up * (0.1f + boundingRadius)), boundingRadius, Vector3.down, out hitInfo, maxDist, groundCheckMask.value))
            {
                groundNormal = hitInfo.normal;
                isOnWall = isWall(groundNormal);
                movementNormal = (isOnWall) ? Vector3.up : groundNormal;

                /* If we've found walkable ground */
                if(!isOnWall)
                {
                    /* If we are close enough to the hit to be touching it then turn off the gravity */
                    if (hitInfo.distance <= maxOnGroundDist)
                    {
                        rb3D.useGravity = false;
                    }
                    /* Else we are close enough to see ground that we want to follow but not close enough to be on it so apply an acceleration force downwards */
                    else
                    {
                        rb3D.velocity += Physics.gravity * groundFollowGravityMult * Time.deltaTime;
                    }
                }
            } else
            {
                groundNormal = Vector3.zero;
                isOnWall = false;
                movementNormal = Vector3.up;
            }

            limitSlopeMovement();

            Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.down * (maxDist - 0.1f)), Color.white, 0f, false);
            Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (velocity.normalized), Color.red, 0f, false);
            Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (rb3D.velocity.normalized * 1.5f), Color.green, 0f, false);
            Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (groundNormal), Color.yellow, 0f, false);
        }
    }

    private bool isWall(Vector3 surfNormal)
    {
        /* If the normal of the surface is greater then our slope limit then its a wall */
        return Vector3.Angle(Vector3.up, surfNormal) > slopeLimit;
    }

    private void limitSlopeMovement()
    {
        /* Only limit the slope if we are on a wall */
        if(isOnWall)
        {
            float angle = Vector3.Angle(rb3D.velocity, groundNormal);

            if (angle > 90f)
            {
                Vector3 groundMovement = Vector3.ProjectOnPlane(rb3D.velocity, groundNormal);
                //Debug.Log(angle + " " + groundMovement.ToString("F4") + " " + Vector3.up.ToString("F4"));

                /* Get vector pointing down the slope) */
                Vector3 rightSlope = Vector3.Cross(groundNormal, Vector3.down);
                Vector3 downSlope = Vector3.Cross(rightSlope, groundNormal);

                //Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (downSlope), Color.blue);

                if (Vector3.Angle(downSlope, groundMovement) > 90f)
                {
                    //rb3D.velocity -= groundMovement;
                    rb3D.velocity = (rb3D.velocity - groundMovement) + Vector3.Project(groundMovement, rightSlope);
                }

                //Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (Vector3.up), Color.blue);
                //Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (groundMovement.normalized), Color.magenta);
            }
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
    private Vector3 lastGroundNormal = Vector3.up;

    private Vector3 getGroundedVelocity()
    {
        if (movementNormal != lastGroundNormal || lastVel != rb3D.velocity)
        {
            groundedCharVel = Vector3.ProjectOnPlane(rb3D.velocity, movementNormal);
            groundedCharVel = Quaternion.FromToRotation(movementNormal, Vector3.up) * groundedCharVel;
            lastGroundNormal = movementNormal;
            lastVel = rb3D.velocity;
        }

        return groundedCharVel;
    }

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
                    Vector3 nonGroundVel = rb3D.velocity - Vector3.ProjectOnPlane(rb3D.velocity, movementNormal);
                    rb3D.velocity = nonGroundVel + (Quaternion.FromToRotation(Vector3.up, movementNormal) * value);

                    limitSlopeMovement();
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
