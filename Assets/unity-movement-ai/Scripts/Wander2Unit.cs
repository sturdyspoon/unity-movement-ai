using UnityEngine;
using System.Collections;

public class Wander2Unit : MonoBehaviour
{

    private SteeringBasics steeringBasics;
    private Wander2 wander;

    // Use this for initialization
    void Start()
    {
        steeringBasics = GetComponent<SteeringBasics>();
        wander = GetComponent<Wander2>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 accel = wander.getSteering();

        steeringBasics.steer(accel);
        steeringBasics.lookWhereYoureGoing();
    }
}
