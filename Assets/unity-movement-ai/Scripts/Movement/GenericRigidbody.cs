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

    private bool isRigidbody;

    public GenericRigidbody(Rigidbody rb)
    {
        this.rb = rb;
        isRigidbody = true;
        setBoundingRadius();
    }

    public GenericRigidbody(Rigidbody2D rb2D)
    {
        this.rb2D = rb2D;
        isRigidbody = false;
        setBoundingRadius();
    }

    public Vector3 position
    {
        get
        {
            if(isRigidbody)
            {
                return rb.position;
            } else
            {
                return rb2D.position;
            }
        }
        set
        {
            if(isRigidbody)
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
            if (isRigidbody)
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
            if (isRigidbody)
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
            if (isRigidbody)
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
        if (isRigidbody)
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
}
