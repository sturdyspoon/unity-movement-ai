using UnityEngine;
using System.Collections;

public class FleeUnit : MonoBehaviour
{

    public Transform target;

    private SteeringBasics steeringBasics;
    private Flee flee;

    // Use this for initialization
    void Start()
    {
        steeringBasics = GetComponent<SteeringBasics>();
        flee = GetComponent<Flee>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 accel = flee.getSteering(target.position);

        steeringBasics.steer(accel);
        steeringBasics.lookWhereYoureGoing();
    }
}
