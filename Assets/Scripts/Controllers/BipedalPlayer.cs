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
  public float calfMass;
  // 0 - 1, factor by which the calfs shorten when moving (to help them not collide with the ground)
  public float calfShortening;
  public float airTimeBeforeFall;
  public float minThighRotation;
  public float maxThighRotation;
  public float thighRotationSpeed;

	// tracks how long both feet have been in the air
	private float secondsInAir;
  // bone movement vectors
  private Dictionary<string, Vector3> mVecs = new Dictionary<string, Vector3>();
  // bone rigid bodies
  private Dictionary<string, Rigidbody> rigidBodies = new Dictionary<string, Rigidbody>();
	// bone capsule colliders
	private Dictionary<string, CapsuleCollider> capsuleColliders = new Dictionary<string, CapsuleCollider>();
	// preserves the original capsule collider height so they can be restored
  private float capsuleColliderHeight;
	// preserves the original spring values so springs can be reinitialized
  private List<float> springVals = new List<float>();
	// velocities of the legs
	private float legLeftVel;
	private float legRightVel;

  void Start () {
    rigidBodies["spine"] = spine.GetComponent<Rigidbody>();
    rigidBodies["thighLeft"] = thighLeft.GetComponent<Rigidbody>();
    rigidBodies["thighRight"] = thighRight.GetComponent<Rigidbody>();
    rigidBodies["calfLeft"] = calfLeft.GetComponent<Rigidbody>();
    rigidBodies["calfRight"] = calfRight.GetComponent<Rigidbody>();

		capsuleColliders["calfLeft"] = calfLeft.GetComponent<CapsuleCollider>();
		capsuleColliders["calfRight"] = calfRight.GetComponent<CapsuleCollider>();
		capsuleColliderHeight = capsuleColliders["calfLeft"].height;
  }

  void FixedUpdate () {
    updateMovementVectors();
    moveLegs();
    updateMovementConstraints();
		handleSpecialInput();
  }


  private void updateMovementVectors () {
    legLeftVel = Input.GetKey("a") ? -1 : 0;
    legRightVel = Input.GetKey("d") ? -1 : 0;;

    mVecs["legLeft"] = new Vector3(0.0f, legLeftVel, 0.0f);
    mVecs["legRight"] = new Vector3(0.0f, legRightVel, 0.0f);
    mVecs["calfLeftRotate"] = new Vector3(0, 0, legLeftVel != 0 ? kneeBend : 0);
    mVecs["calfRightRotate"] = new Vector3(0, 0, legRightVel != 0 ? kneeBend : 0);
  }

  private void updateMovementConstraints () {
		// at least one calf is grounded
    if (calfLeft.isCollidingWithFloor || calfRight.isCollidingWithFloor) {
      secondsInAir = 0;
    } else { // both in air
      secondsInAir += Time.deltaTime;
    }
    if (secondsInAir > airTimeBeforeFall) {
      disableSkeletonSprings();
    }
  }

	private void handleSpecialInput () {
		if (Input.GetKey("p")) {
      disableSkeletonSprings();
    }
    if (Input.GetKey("l")) {
      enableSkeletonSprings();
    }
	}

  private void moveLegs () {
    rigidBodies["thighLeft"].AddRelativeForce(mVecs["legLeft"] * (speed + 10) * Time.deltaTime);
    rigidBodies["thighRight"].AddRelativeForce(mVecs["legRight"] * (speed + 10) * Time.deltaTime);
    rigidBodies["calfLeft"].AddRelativeForce(mVecs["legLeft"] * (speed - 10) * Time.deltaTime);
    rigidBodies["calfRight"].AddRelativeForce(mVecs["legRight"] * (speed - 10) * Time.deltaTime);

    calfLeft.transform.Rotate(mVecs["calfLeftRotate"] * Time.deltaTime);
    calfRight.transform.Rotate(mVecs["calfRightRotate"] * Time.deltaTime);

		// remove drag from the moving leg so it can move freely
    rigidBodies["calfLeft"].drag = legLeftVel == 0 ? legDrag : 0;
    rigidBodies["calfRight"].drag = legRightVel == 0 ? legDrag : 0;

		// adjust the height of the calf capsule colliders so they don't hit the ground as easily
		capsuleColliders["calfLeft"].height = capsuleColliderHeight * (legLeftVel == 0 ? 1 : calfShortening);
    capsuleColliders["calfRight"].height = capsuleColliderHeight * (legRightVel == 0 ? 1 : calfShortening);
    rotateThighJoints();
  }

  private void rotateThighJoints() {
    CharacterJoint thighJointRight = thighRight.GetComponent<CharacterJoint>();
    CharacterJoint thighJointLeft = thighLeft.GetComponent<CharacterJoint>();
    SoftJointLimit thighJointLimitRight = thighJointRight.lowTwistLimit;
    SoftJointLimit thighJointLimitLeft = thighJointLeft.lowTwistLimit;

    if (mVecs["legRight"] != Vector3.zero) {
      thighJointLimitRight.limit = Mathf.Min(maxThighRotation, thighJointLimitRight.limit + thighRotationSpeed);
    } else {
      thighJointLimitRight.limit = Mathf.Max(minThighRotation, thighJointLimitRight.limit - thighRotationSpeed);
    }

    if (mVecs["legLeft"] != Vector3.zero) {
      thighJointLimitLeft.limit = Mathf.Min(maxThighRotation, thighJointLimitLeft.limit + thighRotationSpeed);
    } else {
      thighJointLimitLeft.limit = Mathf.Max(minThighRotation, thighJointLimitLeft.limit - thighRotationSpeed);
    }

    thighJointRight.lowTwistLimit = thighJointLimitRight;
    thighJointLeft.lowTwistLimit = thighJointLimitLeft;
  }

  // makes character collapse
  private void disableSkeletonSprings () {
    SpringJoint[] springJoints;
    springJoints = skeletonSprings.GetComponentsInChildren<SpringJoint>();
    foreach (SpringJoint joint in springJoints) {
      springVals.Add(joint.spring);
      joint.spring = 0;
    }
  }

  // revives the character
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
