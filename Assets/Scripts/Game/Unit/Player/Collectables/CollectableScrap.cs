using System.Collections.Generic;
using UnityEngine;
using Utility;

public class CollectableScrap : MonoBehaviour {
  public new Rigidbody rigidbody = null;
  public float acceleration = 0.5f;
  public float maxSpeed = 20;
  public float spreadTime = 3;
  private Transform target;
  private float spreadTimeCounter = 0;

  void Start() {
    target = LevelTracker.I.player.transform;
  }

  void FixedUpdate() {
    if (target == null) {
      return;
    }

    if (spreadTimeCounter < spreadTime) {
      spreadTimeCounter += Time.fixedDeltaTime;
      if (spreadTimeCounter >= spreadTime)
        gameObject.layer = LayerManager.I.CollectableMask.ToLayer();
    }

    float accelerationFactor = Mathf.Pow(Mathf.Max(1, spreadTimeCounter / spreadTime), 2);
    var targetVector = target.position - rigidbody.position;
    var targetVelocity = targetVector.normalized * maxSpeed;
    var velocityChange = targetVelocity - rigidbody.velocity;
    velocityChange = Mathf.Min(velocityChange.magnitude, accelerationFactor * acceleration) * velocityChange.normalized;
    rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
  }
}

public class SpawnScrapEffect : TakeHitTargetEffect {
  public List<CollectableScrap> scrapPrefabs = new List<CollectableScrap>();
  public float randomVelocity = 10;

  public override void HitTaken(HitTarget target, HitTrigger trigger) {
    if (target.HP <= 0) {
      var scrap = GameObject.Instantiate(scrapPrefabs.PickRandom<CollectableScrap>(), target.transform.position, Random.rotation);
      scrap.rigidbody.AddForce(scrap.rigidbody.velocity + Random.onUnitSphere * randomVelocity, ForceMode.VelocityChange);
      scrap.rigidbody.AddTorque(3 * Random.onUnitSphere, ForceMode.VelocityChange);
    }
  }
}

public class CollectScrapEffect : HitTargetEffect {
  public int scrap = 1;
  public SoundClip pickupSound = null;

  public override void Hit(HitTrigger trigger, HitTarget target) {
    var scrapComponent = trigger.GetComponent<CollectableScrap>();
    if (scrapComponent == null)
      trigger.Destroy();

    var playerMotor = target.GetComponent<PlayerMotor>();
    if (playerMotor != null) {
      playerMotor.scrap.Value += scrap;
      playerMotor.AddSteam(1);
      if (pickupSound != null)
        pickupSound.Play(target.transform.position);
      trigger.Destroy();
    }
  }
}

public class RefillSteamEffect : TakeHitTargetEffect {

  public override void HitTaken(HitTarget target, HitTrigger trigger) {
    if (trigger.source != null) {
      var playerMotor = trigger.source.GetComponent<PlayerMotor>();
      if (playerMotor != null) {
        playerMotor.FillSteam(2);
      }
    }
  }
}