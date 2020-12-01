using UnityEngine;
using Utility;

public class EnemyWindup : AttackState {
  protected Quaternion turnAngle = Quaternion.identity;

  public SoundClip soundClip;

  public override void Enter(Unit unit) {
    base.Enter(unit);

    if (soundClip != null)
      soundClip.Play(unit.transform.position);
    // if (unit.motor != null)
    //   unit.motor.SetTargetRotation(lookDirection, 30);
  }
}

public class EnemyShootProjectile : AttackState {

  public Rigidbody projectilePrefab = null;
  public ParticleSystem effectPrefab = null;

  public float launchSpeed = 10;

  public SoundClip soundClip;

  public override void Enter(Unit unit) {
    base.Enter(unit);
  }

  public override void Update(float progress) {
  }

  public override void FixedUpdate(float progress) {

  }

  public override void Hit() {
    var projectile = GameObject.Instantiate(projectilePrefab,
      unit.AttackPoint.position, unit.AttackPoint.rotation);

    var player = LevelTracker.I.player;
    if (player != null) {
      Vector3 shootVector = unit.AttackPoint.forward;
      shootVector.y = (player.transform.position - unit.AttackPoint.position).normalized.y;
      projectile.AddForce(shootVector * launchSpeed, ForceMode.VelocityChange);
    } else
      projectile.AddForce(unit.AttackPoint.forward * launchSpeed, ForceMode.VelocityChange);

    if (soundClip != null)
      soundClip.Play(unit.transform.position);

    // var slashEffect = GameObject.Instantiate(slashEffectPrefab,
    //   unit.AttackPoint.position, unit.AttackPoint.rotation);
  }

  public override void End() {
    // if (unit.motor != null) {
    //   var euler = lookDirection.eulerAngles;
    //   euler.x = 0;
    //   euler.z = 0;
    //   unit.motor.SetTargetRotation(Quaternion.Euler(euler), 5);
    // }
  }
}