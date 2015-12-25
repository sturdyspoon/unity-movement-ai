using UnityEngine;
using System.Collections;

public class ThirdPersonUnit : MonoBehaviour {

    public float speed = 5;

    public float xSensitivity = 2f;

    private MovementAIRigidbody rb;

    private float vertAxis = 0f;

    private Transform cam;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<MovementAIRigidbody>();
        cam = GetComponentInChildren<Camera>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        rotate();

        vertAxis = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        move();
    }

    private void rotate()
    {
        /* Rotate the character */
        float yRot = Input.GetAxisRaw("Horizontal") * xSensitivity;

        transform.Rotate(new Vector3(0f, yRot, 0f), Space.World);

        rotateCamera();
    }

    private void rotateCamera()
    {
        if(Input.GetMouseButton(0))
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

            cam.LookAt(transform.position + (rb.boundingRadius * Vector3.up));
        }
    }

    private void move()
    {
        Vector3 vel = transform.right * vertAxis * speed;
        rb.velocity = vel;
    }
}
