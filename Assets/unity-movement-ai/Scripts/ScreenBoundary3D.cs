using UnityEngine;

namespace UnityMovementAI
{
    public class ScreenBoundary3D : MonoBehaviour
    {

        private Vector3 bottomLeft;
        private Vector3 topRight;
        private Vector3 widthHeight;

        void Start()
        {
            float distAway = Mathf.Abs(Camera.main.transform.position.y);

            bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distAway));
            topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distAway));
            widthHeight = topRight - bottomLeft;

            transform.localScale = new Vector3(widthHeight.x, transform.localScale.y, widthHeight.z);
        }

        void OnTriggerStay(Collider other)
        {
            keepInBounds(other);
        }

        void OnTriggerExit(Collider other)
        {
            keepInBounds(other);
        }

        private void keepInBounds(Collider other)
        {
            Transform t = other.transform;

            if (t.position.x < bottomLeft.x)
            {
                t.position = new Vector3(t.position.x + widthHeight.x, t.position.y, t.position.z);
            }

            if (t.position.x > topRight.x)
            {
                t.position = new Vector3(t.position.x - widthHeight.x, t.position.y, t.position.z);
            }

            if (t.position.z < bottomLeft.z)
            {
                t.position = new Vector3(t.position.x, t.position.y, t.position.z + widthHeight.z);
            }

            if (t.position.z > topRight.z)
            {
                t.position = new Vector3(t.position.x, t.position.y, t.position.z - widthHeight.z);
            }
        }
    }
}