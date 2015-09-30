using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BipedalPlayer : MonoBehaviour {

	public GameObject spine;
	public GameObject thighLeft;
	public GameObject thighRight;
	public Collidable calfLeft;
	public Collidable calfRight;
	public GameObject skeletonSprings;
	public float speed;
	public float legDrag;
	public float kneeBend;
	public float legUpForce;
	public float calfMass;
	public float airTimeBeforeFall;
	private List<float> springVals = new List<float>();
	private float secondsInAir;

	// movement vectors
	private Dictionary<string, Vector3> mVecs = new Dictionary<string, Vector3>();
	// bone rigidbodies
	private Dictionary<string, Rigidbody> rigidBodies = new Dictionary<string, Rigidbody>();

	void Start () {
		rigidBodies["spine"] = spine.GetComponent<Rigidbody>();
		rigidBodies["thighLeft"] = thighLeft.GetComponent<Rigidbody>();
		rigidBodies["thighRight"] = thighRight.GetComponent<Rigidbody>();
		rigidBodies["calfLeft"] = calfLeft.GetComponent<Rigidbody>();
		rigidBodies["calfRight"] = calfRight.GetComponent<Rigidbody>();
	}

	void FixedUpdate () {
		updateMovementVectors();
		updateMovementConstraints();
	}


	private void updateMovementVectors () {
		float legLeftVel = Input.GetKey("a") ? -1 : 0;
		float legRightVel = Input.GetKey("d") ? -1 : 0;;

		mVecs["legLeft"] = new Vector3(legUpForce, legLeftVel, 0.0f);
		mVecs["legRight"] = new Vector3(legUpForce, legRightVel, 0.0f);
		mVecs["calfLeftRotate"] = new Vector3(0, 0, legLeftVel != 0 ? kneeBend : 0);
		mVecs["calfRightRotate"] = new Vector3(0, 0, legRightVel != 0 ? kneeBend : 0);

		if (legLeftVel != 0 || legRightVel != 0) {
			moveLegsForward();
		}

		// remove drag from the moving leg so it can move freely
		rigidBodies["calfLeft"].drag = legLeftVel == 0 ? legDrag : 0;
		rigidBodies["calfRight"].drag = legRightVel == 0 ? legDrag : 0;

		if (Input.GetKey("p")) {
			disableSkeletonSprings();
		}
		if (Input.GetKey("l")) {
			enableSkeletonSprings();
		}
	}

	private void updateMovementConstraints () {
		if (calfLeft.isCollidingWithFloor || calfRight.isCollidingWithFloor) {
			secondsInAir = 0;
		} else {
			secondsInAir += Time.deltaTime;
			Debug.Log("both in air " + secondsInAir);
		}
		if (secondsInAir > airTimeBeforeFall) {
			disableSkeletonSprings();
		}
	}

	private void moveLegsForward () {
		rigidBodies["thighLeft"].AddRelativeForce(mVecs["legLeft"] * (speed + 10));
		rigidBodies["thighRight"].AddRelativeForce(mVecs["legRight"] * (speed + 10));
		rigidBodies["calfLeft"].AddRelativeForce(mVecs["legLeft"] * (speed - 10));
		rigidBodies["calfRight"].AddRelativeForce(mVecs["legRight"] * (speed - 10));

		calfLeft.transform.Rotate(mVecs["calfLeftRotate"]);
		calfRight.transform.Rotate(mVecs["calfRightRotate"]);
	}

	private void disableSkeletonSprings () {
		SpringJoint[] springJoints;
		springJoints = skeletonSprings.GetComponentsInChildren<SpringJoint>();
		foreach (SpringJoint joint in springJoints) {
			springVals.Add(joint.spring);
			joint.spring = 0;
		}
	}

	private void enableSkeletonSprings () {
		SpringJoint[] springJoints;
		springJoints = skeletonSprings.GetComponentsInChildren<SpringJoint>();
		int i = 0;
		foreach (SpringJoint joint in springJoints) {
			joint.spring = springVals[i];
			i++;
		}
	}
}
