using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "SkyJunk/AttackStateDefinition")]
public class AttackStateDefinition : SerializedScriptableObject {
  [SerializeField] private AttackState attackState = null;

  public AttackState Create() {
    var clone = Instantiate(this);
    return clone.attackState;
  }
}

public class AttackState {
  public string animationName = "Idle";
  public float totalTime = 0.5f;
  public float blendTime = 0.1f;
  public float hitTime = -1;
  public float cancelTime = -1;
  public bool restrictMovement = false;
  public bool restrictRotation = false;

  public AttackState nextState = null;

  protected Unit unit;

  public virtual void Enter(Unit unit) {
    this.unit = unit;
  }
  public virtual void End() { }
  public virtual void Update(float progress) { }
  public virtual void FixedUpdate(float progress) { }
  public virtual void Hit() { }
}