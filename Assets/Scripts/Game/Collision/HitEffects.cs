using System.Collections.Generic;
using UnityEngine;

public class HitTargetEffect {
  public virtual void Hit(HitTrigger trigger, HitTarget target) { }
}

public class HitRigidbodyEffect {
  public virtual void Hit(HitTrigger trigger, Rigidbody rigidbody) { }
}

public class DamageEffect : HitTargetEffect {

  public int damage = 1;

  public override void Hit(HitTrigger trigger, HitTarget target) {
    target.TakeDamage(damage, trigger);
  }
}

public class BlowbackEffect : HitRigidbodyEffect {

  public float velocityChange = 10;
  public float upwardsModifier = 5;

  public override void Hit(HitTrigger trigger, Rigidbody target) {
    target.AddForce(trigger.transform.forward * velocityChange +
      Vector3.up * upwardsModifier + Random.onUnitSphere * 5, ForceMode.VelocityChange);
  }
}

public class BounceSourceEffect : HitTargetEffect {

  public override void Hit(HitTrigger trigger, HitTarget target) {
    if (trigger.source != null) {
      var rigidbody = trigger.source.GetComponent<Rigidbody>();
      if (rigidbody != null) {
        var bouncedVelocity = rigidbody.velocity;
        bouncedVelocity.y = Mathf.Abs(bouncedVelocity.y) + bouncedVelocity.magnitude / 3;
        rigidbody.velocity = bouncedVelocity;
      }
    }
  }
}