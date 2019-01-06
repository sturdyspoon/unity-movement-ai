using UnityEngine;

namespace UnityMovementAI
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        public float distance = 5f;
        public float sensitivityX = 4f;
        public float sensitivityY = 1f;
        public float minY = 10f;
        public float maxY = 50f;

        public Transform target;
        public Vector3 targetOffset = new Vector3(0f, 1f, 0f);

        float currentX;
        float currentY;

        public static CursorLockMode cursorMode;

        void Start()
        {
            cursorMode = CursorLockMode.Locked;
        }

        void Update()
        {
            UpdateCursor();

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                currentX += Input.GetAxis("Mouse X");
                currentY += -1 * Input.GetAxis("Mouse Y");

                currentY = Mathf.Clamp(currentY, minY, maxY);
            }
        }

        void UpdateCursor()
        {
            /* Release cursor on escape keypress */
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                cursorMode = CursorLockMode.None;
            }

            /* Lock cursor on click */
            if (target != null && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)))
            {
                cursorMode = CursorLockMode.Locked;
            }

            /* Apply requested cursor state */
            Cursor.lockState = cursorMode;
            /* Hide cursor when locking */
            Cursor.visible = (CursorLockMode.Locked != cursorMode);
        }

        void LateUpdate()
        {
            if (target != null)
            {
                Vector3 dir = new Vector3(0, 0, -distance);
                Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
                transform.position = target.position + targetOffset + (rotation * dir);
                transform.LookAt(target.position + targetOffset);
            }
        }
    }
}