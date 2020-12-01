using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Utility;

public class HitTrigger : SerializedMonoBehaviour {
  public float lifetime = 1;
  public int hits = 1;
  public int groundHits = 1;
  public bool triggerEffectsOnEachHit = false;

  public List<HitTargetEffect> hitTargetEffects = new List<HitTargetEffect>();
  public List<HitRigidbodyEffect> hitRigidbodyEffects = new List<HitRigidbodyEffect>();
  private int hitCount = 0;
  public int HitCount => hitCount;

  public List<Renderer> renderers = new List<Renderer>();

  protected HashSet<GameObject> alreadyHit = new HashSet<GameObject>();

  public SoundClip shootSound;
  public SoundClip hitSound;

  [System.NonSerialized] public Unit source;

  void Start() {
    if (shootSound != null && shootSound.loop)
      shootSound.Play(transform.position, gameObject);
    else
      shootSound.Play(transform.position);
  }

  protected virtual void OnTriggerEnter(Collider collider) {
    TestCollision(collider.gameObject);
  }

  protected virtual void OnCollisionEnter(Collision collision) {
    TestCollision(collision.gameObject);
  }

  protected virtual void OnTriggerStay(Collider collider) {
    TestCollision(collider.gameObject);
  }

  protected void TestCollision(GameObject gameObject) {
    if (!enabled)
      return;

    if (LayerManager.I.GroundMask.Contains(gameObject.layer)) {
      if (groundHits > 0) {
        groundHits--;
        if (groundHits == 0)
          Destroy();
      }
    } else {
      if (alreadyHit.Contains(gameObject))
        return;
      alreadyHit.Add(gameObject);
      hitCount++;

      var hitTarget = gameObject.GetComponent<HitTarget>();
      var hitRigidbody = gameObject.GetComponent<Rigidbody>();
      if (hitTarget != null || hitRigidbody != null)
        HitObject(hitTarget, hitRigidbody);

      if (hits > 0 && hitCount >= hits)
        Destroy();
    }
  }

  void Update() {
    if (lifetime > 0) {
      lifetime -= Time.deltaTime;
      if (lifetime <= 0)
        Destroy(gameObject);
    }
  }

  private void HitObject(HitTarget hitTarget, Rigidbody hitRigidbody) {
    hitSound.Play(transform.position);

    if (triggerEffectsOnEachHit || hitCount >= hits) {

      if (hitTarget != null) {
        foreach (var effect in hitTargetEffects) {
          effect.Hit(this, hitTarget);
          hitTarget.HitEffectTaken(this, effect);
        }
        hitTarget.HitTaken(this);
      }

      if (hitRigidbody != null)
        foreach (var effect in hitRigidbodyEffects)
          effect.Hit(this, hitRigidbody);
    }
  }

  public void Destroy() {
    Destroy(gameObject);

    // enabled = false;
    // gameObject.layer = LayerManager.I.EffectMask;

    // foreach (var renderer in renderers)
    //   renderer.material.SetColor("_MultColor", new Color(.6f, .6f, .8f));
  }
}