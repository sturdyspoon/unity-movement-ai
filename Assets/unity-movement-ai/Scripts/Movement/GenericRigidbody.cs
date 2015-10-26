using UnityEngine;
using System.Collections;

public class GenericRigidbody {

    private Rigidbody rb;
    private Rigidbody2D rb2D;

    private bool isRigidbody;

    public GenericRigidbody(Rigidbody rb)
    {
        this.rb = rb;
        isRigidbody = true;
    }

    public GenericRigidbody(Rigidbody2D rb2D)
    {
        this.rb2D = rb2D;
        isRigidbody = false;
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
    }
