using UnityEngine;
using System.Collections;

public class GenericRigidbody {

    /// <summary>
    /// This holds the bounding radius for the current game object (either the radius of a sphere
    /// or circle collider). If the game object does not have a sphere or circle collider this 
    /// will be set to -1.
    /// </summary>
    public float boundingRadius = -1f;

    private Rigidbody rb;
    private Rigidbody2D rb2D;

    public bool is3D;

    public GenericRigidbody(Rigidbody rb)
    {
        this.rb = rb;
        is3D = true;
        setBoundingRadius();
    }

    public GenericRigidbody(Rigidbody2D rb2D)
    {
        this.rb2D = rb2D;
        is3D = false;
        setBoundingRadius();
    }

    public Vector3 position
    {
        get
        {
            if(is3D)
            {
                return rb.position;
            } else
            {
                return rb2D.position;
            }
        }
        set
        {
            if(is3D)
            {
                rb.position = value;
            } else
            {
                rb2D.position = value;
            }
        }
    }

    public Vector3 velocity
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

    public Transform transform
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

    private void setBoundingRadius()
    {
        if (is3D)
        {
            SphereCollider col = rb.GetComponent<SphereCollider>();

            if(col != null)
            {
                boundingRadius = Mathf.Max(rb.transform.localScale.x, rb.transform.localScale.y, rb.transform.localScale.z) * col.radius; ;
            } 
        }
        else
        {
            CircleCollider2D col = rb2D.GetComponent<CircleCollider2D>();

            if(col != null)
            {
                boundingRadius = Mathf.Max(rb2D.transform.localScale.x, rb2D.transform.localScale.y) * col.radius;
            }
        }
    }

    public override bool Equals(System.Object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        GenericRigidbody p = obj as GenericRigidbody;
        if ((System.Object)p == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (rb == p.rb) && (rb2D == p.rb2D);
    }

    public bool Equals(GenericRigidbody p)
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

    public static bool operator ==(GenericRigidbody a, GenericRigidbody b)
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

    public static bool operator !=(GenericRigidbody a, GenericRigidbody b)
    {
        return !(a == b);
    }
}
