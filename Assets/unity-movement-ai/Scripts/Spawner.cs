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
    public GameObject[] thingsToAvoid;
    private GenericRigidbody[] rigidBodiesToAvoid;

    private Vector3 bottomLeft;
    private Vector3 widthHeight;

    [System.NonSerialized]
    public List<GenericRigidbody> objs = new List<GenericRigidbody>();

    // Use this for initialization
    void Start()
    {
        //Find the size of the map
        float z = -1 * Camera.main.transform.position.z;

        bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, z));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, z));
        widthHeight = topRight - bottomLeft;

        //Find the GenericRigidbodies' of the things to avoid
        rigidBodiesToAvoid = new GenericRigidbody[thingsToAvoid.Length];

        for (int i = 0; i < thingsToAvoid.Length; i++)
        {
            rigidBodiesToAvoid[i] = SteeringBasics.getGenericRigidbody(thingsToAvoid[i]);
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

            objs.Add(SteeringBasics.getGenericRigidbody(t.gameObject));

            return true;
        }

        return false;
    }

    private bool canPlaceObject(float halfSize, Vector3 pos)
    {
        //Make sure it does not overlap with any thing to avoid
        for (int i = 0; i < rigidBodiesToAvoid.Length; i++)
        {
            float dist = Vector3.Distance(rigidBodiesToAvoid[i].position, pos);

            if(dist < halfSize + rigidBodiesToAvoid[i].boundingRadius)
            {
                return false;
            }
        }

        //Make sure it does not overlap with any existing object
        foreach(GenericRigidbody o in objs)
        {
            float dist = Vector3.Distance(o.position, pos);

            if (dist < o.boundingRadius + spaceBetweenObjects + halfSize)
            {
                return false;
            }
        }

        return true;
    }
}
