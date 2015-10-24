using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public Transform target;

	private Vector3 displacement;
	// Use this for initialization
	void Start () {
		//target = GameObject.Find ("Player").transform;

		displacement = transform.position - target.position;
	}
	
	// LateUpdate is called once per frame after the other normal Update functions have already run
	void LateUpdate () {
		//Debug.Log (Vector2.Distance (transform.position, target.position));
		if(target != null) {
			transform.position = target.position + displacement;
		}
	}
}
