using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuadripedalPlayerController : MonoBehaviour {

  public GameObject spine;

  public GameObject thighRearLeft;
  public GameObject thighRearRight;
  public GameObject calfRearLeft;
  public GameObject calfRearRight;

  public GameObject thighFrontLeft;
  public GameObject thighFrontRight;
  public GameObject calfFrontLeft;
  public GameObject calfFrontRight;

  public float speed;
  public float legDrag;
  public float kneeBend;

  // movement vectors
  private Dictionary<string, Vector3> mVecs = new Dictionary<string, Vector3>();
  // bone rigidbodies
  private Dictionary<string, Rigidbody> rigidBodies = new Dictionary<string, Rigidbody>();

  void Start () {
    rigidBodies["spine"] = spine.GetComponent<Rigidbody>();
    rigidBodies["thighFrontLeft"] = thighFrontLeft.GetComponent<Rigidbody>();
    rigidBodies["thighFrontRight"] = thighFrontRight.GetComponent<Rigidbody>();
    rigidBodies["calfFrontLeft"] = calfFrontLeft.GetComponent<Rigidbody>();
    rigidBodies["calfFrontRight"] = calfFrontRight.GetComponent<Rigidbody>();
    rigidBodies["thighRearLeft"] = thighRearLeft.GetComponent<Rigidbody>();
    rigidBodies["thighRearRight"] = thighRearRight.GetComponent<Rigidbody>();
    rigidBodies["calfRearLeft"] = calfRearLeft.GetComponent<Rigidbody>();
    rigidBodies["calfRearRight"] = calfRearRight.GetComponent<Rigidbody>();
  }

  void FixedUpdate () {
    updateMovementVectors();
  }


  private void updateMovementVectors () {
    float legFrontLeftVel = Input.GetKey("q") ? -1 : 0;
    float legFrontRightVel = Input.GetKey("e") ? -1 : 0;
    float legRearLeftVel = Input.GetKey("a") ? -1 : 0;
    float legRearRightVel = Input.GetKey("d") ? -1 : 0;

    mVecs["legFrontLeft"] = new Vector3(0.0f, legFrontLeftVel, 0.0f);
    mVecs["legFrontRight"] = new Vector3(0.0f, legFrontRightVel, 0.0f);
    mVecs["calfFrontLeftRotate"] = new Vector3(0, 0, legFrontLeftVel != 0 ? -kneeBend : 0);
    mVecs["calfFrontRightRotate"] = new Vector3(0, 0, legFrontRightVel != 0 ? -kneeBend : 0);
    mVecs["legRearLeft"] = new Vector3(0.0f, legRearLeftVel, 0.0f);
    mVecs["legRearRight"] = new Vector3(0.0f, legRearRightVel, 0.0f);
    mVecs["calfRearLeftRotate"] = new Vector3(0, 0, legRearLeftVel != 0 ? kneeBend : 0);
    mVecs["calfRearRightRotate"] = new Vector3(0, 0, legRearRightVel != 0 ? kneeBend : 0);

    if (legFrontLeftVel != 0 || legFrontRightVel != 0 || legRearLeftVel != 0 || legRearRightVel != 0) {
      moveLegsForward();
    }

    // remove drag from the moving leg so it can move freely
    rigidBodies["calfFrontLeft"].drag = legFrontLeftVel == 0 ? legDrag : 0;
    rigidBodies["calfFrontRight"].drag = legFrontRightVel == 0 ? legDrag : 0;
    rigidBodies["calfRearLeft"].drag = legRearLeftVel == 0 ? legDrag : 0;
    rigidBodies["calfRearRight"].drag = legRearRightVel == 0 ? legDrag : 0;
  }

  private void moveLegsForward () {
    rigidBodies["thighFrontLeft"].AddRelativeForce(mVecs["legFrontLeft"] * (speed + 10));
    rigidBodies["thighFrontRight"].AddRelativeForce(mVecs["legFrontRight"] * (speed + 10));
    rigidBodies["calfFrontLeft"].AddRelativeForce(mVecs["legFrontLeft"] * (speed - 10));
    rigidBodies["calfFrontRight"].AddRelativeForce(mVecs["legFrontRight"] * (speed - 10));
    rigidBodies["thighRearLeft"].AddRelativeForce(mVecs["legRearLeft"] * (speed + 10));
    rigidBodies["thighRearRight"].AddRelativeForce(mVecs["legRearRight"] * (speed + 10));
    rigidBodies["calfRearLeft"].AddRelativeForce(mVecs["legRearLeft"] * (speed - 10));
    rigidBodies["calfRearRight"].AddRelativeForce(mVecs["legRearRight"] * (speed - 10));

    calfFrontLeft.transform.Rotate(mVecs["calfFrontLeftRotate"]);
    calfFrontRight.transform.Rotate(mVecs["calfFrontRightRotate"]);
    calfRearLeft.transform.Rotate(mVecs["calfRearLeftRotate"]);
    calfRearRight.transform.Rotate(mVecs["calfRearRightRotate"]);
  }

  // private void moveLegsBackward (Vector3 mVecs["legLeft"], Vector3 mVecs["legRight"]) {
  //   rigidBodies["calfLeft"].AddRelativeForce(mVecs["legLeft"] * (speed - 10));
  //   rigidBodies["calfRight"].AddRelativeForce(mVecs["legRight"] * (speed - 10));
  // }
}
