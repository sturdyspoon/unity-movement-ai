using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

[RequireComponent(typeof(MovementAIRigidbody))]
[RequireComponent(typeof(Camera))]
public class ThirdPersonUnit : MonoBehaviour {

    public float speed = 5;

    public float turnSpeed = 120f;

    private MovementAIRigidbody rb;

    private float horAxis = 0f;
    private float vertAxis = 0f;
    private float sideStepDir = 0f;

    private Transform cam;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<MovementAIRigidbody>();
        cam = GetComponentInChildren<Camera>().transform;

        lookAtChar();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            rotateCamera();
        }

        horAxis = Input.GetAxisRaw("Horizontal");
        vertAxis = Input.GetAxisRaw("Vertical");
        //vertAxis = 1f;

        if (Input.GetKey(KeyCode.Q))
        {
            sideStepDir = 1f;
        } else if (Input.GetKey(KeyCode.E))
        {
            sideStepDir = -1f;
        } else
        {
            sideStepDir = 0f;
        }
    }

    private void rotateCamera()
    {
        float radius = cam.localPosition.magnitude;
        float theta = Mathf.Acos(cam.localPosition.y / radius);
        float phi = Mathf.Atan(cam.localPosition.z / cam.localPosition.x);

        theta += Input.GetAxis("Mouse Y") * Mathf.Deg2Rad;

        Vector3 newPos = new Vector3();
        newPos.x = -1 * radius * Mathf.Sin(theta) * Mathf.Cos(phi);
        newPos.y = radius * Mathf.Cos(theta);
        newPos.z = radius * Mathf.Sin(theta) * Mathf.Sin(phi);

        cam.localPosition = newPos;

        lookAtChar();
    }

    private void lookAtChar()
    {
        cam.LookAt(transform.position + (rb.boundingRadius * Vector3.up));
    }

    void FixedUpdate()
    {
        rotateChar();
        moveChar();
    }

    private void moveChar()
    {
        Vector3 vel = (transform.right * vertAxis) + (transform.forward * sideStepDir);
        vel = vel.normalized * speed;
        rb.velocity = vel;
    }

    private void rotateChar()
    {
        rb.angularVelocity = horAxis * turnSpeed * Mathf.Deg2Rad;
        // Clear out any x/z orientation
        rb.rotation = Quaternion.Euler(0, rb.rotation.eulerAngles.y, 0);
    }
}
