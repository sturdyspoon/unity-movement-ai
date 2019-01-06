using UnityEngine;

namespace UnityMovementAI
{
    public class FirstPersonCamera : MonoBehaviour
    {
        public float speed = 15;

        public float xSensitivity = 2f;
        public float ySensitivity = 2f;

        public bool clampVerticalRotation = true;

        CursorLockMode wantedMode;

        void Start()
        {
            wantedMode = CursorLockMode.Locked;
        }

        void Update()
        {
            UpdateCursor();

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                RotateCamera();
                MoveCamera();
            }
        }

        void UpdateCursor()
        {
            /* Release cursor on escape keypress. */
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                wantedMode = CursorLockMode.None;
            }

            /* Lock cursor on click. */
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                wantedMode = CursorLockMode.Locked;
            }

            /* Apply requested cursor state. */
            Cursor.lockState = wantedMode;
            /* Hide cursor when locking */
            Cursor.visible = (CursorLockMode.Locked != wantedMode);
        }

        void RotateCamera()
        {
            float yRot = Input.GetAxis("Mouse X") * xSensitivity;
            float xRot = -1 * Input.GetAxis("Mouse Y") * ySensitivity;

            if (clampVerticalRotation)
            {
                xRot = ClampXAxisRotation(xRot);
            }

            transform.Rotate(new Vector3(xRot, 0f, 0f), Space.Self);
            transform.Rotate(new Vector3(0f, yRot, 0f), Space.World);
        }

        float ClampXAxisRotation(float xRot)
        {
            float curXRot = transform.localEulerAngles.x;
            float newXRot = curXRot + xRot;

            if (newXRot > 90 && newXRot < 270)
            {
                if (xRot > 0)
                {
                    xRot = 90 - curXRot;
                }
                else
                {
                    xRot = 270 - curXRot;
                }
            }

            return xRot;
        }

        void MoveCamera()
        {
            float vertKey = Input.GetAxisRaw("Vertical");
            float horKey = Input.GetAxisRaw("Horizontal");

            Vector3 moveDir = transform.right * horKey + transform.forward * vertKey;

            if (Input.GetButton("Jump"))
            {
                moveDir += transform.up;
            }

            transform.position += moveDir.normalized * speed * Time.deltaTime;
        }
    }
}