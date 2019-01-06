using UnityEngine;
using System.Collections.Generic;

namespace UnityMovementAI
{
    public class Spawner : MonoBehaviour
    {
        public Transform obj;
        public Vector2 objectSizeRange = new Vector2(1, 2);

        public int numberOfObjects = 10;
        public bool randomizeOrientation = false;

        public float boundaryPadding = 1f;
        public float spaceBetweenObjects = 1f;

        public MovementAIRigidbody[] thingsToAvoid;

        Vector3 bottomLeft;
        Vector3 widthHeight;

        bool isObj3D;

        [System.NonSerialized]
        public List<MovementAIRigidbody> objs = new List<MovementAIRigidbody>();

        void Start()
        {
            MovementAIRigidbody rb = obj.GetComponent<MovementAIRigidbody>();
            /* Manually set up the MovementAIRigidbody since the given obj can be a prefab */
            rb.SetUp();
            isObj3D = rb.is3D;

            /* Find the size of the map */
            float distAway = Camera.main.WorldToViewportPoint(Vector3.zero).z;

            bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distAway));
            Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distAway));
            widthHeight = topRight - bottomLeft;

            /* Create the create the objects */
            for (int i = 0; i < numberOfObjects; i++)
            {
                /* Try to place the objects multiple times before giving up */
                for (int j = 0; j < 10; j++)
                {
                    if (TryToCreateObject())
                    {
                        break;
                    }
                }
            }
        }

        bool TryToCreateObject()
        {
            float size = Random.Range(objectSizeRange.x, objectSizeRange.y);
            float halfSize = size / 2f;

            Vector3 pos = new Vector3();
            pos.x = bottomLeft.x + Random.Range(boundaryPadding + halfSize, widthHeight.x - boundaryPadding - halfSize);

            if (isObj3D)
            {
                pos.z = bottomLeft.z + Random.Range(boundaryPadding + halfSize, widthHeight.z - boundaryPadding - halfSize);
            }
            else
            {
                pos.y = bottomLeft.y + Random.Range(boundaryPadding + halfSize, widthHeight.y - boundaryPadding - halfSize);
            }

            if (CanPlaceObject(halfSize, pos))
            {
                Transform t = Instantiate(obj, pos, Quaternion.identity) as Transform;

                if (isObj3D)
                {
                    t.localScale = new Vector3(size, obj.localScale.y, size);
                }
                else
                {
                    t.localScale = new Vector3(size, size, obj.localScale.z);
                }

                if (randomizeOrientation)
                {
                    Vector3 euler = transform.eulerAngles;
                    if (isObj3D)
                    {
                        euler.y = Random.Range(0f, 360f);
                    }
                    else
                    {
                        euler.z = Random.Range(0f, 360f);
                    }

                    transform.eulerAngles = euler;
                }

                objs.Add(t.GetComponent<MovementAIRigidbody>());

                return true;
            }

            return false;
        }

        bool CanPlaceObject(float halfSize, Vector3 pos)
        {
            /* Make sure it does not overlap with any thing to avoid */
            for (int i = 0; i < thingsToAvoid.Length; i++)
            {
                float dist = Vector3.Distance(thingsToAvoid[i].Position, pos);

                if (dist < halfSize + thingsToAvoid[i].Radius)
                {
                    return false;
                }
            }

            /* Make sure it does not overlap with any existing object */
            foreach (MovementAIRigidbody o in objs)
            {
                float dist = Vector3.Distance(o.Position, pos);

                if (dist < o.Radius + spaceBetweenObjects + halfSize)
                {
                    return false;
                }
            }

            return true;
        }
    }
}