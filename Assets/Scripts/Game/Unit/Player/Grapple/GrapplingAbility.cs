using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class GrapplingAbility : MonoBehaviour {

  // hook targeting
  public float radius;
  public float minRange;
  public float maxLookAngle;
  public float graceLookTime;

  // grappling action
  public float latchSpeed;
  public float swingMaxSpeed;
  public float reelSpeed;
  public float reelAccel;
  private float latchStartSpeed;
  //public float obstacleGraceTime;

  public bool Grappling { get; private set; }
  public bool Latched { get; private set; }

  [SerializeField] private Rigidbody sourceRigidbody = null;
  [SerializeField] private Transform ropeEffectSource = null;
  [SerializeField] private Transform armHookTransform = null;
  [SerializeField] private GrapplingRope ropeEffectPrefab = null;
  [SerializeField] private UI.UIGfxWorldObject targetGraphicPrefab = null;

  [SerializeField] private Transform armTarget = null;
  [SerializeField] private float armTargetMaxX = -.2f;
  [SerializeField] private float armTargetMaxDistance = 0.4f;

  public SoundClip hookLaunch = null;
  public SoundClip hookLatch = null;

  private float lookAwayTime;
  private HookPoint currentHook;
  private Transform cameraTransform;

  private float latchTotalTime;
  private float latchTime;
  //private float obstacleTime;
  private bool reelIn;
  private Vector3 normalizedTilt;
  private Rigidbody hookRigidbody;
  private GrapplingRope ropeEffect;
  private Vector3 armTargetBasePosition;

  private UI.UIGfxWorldObject targetGraphic;

  void Awake() {
    cameraTransform = GameManager.I.BaseCamera.transform;

    armTargetBasePosition = armTarget.localPosition;
  }

  void OnDisable() {
    if (targetGraphic != null) {
      targetGraphic.ReturnToPool();
      targetGraphic = null;
    }
  }

  void Update() {
    if (Grappling) { // remove the target graphic if we're grappling
      if (targetGraphic) {
        targetGraphic.ReturnToPool();
        targetGraphic = null;
      }
    } else { // else determine current hook
      Vector3 cameraForward = cameraTransform.rotation * Vector3.forward;
      float currentAngle = maxLookAngle;

      // clear the current hook if
      //  - we have not been looking at it for longer than graceTime
      //  - there are colliders between the character and the hook
      if (currentHook) {
        Vector3 hookVector = currentHook.transform.position - cameraTransform.position;
        currentAngle = Vector3.Angle(hookVector, cameraForward);
        if (currentAngle > maxLookAngle) {
          lookAwayTime += Time.deltaTime;
          if (lookAwayTime > graceLookTime)
            currentHook = null;
        } else {
          lookAwayTime = 0;
          Vector3 hookSourceVector = currentHook.transform.position - ropeEffectSource.position;
          if (Physics.Raycast(ropeEffectSource.position, hookSourceVector,
            hookSourceVector.magnitude, LayerManager.I.GroundMask))
            currentHook = null;
        }
      }

      // find all hooks potentially in range and set them to the current hook if we're looking at them and unobstructed
      var sphereCenter = cameraTransform.position + cameraTransform.rotation * Vector3.forward * (radius + minRange);
      var foundHooks = Physics.OverlapSphere(sphereCenter, radius, LayerManager.I.HookTargets);
      foreach (var collider in foundHooks) {
        var testHook = collider.GetComponent<HookPoint>();
        if (testHook) {
          Vector3 hookVector = testHook.transform.position - cameraTransform.position;
          float testAngle = Vector3.Angle(hookVector, cameraForward);
          if (testAngle < currentAngle) {
            Vector3 hookSourceVector = testHook.transform.position - ropeEffectSource.position;
            if (!Physics.Raycast(ropeEffectSource.position, hookSourceVector,
              hookSourceVector.magnitude, LayerManager.I.GroundMask)) {
              currentAngle = testAngle;
              currentHook = testHook;
              lookAwayTime = 0;
            }
          }
        }
      }

      // create/move/remove the target graphic
      if (currentHook) {
        if (!targetGraphic)
          targetGraphic = targetGraphicPrefab.CreatePooledInstance<UI.UIGfxWorldObject>(GameManager.I.UIGfxCanvas);
        targetGraphic.SetTracking(currentHook.transform, GameManager.I.BaseCamera);
      } else if (targetGraphic) {
        targetGraphic.ReturnToPool();
        targetGraphic = null;
      }
    }
  }

  // set the positions of the rope effect before the next frame
  void LateUpdate() {
    if (ropeEffect != null && currentHook != null) {
      // update the rope points
      Vector3 end = ropeEffect.UpdatePositions(ropeEffectSource.position, currentHook.transform.position);

      // move the hook hand object to the end of the rope, and turn towards the hook
      armHookTransform.position = end;
      Vector3 targetDirection = currentHook.transform.position - armHookTransform.position;
      Vector3 newDirection = Vector3.RotateTowards(armHookTransform.forward, targetDirection, 0.5f, 0);
      armHookTransform.rotation = Quaternion.LookRotation(newDirection);

      // set the IK target transform for the left arm
      //  its max distance from the shoulder is armTargetShoulderDistance
      //  don't allow it's X position in local space to be more than armTargetMaxX,
      //   in order to stop the arm from clipping the body
      Vector3 hookVector = end - ropeEffectSource.position;
      Vector3 localDirection = armTarget.parent.InverseTransformDirection(hookVector);
      Vector3 localPosition = Mathf.Min(hookVector.magnitude, armTargetMaxDistance) * localDirection;
      localPosition.x = Mathf.Min(armTargetMaxX, localPosition.x);
      armTarget.localPosition = localPosition;
    }
  }

  void FixedUpdate() {
    if (!Grappling || currentHook == null) {
      ClearRopeEffect();
      return;
    }

    Vector3 hookVector = currentHook.transform.position - ropeEffectSource.position;

    // obstacles break rope - don't think this mechanic is fun
    // if (Physics.Raycast(ropeEffectSource.position, hookVector,
    //         hookVector.magnitude, LayerManager.I.GroundMask)) {
    //   obstacleTime += Time.fixedDeltaTime;
    //   if (obstacleTime > obstacleGraceTime) {
    //     StopGrapple();
    //     return;
    //   }
    // }

    if (!Latched) {
      latchTime += Time.fixedDeltaTime;
      if (latchTime >= latchTotalTime) {
        Latched = true;
        latchStartSpeed = sourceRigidbody.velocity.magnitude;
        if (hookLatch != null)
          hookLatch.Play(transform.position);
      }
    } else {
      if (hookRigidbody == null) { // attached to a static hook object

        float dp = Vector3.Dot(sourceRigidbody.velocity, hookVector);
        if (dp < 0) { // if the vectors are facing different directions, limit the velocity moving away from the rope
          Vector3 rejection = sourceRigidbody.velocity - (dp / Vector3.Dot(hookVector, hookVector)) * hookVector;
          Vector3 targetVelocity = 1.005f * sourceRigidbody.velocity.magnitude * rejection.normalized;
          targetVelocity = Mathf.Min(Mathf.Max(swingMaxSpeed, latchStartSpeed), targetVelocity.magnitude) * targetVelocity.normalized;
          Vector3 velocityChange = targetVelocity - sourceRigidbody.velocity;
          sourceRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        if (reelIn) {
          Vector3 aboveHook = currentHook.transform.position + 1 * currentHook.transform.up;
          Vector3 pullVector = aboveHook - ropeEffectSource.position;
          Vector3 velocityChange = (Mathf.Max(reelSpeed, sourceRigidbody.velocity.magnitude) * pullVector.normalized) - sourceRigidbody.velocity;
          velocityChange.y = Mathf.Max(0, velocityChange.y);
          velocityChange = Mathf.Min(reelAccel, velocityChange.magnitude) * velocityChange.normalized;
          sourceRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        if ((ropeEffectSource.position - currentHook.transform.position).sqrMagnitude < 2 * 2)
          StopGrapple();

      } else { // attached to a dynamic hook object

      }
    }
  }

  public bool CanGrapple() {
    return currentHook != null;
  }

  public void StartGrapple() {
    if (currentHook == null || Grappling)
      return;

    Grappling = true;
    Latched = false;
    hookRigidbody = currentHook.rigidbody;
    latchTotalTime = (currentHook.transform.position - ropeEffectSource.position).magnitude / latchSpeed;
    latchTime = 0;
    //obstacleTime = 0;
    reelIn = false;
    normalizedTilt = Vector3.zero;

    armHookTransform.SetParent(null, true);

    ClearRopeEffect();
    ropeEffect = Instantiate(ropeEffectPrefab, ropeEffectSource.position, Quaternion.identity, ropeEffectSource);
    ropeEffect.SetTravelTime(latchTotalTime);

    if (hookLaunch != null)
      hookLaunch.Play(transform.position);
  }

  public void StopGrapple() {
    Grappling = false;
    Latched = false;
    currentHook = null;
    armHookTransform.SetParent(ropeEffectSource);
    armHookTransform.localPosition = Vector3.zero;
    armHookTransform.localRotation = Quaternion.identity;
    armTarget.localPosition = armTargetBasePosition;
    ClearRopeEffect();
  }

  public void SetCurrentTilt(Vector3 normalizedTilt) {
    this.normalizedTilt = normalizedTilt;
  }

  public void SetReelIn(bool reelIn) {
    this.reelIn = reelIn;
  }

  private void ClearRopeEffect() {
    if (ropeEffect) {
      Destroy(ropeEffect.gameObject);
      ropeEffect = null;
    }
  }

  void OnDrawGizmos() {
    if (cameraTransform != null) {
      Gizmos.color = Color.cyan;
      var sphereCenter = cameraTransform.position + cameraTransform.rotation * Vector3.forward * (radius + minRange);
      Gizmos.DrawWireSphere(sphereCenter, radius);
    }
  }

}
