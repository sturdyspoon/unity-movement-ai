using UnityEngine;
using System.Collections;

public class ScreenBoundary : MonoBehaviour {

    private Vector3 bottomLeft;
    private Vector3 topRight;
    private Vector3 widthHeight;

    // Use this for initialization
    void Start () {
        float z = -1*Camera.main.transform.position.z;

        bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, z));
        topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, z));
        widthHeight = topRight - bottomLeft;

        transform.localScale = new Vector3(widthHeight.x, widthHeight.y, transform.localScale.z);
    }

    void OnTriggerStay(Collider other)
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

        if (t.position.y < bottomLeft.y)
        {
            t.position = new Vector3(t.position.x, t.position.y + widthHeight.y, t.position.z);
        }

        if (t.position.y > topRight.y)
        {
            t.position = new Vector3(t.position.x, t.position.y - widthHeight.y, t.position.z);
        }
    }
}
