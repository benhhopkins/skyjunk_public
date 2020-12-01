using UnityEngine;

public class PlayerAnimator : ObjectAnimator {

  [SerializeField] private Unit unit = null;

  protected override void UpdateAnimationVariables() {
    animator.SetBool("Grounded", unit.motor.Grounded);

    Vector3 horizontalVel = unit.rigidbody.velocity;
    horizontalVel.y = 0;
    animator.SetFloat("HorizontalSpeed", horizontalVel.magnitude);
    animator.SetFloat("VerticalVelocity", unit.rigidbody.velocity.y);
  }

  protected override void UpdateDefaultAnimations() {
    Vector3 horizontalVel = unit.rigidbody.velocity;
    horizontalVel.y = 0;

    if (unit.motor.Grounded) {
      if (horizontalVel.magnitude > 0.05f)
        PlayAnimation("Run", 0, 0.1f);
      else
        PlayAnimation("Idle", 0, 0.1f);
    } else {
      PlayAnimation("AerialTree", 0, 0.1f);
    }
  }
}