using System.Collections.Generic;
using UnityEngine;
using UI;

public class RoobotMotor : UnitMotor {

  public Transform armTransform;
  public AttackStateDefinition attack = null;
  public float attackCooldown = 2;
  public float targetingRange = 100;

  private Quaternion baseArmRotation;
  private Vector3 startPosition;
  private float currentCooldown = 10;
  private Transform target;

  protected override void Awake() {
    base.Awake();
    startPosition = transform.position;
    baseArmRotation = armTransform.localRotation;
  }

  void Start() {
    target = LevelTracker.I.player.transform;
  }

  void Update() {
    if (Time.timeScale == 0 || target == null)
      return;

    // check position of target so we don't shoot player at spawn
    if (currentCooldown > 0 && target.position.sqrMagnitude > 5)
      currentCooldown -= Time.deltaTime;

    float targetDistance = Vector3.Distance(target.position, transform.position);
    if (targetDistance < targetingRange) {

      if (unit.CanRotate) {
        var targetVector = target.position - transform.position;
        float yDiff = targetVector.y;
        targetVector.y = 0;
        float baseAngle = Mathf.Atan2(targetVector.x, targetVector.z) * Mathf.Rad2Deg;
        SetTargetRotation(Quaternion.Euler(0, baseAngle, 0), 2);

        float armAngle = Mathf.Atan2(yDiff, targetVector.magnitude) * Mathf.Rad2Deg;
        armTransform.localRotation = Quaternion.Euler(-armAngle, 0, 0) * baseArmRotation;
      }

      if (unit.CanUseAbility) {
        if (currentCooldown <= 0) {
          unit.SetNextState(attack.Create());
          currentCooldown = attackCooldown + Random.Range(0, 2);
        }
      }

    }
  }
}
