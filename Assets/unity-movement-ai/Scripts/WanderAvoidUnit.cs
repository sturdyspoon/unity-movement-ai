using UnityEngine;
using System.Collections;

public class WanderAvoidUnit : MonoBehaviour {

    private SteeringBasics steeringBasics;
    private Wander2 wander;
    private CollisionAvoidance colAvoid;

    private NearSensor colAvoidSensor;

    // Use this for initialization
    void Start()
    {
        steeringBasics = GetComponent<SteeringBasics>();
        wander = GetComponent<Wander2>();
        colAvoid = GetComponent<CollisionAvoidance>();

        colAvoidSensor = transform.Find("ColAvoidSensor").GetComponent<NearSensor>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 accel = colAvoid.getSteering(colAvoidSensor.targets);

        if (accel.magnitude < 0.005f)
        {
            accel = wander.getSteering();
        }

        steeringBasics.steer(accel);
        steeringBasics.lookWhereYoureGoing();
    }
}