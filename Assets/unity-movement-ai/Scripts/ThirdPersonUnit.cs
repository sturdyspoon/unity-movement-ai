using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

[RequireComponent(typeof(MovementAIRigidbody))]
[RequireComponent(typeof(Camera))]
public class ThirdPersonUnit : MonoBehaviour {

    public float speed = 5f;

    public float facingSpeed = 720f;

    public float jumpSpeed = 7f;

    private MovementAIRigidbody rb;

    private Transform cam;

    private float horAxis = 0f;
    private float vertAxis = 0f;

    private Transform human;

    void Start()
    {
        rb = GetComponent<MovementAIRigidbody>();
        cam = Camera.main.transform;
        human = transform.Find("human");
    }

    void Update()
    {
        horAxis = Input.GetAxisRaw("Horizontal");
        vertAxis = Input.GetAxisRaw("Vertical");

        if(Input.GetButtonDown("Jump"))
        {
            rb.jump(jumpSpeed);
        }
    }


    void FixedUpdate()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            rb.velocity = getMovementDir() * speed;
        } else
        {
            rb.velocity = Vector3.zero;
        }
    }

    void LateUpdate()
    {
		if (Cursor.lockState == CursorLockMode.Locked) {
			Vector3 dir = getMovementDir ();

			if (dir.magnitude > 0) {
				float curFacing = human.eulerAngles.y;
				float facing = Mathf.Atan2 (-dir.z, dir.x) * Mathf.Rad2Deg;
				human.rotation = Quaternion.Euler (0, Mathf.MoveTowardsAngle (curFacing, facing, facingSpeed * Time.deltaTime), 0);
			}
		}
    }

    private Vector3 getMovementDir()
    {
        return ((cam.forward * vertAxis) + (cam.right * horAxis)).normalized;
    }
}
