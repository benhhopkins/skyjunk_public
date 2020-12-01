using System.Collections.Generic;
using UnityEngine;
using UI;
using Utility;

public class PlayerMotor : UnitMotor {

  private Camera cam = null;
  private PlayerInput input = null;
  public PlayerInput Input => input;

  private Vector3 respawnPosition;
  private float respawning = 0;

  private GrapplingAbility grapplingAbility;
  private HitTarget hitTarget;

  public AttackStateDefinition triggerPress = null;
  public AttackStateDefinition doubleJump = null;

  public IntVariable scrap;
  public IntVariable steam;

  protected override void Awake() {
    base.Awake();
    cam = GameManager.I.BaseCamera;

    input = new PlayerInput();

    grapplingAbility = GetComponent<GrapplingAbility>();
    hitTarget = GetComponent<HitTarget>();

    respawnPosition = transform.position;

    scrap.Value = 0;
    steam.Value = 2;
  }

  void OnEnable() {
    input.Enable();
  }

  void OnDisable() {
    input.Disable();
  }

  void Update() {
    if (GameManager.I.GameState != GameState.Game)
      return;

    if (respawning > 0) {
      respawning -= Time.deltaTime;
      if (respawning <= 0) {
        UIGroupManager.I.SetUIGroupState("CloudWipe", false, false);
        transform.position = respawnPosition;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
      }
      return;
    }

    // calculate arrow keys/left stick normalizedTilt
    Vector2 inputTilt = input.Gameplay.Move.ReadValue<Vector2>();
    Vector3 moveDir = new Vector3(inputTilt.x, 0, inputTilt.y).normalized;

    float angle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
    Vector3 cameraMoveDirection = Quaternion.Euler(0, angle, 0) * Vector3.forward;
    Vector3 normalizedTilt = Mathf.Min(1, inputTilt.magnitude) * cameraMoveDirection;

    if (grapplingAbility.Grappling && !input.Gameplay.Ability2.IsPressed())
      grapplingAbility.StopGrapple();

    if (unit.CanMove)
      SetMoveInput(normalizedTilt, Grounded ? 1 : 0);
    else
      SetMoveInput(Vector3.zero, 0);

    if (unit.CanRotate && normalizedTilt.sqrMagnitude > 0.001f)
      SetTargetRotation(Quaternion.Euler(0, angle, 0), 20);

    if (unit.CanUseAbility) {
      if (grapplingAbility.Latched) {
        grapplingAbility.SetCurrentTilt(normalizedTilt);
        grapplingAbility.SetReelIn(input.Gameplay.Jump.IsPressed());
        SetMoveInput(Vector3.zero, 0);
      } else {
        if (input.Gameplay.Jump.WasPressedThisFrame()) {
          if (!Jump() && UseSteam()) {
            unit.SetNextState(doubleJump.Create());
          }
        }

        if (input.Gameplay.Jump.WasReleasedThisFrame())
          JumpCancel();

        if (input.Gameplay.Ability2.WasPressedThisFrame())
          grapplingAbility.StartGrapple();

        if (input.Gameplay.Ability1.WasPressedThisFrame() && UseSteam())
          unit.SetNextState(triggerPress.Create());

      }
    }

    if (Grounded)
      FillSteam(2);

    if (transform.position.y < -100) {
      UIGroupManager.I.SetUIGroupState("CloudWipe", true, false);
      respawning = 1.5f;

      hitTarget.TakeDamage(1, null);
    }

    if (scrap.Value >= 20) {
      GameManager.I.WinGame();
    }

    if (hitTarget.HP <= 0) {
      GameManager.I.LoseGame();
    }
  }

  public void TouchRespawnPoint(Vector3 position) {
    respawnPosition = position;

  }

  public void AddSteam(int amount) {
    FillSteam(steam.Value + amount);
  }

  public void FillSteam(int amount) {
    amount = Mathf.Min(5, amount);
    if (steam.Value < amount) {
      steam.Value = amount;
    }
  }

  public bool UseSteam() {
    if (steam.Value > 0) {
      steam.Value--;
      return true;
    }
    return false;
  }
}
