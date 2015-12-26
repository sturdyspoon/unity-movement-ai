using UnityEngine;
using System.Collections;

public class ThirdPersonUnit : MonoBehaviour {

    public float speed = 5;

    public float turnSpeed = 120f;

    private MovementAIRigidbody rb;

    private float horAxis = 0f;
    private float vertAxis = 0f;

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
        Vector3 vel = transform.right * vertAxis * speed;
        rb.velocity = vel;
    }

    private void rotateChar()
    {
        float yRot = horAxis * turnSpeed * Time.deltaTime;
        rb.MoveRotation(transform.eulerAngles.y + yRot);
    }
}
