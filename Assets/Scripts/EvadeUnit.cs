using UnityEngine;
using System.Collections;

public class EvadeUnit : MonoBehaviour
{

    public Rigidbody target;

    private SteeringBasics steeringBasics;
    private Evade evade;

    // Use this for initialization
    void Start()
    {
        steeringBasics = GetComponent<SteeringBasics>();
        evade = GetComponent<Evade>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 accel = evade.getSteering(target);

        steeringBasics.steer(accel);
        steeringBasics.lookWhereYoureGoing();
    }
}
