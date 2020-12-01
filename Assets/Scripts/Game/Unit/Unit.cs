using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Unit : SerializedMonoBehaviour {

  [SerializeField] private Transform attackPoint = null;
  public Transform AttackPoint => attackPoint != null ? attackPoint : transform;

  public ObjectAnimator objectAnimator = null;
  public new Rigidbody rigidbody = null;
  public UnitMotor motor = null;

  public bool CanMove => currentState == null || !currentState.restrictMovement;
  public bool CanRotate => currentState == null || !currentState.restrictRotation;
  public bool CanUseAbility => currentState == null || (currentState.cancelTime >= 0 && stateTime > currentState.cancelTime);

  private AttackState currentState;
  private AttackState nextState;
  private float stateTime = 0;
  private bool forceNextState = false;

  void Awake() {
  }

  void Start() {
  }

  void OnDestroy() {
  }

  void Update() {
    UpdateNextState();

    if (currentState == null)
      return;

    stateTime += Time.deltaTime;
    if (currentState.totalTime <= stateTime) {
      if (currentState.nextState != null) {
        SetNextState(currentState.nextState, true);
        UpdateNextState();
      } else {
        currentState.End();
        currentState = null;
      }
    } else {
      float progress = 1;
      if (currentState.totalTime > 0)
        progress = stateTime / currentState.totalTime;
      currentState.Update(progress);
      if (currentState.hitTime >= 0 && currentState.hitTime < currentState.totalTime - stateTime) {
        currentState.hitTime = -1;
        currentState.Hit();
      }
    }
  }

  void FixedUpdate() {
    if (currentState != null) {
      float progress = 1;
      if (currentState.totalTime > 0)
        progress = stateTime / currentState.totalTime;
      currentState.FixedUpdate(progress);
    }
  }

  public void SetNextState(AttackState attackState, bool forceNextState = false) {
    nextState = attackState;
    if (!this.forceNextState)
      this.forceNextState = forceNextState;
  }

  private void UpdateNextState() {
    if (nextState != null) {
      if (currentState != null) {
        if (!CanUseAbility && !forceNextState)
          return;
        else
          currentState.End();
      }
      forceNextState = false;

      currentState = nextState;
      nextState = null;
      currentState.Enter(this);

      stateTime = 0;
      objectAnimator.PlayAnimation(currentState.animationName, currentState.totalTime, currentState.blendTime);
    }
  }
}
