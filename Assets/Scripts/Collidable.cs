using UnityEngine;
using System.Collections;

public class Collidable : MonoBehaviour {

	public bool isCollidingWithFloor = false;

	// Use this for initialization
	void Start () {

	}

	void OnCollisionEnter (Collision collision) {
		isCollidingWithFloor = true;
	}

	void OnCollisionStay (Collision collision) {
	}

	void OnCollisionExit (Collision collision) {
		isCollidingWithFloor = false;
	}
}
