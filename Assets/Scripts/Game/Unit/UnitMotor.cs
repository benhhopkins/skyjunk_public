using UnityEngine;
using Utility;

// UnitMotor is in charge of moving a unit and simulating it's physics
public class UnitMotor : MonoBehaviour {

  protected Unit unit;
  protected new Collider collider;

  public bool Grounded { get; private set; } = false;

  public float gravity = 30;

  public float maxSpeed = 10;
  public float acceleration = 10;
  public float deceleration = 10;

  public float jumpSpeed = 10;
  public float jumpTime = 2;
  private float jumpTimeRemaining = 0;

  // how near the feet need to be to the ground to start being grounded
  public float groundSensorLength = 0.1f;
  // how far the feet need to be from the ground to stop being grounded (when not jumping)
  public float stepSensorLength = 0.3f;
  // how long after being grounded can we still jump
  public float jumpGraceTime = 0.25f;
  private float jumpGraceTimeRemaining = 0;

  public SoundClip jumpSound = null;
  public SoundClip landSound = null;

  private Vector3 moveInput;
  public Vector3 MoveInput => moveInput;
  private float decelerationFactor = 1;

  private Quaternion targetRotation;
  private float rotationSpeedDegrees;

  protected virtual void Awake() {
    unit = GetComponent<Unit>();
    collider = GetComponent<Collider>();

    targetRotation = transform.rotation;
  }

  protected virtual void FixedUpdate() {
    UpdateMovement();
    UpdateRotation();
    UpdateJump();
    UpdateGroundPhysics();
  }

  public void SetMoveInput(Vector3 moveInput, float decelerationFactor) {
    this.moveInput = moveInput;
    this.decelerationFactor = decelerationFactor;
  }

  public void SetTargetRotation(Quaternion targetRotation, float rotationSpeedDegrees) {
    this.targetRotation = targetRotation;
    this.rotationSpeedDegrees = rotationSpeedDegrees;
  }

  private void UpdateMovement() {
    Vector3 move = maxSpeed * moveInput;

    float currentDecel = decelerationFactor * deceleration;

    Vector3 velChange = Vector3.zero;
    int i = 0;
    velChange[i] = CalculateVelChange(
        unit.rigidbody.velocity[i], move[i], acceleration, currentDecel);
    i = 2;
    velChange[i] = CalculateVelChange(
        unit.rigidbody.velocity[i], move[i], acceleration, currentDecel);

    unit.rigidbody.AddForce(velChange, ForceMode.VelocityChange);
  }

  private static float CalculateVelChange(float current, float target, float accel, float decel) {
    float targetDifference = target - current;
    float maxChange = decel * Time.fixedDeltaTime;

    if (Mathf.Abs(target) > Mathf.Abs(current) || System.Math.Sign(current) + System.Math.Sign(target) == 0)
      maxChange = accel * Time.fixedDeltaTime;

    return Mathf.Clamp(targetDifference, -maxChange, maxChange);
  }

  private void UpdateRotation() {
    var newRotation = Quaternion.RotateTowards(unit.rigidbody.rotation, targetRotation, rotationSpeedDegrees);
    unit.rigidbody.MoveRotation(newRotation);
    unit.rigidbody.angularVelocity = Vector3.zero;
  }

  private void UpdateJump() {
    if (jumpTimeRemaining > 0) {
      jumpTimeRemaining -= Time.fixedDeltaTime;
      if (jumpTimeRemaining > 0) {
        var jumpVelocity = Vector3.up * Mathf.Max(0, jumpSpeed - unit.rigidbody.velocity.y);

        unit.rigidbody.AddForce(jumpVelocity, ForceMode.VelocityChange);
      } else {
        var jumpVelocity = Vector3.up * -jumpSpeed / 5;
        unit.rigidbody.AddForce(jumpVelocity, ForceMode.VelocityChange);
      }
    }
  }

  private void UpdateGroundPhysics() {
    RaycastHit hitInfo;
    if (jumpTimeRemaining > 0)
      Grounded = false;
    else {
      Vector3 colliderBottom = new Vector3(collider.bounds.center.x, collider.bounds.min.y, collider.bounds.center.z);
      bool wasGrounded = Grounded;
      //if (!Grounded)
      Grounded = Physics.Raycast(
        colliderBottom + 0.1f * Vector3.up,
        (groundSensorLength + 0.1f) * Vector3.down,
        out hitInfo, 0.3f, LayerManager.I.WalkableMask, QueryTriggerInteraction.Ignore);

      if (Grounded) {
        //unit.rigidbody.MovePosition(hitInfo.point - (colliderBottom - unit.rigidbody.position));
        unit.rigidbody.AddForce(Mathf.Max(0, unit.rigidbody.velocity.y) * Vector3.down, ForceMode.VelocityChange);
        jumpGraceTimeRemaining = jumpGraceTime;

        if (!wasGrounded && landSound != null) {
          var sound = landSound.Play(transform.position);
        }
      }
    }

    if (!Grounded) {
      unit.rigidbody.AddForce(gravity * Vector3.down, ForceMode.Acceleration);
      jumpGraceTimeRemaining -= Time.fixedDeltaTime;
    }
  }

  public bool Jump() {
    if (Grounded || jumpGraceTimeRemaining > 0) {
      jumpTimeRemaining = jumpTime;
      if (jumpSound != null)
        jumpSound.Play(transform.position);
      return true;
    }
    return false;
  }

  public void JumpCancel() {
    if (jumpTimeRemaining > 0) {
      var jumpVelocity = Vector3.up * -jumpSpeed / 3;
      unit.rigidbody.AddForce(jumpVelocity, ForceMode.VelocityChange);
    }
    jumpTimeRemaining = 0;
  }

  void OnDrawGizmos() {
    var collider = GetComponent<Collider>();

    Gizmos.color = Color.red;
    var rayStart = new Vector3(collider.bounds.center.x, collider.bounds.min.y, collider.bounds.center.z) + 0.1f * Vector3.up;
    Gizmos.DrawLine(rayStart, rayStart + (groundSensorLength + 0.1f) * Vector3.down);

    if (Application.isPlaying) {
      RaycastHit hitInfo;
      Physics.Raycast(
        new Vector3(collider.bounds.center.x, collider.bounds.min.y, collider.bounds.center.z) + 0.1f * Vector3.up,
        (groundSensorLength + 0.1f) * Vector3.down,
        out hitInfo, 0.3f, LayerManager.I.WalkableMask, QueryTriggerInteraction.Ignore);
      Gizmos.DrawSphere(hitInfo.point, 0.2f);
    }
  }
}