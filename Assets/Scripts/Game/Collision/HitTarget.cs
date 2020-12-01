using System.Collections.Generic;
using UnityEngine;
using Utility;
using Sirenix.OdinInspector;

public class HitTarget : SerializedMonoBehaviour {

  [SerializeField] private int hp = 10;
  [SerializeField] private int maxHp = 10;

  public List<TakeHitTargetEffect> takeHitEffects = new List<TakeHitTargetEffect>();

  public IntVariable serializedInt = null;

  public SoundClip hurtSound = null;
  public SoundClip deathSound = null;

  public Sliceable sliceable = null;

  public void Start() {
    if (serializedInt != null)
      serializedInt.Value = hp;
  }

  public int HP {
    get {
      return hp;
    }
    set {
      hp = Mathf.Clamp(value, 0, maxHp);
      if (serializedInt != null)
        serializedInt.Value = hp;
    }
  }

  public void HitEffectTaken(HitTrigger hitTrigger, HitTargetEffect targetEffect) {
    foreach (var effect in takeHitEffects) {
      effect.HitEffectTaken(this, hitTrigger, targetEffect);
    }
  }

  public void HitTaken(HitTrigger hitTrigger) {
    foreach (var effect in takeHitEffects)
      effect.HitTaken(this, hitTrigger);
  }

  public void TakeDamage(int damage, HitTrigger hitTrigger = null) {
    HP = HP - damage;

    if (HP <= 0)
      Kill(hitTrigger);
    else if (hurtSound != null)
      hurtSound.Play(transform.position);
  }

  public void Kill(HitTrigger hitTrigger = null) {
    if (deathSound != null)
      deathSound.Play(transform.position);

    if (sliceable != null)
      sliceable.SliceAndDestroy(hitTrigger);
    else
      Destroy(gameObject);
  }

}

public class TakeHitTargetEffect {
  // called after each hit effect is processed on this target
  public virtual void HitEffectTaken(HitTarget target, HitTrigger trigger, HitTargetEffect targetEffect) { }

  // called once after all hit effects are processed
  public virtual void HitTaken(HitTarget target, HitTrigger trigger) { }
}