using UnityEngine;
using Utility;

public class PlayerWindup : AttackState {
  protected Quaternion turnAngle = Quaternion.identity;

  public SoundClip soundClip = null;

  public override void Enter(Unit unit) {
    base.Enter(unit);
    Quaternion lookDirection = GameManager.I.BaseCamera.transform.rotation;
    if (unit.motor != null)
      unit.motor.SetTargetRotation(lookDirection, 30);

    if (soundClip != null)
      soundClip.Play(unit.transform.position);
  }
}

public class PlayerBaseAttack : AttackState {

  public float dashSpeed = 20;

  public HitTrigger hitTriggerPrefab = null;
  public ParticleSystem slashEffectPrefab = null;

  public SoundClip soundClip = null;

  private Quaternion lookDirection;

  public override void Enter(Unit unit) {
    base.Enter(unit);
    lookDirection = GameManager.I.BaseCamera.transform.rotation;
    if (unit.motor != null)
      unit.motor.SetTargetRotation(lookDirection, 30);

    dashSpeed = Mathf.Max(dashSpeed, unit.rigidbody.velocity.magnitude / 2 + dashSpeed);

    if (soundClip != null)
      soundClip.Play(unit.transform.position);
  }

  public override void Update(float progress) {
  }

  public override void FixedUpdate(float progress) {
    if (progress < 0.5f) {
      Vector3 targetVelocity = lookDirection * Vector3.forward * dashSpeed;
      Vector3 velocityChange = targetVelocity - unit.rigidbody.velocity;
      unit.rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
    }
  }

  public override void Hit() {
    var hitTrigger = GameObject.Instantiate(hitTriggerPrefab,
      unit.AttackPoint.position, unit.AttackPoint.rotation, unit.AttackPoint);
    hitTrigger.source = unit;

    var slashEffect = GameObject.Instantiate(slashEffectPrefab,
      unit.AttackPoint.position, unit.AttackPoint.rotation);
  }

  public override void End() {
    if (unit.motor != null) {
      var euler = lookDirection.eulerAngles;
      euler.x = 0;
      euler.z = 0;
      unit.motor.SetTargetRotation(Quaternion.Euler(euler), 5);
    }
  }
}

public class PlayerDoubleJump : AttackState {
  public float jumpVelocity = 15;
  public float holdVelocity = 10;
  public float boostPower = 10;
  private PlayerMotor playerMotor;

  public SoundClip soundClip = null;

  public override void Enter(Unit unit) {
    base.Enter(unit);
    playerMotor = unit.GetComponent<PlayerMotor>();
    float velocityChange = Mathf.Max(0, Mathf.Max(unit.rigidbody.velocity.y, jumpVelocity) - unit.rigidbody.velocity.y);
    // can't use AddForce because we want to read the correct velocity value later this frame
    unit.rigidbody.velocity = unit.rigidbody.velocity + Vector3.up * velocityChange;

    if (soundClip != null)
      soundClip.Play(unit.transform.position);
  }

  public override void FixedUpdate(float progress) {
    if (playerMotor.Input.Gameplay.Jump.IsPressed()) {
      float velocityChange = Mathf.Max(0, Mathf.Max(unit.rigidbody.velocity.y, holdVelocity) - unit.rigidbody.velocity.y);
      unit.rigidbody.velocity = unit.rigidbody.velocity + Vector3.up * velocityChange;

      if (playerMotor.MoveInput.sqrMagnitude > 0.1f) {
        unit.rigidbody.AddForce(boostPower * playerMotor.MoveInput, ForceMode.Acceleration);
      }
    }
  }
}
