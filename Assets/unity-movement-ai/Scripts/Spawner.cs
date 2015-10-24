using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

    public Transform obj;
    public Vector2 objectSizeRange = new Vector2(1, 2);
    public int numberOfObjects = 10;
    public bool randomizeOrientation = false;
    public float boundaryPadding = 1f;
    public float spaceBetweenObjects = 1f;
    public Transform[] thingsToAvoid;

    private Vector3 bottomLeft;
    private Vector3 widthHeight;

    private float[] thingsToAvoidRadius;

    [System.NonSerialized]
    public List<Rigidbody> objs = new List<Rigidbody>();

    // Use this for initialization
    void Start()
    {
        //Find the size of the map
        float z = -1 * Camera.main.transform.position.z;

        bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, z));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, z));
        widthHeight = topRight - bottomLeft;

        //Find the radius' of the things to avoid
        thingsToAvoidRadius = new float[thingsToAvoid.Length];

        for (int i = 0; i < thingsToAvoid.Length; i++)
        {
            thingsToAvoidRadius[i] = SteeringBasics.getBoundingRadius(thingsToAvoid[i].transform);
        }

        //Create the create the objects
        for (int i = 0; i < numberOfObjects; i++)
        {
            //Try to place the objects multiple times before giving up
            for(int j = 0; j < 10; j++)
            {
                if(tryToCreateObject())
                {
                    break;
                }
            }
        }
    }

    private bool tryToCreateObject()
    {
        float size = Random.Range(objectSizeRange.x, objectSizeRange.y);
        float halfSize = size / 2f;

        Vector3 pos = new Vector3();
        pos.x = bottomLeft.x + Random.Range(boundaryPadding + halfSize, widthHeight.x - boundaryPadding - halfSize);
        pos.y = bottomLeft.y + Random.Range(boundaryPadding + halfSize, widthHeight.y - boundaryPadding - halfSize);

        if(canPlaceObject(halfSize, pos))
        {
            Transform t = Instantiate(obj, pos, Quaternion.identity) as Transform;
            t.localScale = new Vector3(size, size, obj.localScale.z);

            if(randomizeOrientation)
            {
                Vector3 euler = transform.eulerAngles;
                euler.z = Random.Range(0f, 360f);
                transform.eulerAngles = euler;
            }

            objs.Add(t.GetComponent<Rigidbody>());

            return true;
        }

        return false;
    }

    private bool canPlaceObject(float halfSize, Vector3 pos)
    {
        //Make sure it does not overlap with any thing to avoid
        for (int i = 0; i < thingsToAvoid.Length; i++)
        {
            float dist = Vector3.Distance(thingsToAvoid[i].position, pos);

            if(dist < halfSize + thingsToAvoidRadius[i])
            {
                return false;
            }
        }

        //Make sure it does not overlap with any existing object
        foreach(Rigidbody o in objs)
        {
            float dist = Vector3.Distance(o.position, pos);

            float oRadius = SteeringBasics.getBoundingRadius(o.transform);

            if (dist < oRadius + spaceBetweenObjects + halfSize)
            {
                return false;
            }
        }

        return true;
    }
}
