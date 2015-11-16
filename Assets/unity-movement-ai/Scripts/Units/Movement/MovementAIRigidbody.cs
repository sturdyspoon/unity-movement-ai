using UnityEngine;
using System.Collections;

/// <summary>
/// This is a wrapper class for either a Rigidbody or Rigidbody2D, so that either can be used with the Unity Movement AI code. 
/// </summary>
public class MovementAIRigidbody : MonoBehaviour {
    [Header("3D Only Settings")]

    /* Determines if the character should follow the ground or can fly any where in 3D space */
    public bool fooCanFly = false;

    /* Controls how far a ray should try to reach to check for ground (for 3D characters only) */
    public float fooGroundCheckDistance = 1f;


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
    public Vector3 groundNormal = Vector3.up;

    private Rigidbody rb;
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
            this.rb = rb;
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
            SphereCollider col = rb.GetComponent<SphereCollider>();

            if (col != null)
            {
                boundingRadius = Mathf.Max(rb.transform.localScale.x, rb.transform.localScale.y, rb.transform.localScale.z) * col.radius;
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

    // Update is called once per frame
    void Update()
    {
        /* If the character can't fly then find the current the ground normal */
        if(is3D && !fooCanFly)
        {
            groundNormal = Vector3.up;

            RaycastHit hitInfo;

            /* 
            Start the ray with a small offset of 0.1f from inside the character. The
            transform.position of the characer is assumed to be at the base of the character.
             */
            if (Physics.SphereCast(transform.position + (Vector3.up * (0.1f + boundingRadius)), boundingRadius, Vector3.down, out hitInfo, fooGroundCheckDistance))
            //if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, fooGroundCheckDistance))
            {
                groundNormal = hitInfo.normal;
            }
            
            Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * fooGroundCheckDistance), Color.white);
            Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (velocity.normalized), Color.red);
            Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (rb.velocity.normalized*1.5f), Color.green);

            //Vector3 foo = Vector3.ProjectOnPlane(rb.velocity, groundNormal);
            //Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (foo.normalized * 1.5f), Color.green);

            Debug.DrawLine(transform.position + (Vector3.up * 0.3f), transform.position + (Vector3.up * 0.3f) + (groundNormal), Color.yellow);
        }
    }

    public Vector3 position
    {
        get
        {
            if(is3D)
            {
                if (fooCanFly)
                {
                    return rb.position;
                } else
                {
                    return new Vector3(rb.position.x, 0, rb.position.z);
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

    private Vector3 getGroundedCharVel()
    {
        if (groundNormal != lastGroundNormal || lastVel != rb.velocity)
        {
            groundedCharVel = Vector3.ProjectOnPlane(rb.velocity, groundNormal);
            groundedCharVel = Quaternion.FromToRotation(groundNormal, Vector3.up) * groundedCharVel;
            lastGroundNormal = groundNormal;
            lastVel = rb.velocity;
        }

        return groundedCharVel;
    }

    public Vector3 velocity
    {
        get
        {
            if (is3D)
            {
                if(fooCanFly)
                {
                    return rb.velocity;
                } else
                {
                    return getGroundedCharVel();
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
                if(fooCanFly)
                {
                    rb.velocity = value;
                } else
                {
                    Vector3 nonGroundVel = rb.velocity - Vector3.ProjectOnPlane(rb.velocity, groundNormal);
                    rb.velocity = nonGroundVel + (Quaternion.FromToRotation(Vector3.up, groundNormal) * value);
                }
            }
            else
            {
                rb2D.velocity = value;
            }
        }
    }

    public Vector3 realVelocity
    {
        get
        {
            if (is3D)
            {
                return rb.velocity;
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
                rb.velocity = value;
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
                return rb.transform;
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
                return rb.rotation;
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
                return rb.rotation.eulerAngles.y * Mathf.Deg2Rad;
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

    public Vector3 convertVector(Vector3 v)
    {
        /* If the character is a 2D character then ignore the z component */
        if (!is3D)
        {
            v.z = 0;
        }
        /* Else if the charater is a 3D character who can't fly then ignore the y component */
        else if(!fooCanFly)
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
        return (rb == p.rb) && (rb2D == p.rb2D);
    }

    public bool Equals(MovementAIRigidbody p)
    {
        // If parameter is null return false:
        if ((object)p == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (rb == p.rb) && (rb2D == p.rb2D);
    }

    public override int GetHashCode()
    {
        if(is3D)
        {
            return rb.GetHashCode();
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
        return a.rb == b.rb && a.rb2D == b.rb2D;
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
