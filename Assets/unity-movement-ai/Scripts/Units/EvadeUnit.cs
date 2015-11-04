using UnityEngine;
using System.Collections;

public class EvadeUnit : MonoBehaviour
{

    public GameObject target;
    private GenericRigidbody targetRigidBody;

    private SteeringBasics steeringBasics;
    private Evade evade;

    // Use this for initialization
    void Start()
    {
        targetRigidBody = SteeringBasics.getGenericRigidbody(target);

        steeringBasics = GetComponent<SteeringBasics>();
        evade = GetComponent<Evade>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 accel = evade.getSteering(targetRigidBody);

        steeringBasics.steer(accel);
        steeringBasics.lookWhereYoureGoing();
    }
}
