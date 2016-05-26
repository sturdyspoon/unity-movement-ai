using UnityEngine;
using System.Collections;

public class DebugSphereCast : MonoBehaviour {
    public float radius;
    public Vector3 hitPosition;
    public Vector3 hitSphereCenter;
    public float slopeLimit = 80f;

    // Use this for initialization
    void Start() {
        SphereCollider col = GetComponent<SphereCollider>();
        radius = Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z) * col.radius;
    }
	
	// Update is called once per frame
	void Update () {
        hitPosition = Vector3.zero;
        hitSphereCenter = Vector3.zero;

        RaycastHit hitInfo;
	    if(Physics.SphereCast(transform.position, radius, Vector3.down, out hitInfo))
        {
            hitPosition = hitInfo.point;
            hitSphereCenter = transform.position + (Vector3.down * (hitInfo.distance + radius));

            SteeringBasics.debugCross(hitPosition, 0.5f, Color.yellow);
            SteeringBasics.debugCross(hitSphereCenter, 0.5f, Color.red);

            if(isWall(hitInfo.normal))
            {
                /* Get vector pointing down the slope) */
                Vector3 rightSlope = Vector3.Cross(hitInfo.normal, Vector3.down);
                Vector3 downSlope = Vector3.Cross(rightSlope, hitInfo.normal);

                RaycastHit rayHitInfo;

                if (Physics.Raycast(hitInfo.point, downSlope, out rayHitInfo)) 
                {
                    SteeringBasics.debugCross(rayHitInfo.point, 0.5f, Color.magenta);
                }
            }
        }
	}

    private bool isWall(Vector3 surfNormal)
    {
        /* If the normal of the surface is greater then our slope limit then its a wall */
        return Vector3.Angle(Vector3.up, surfNormal) > slopeLimit;
    }
}
