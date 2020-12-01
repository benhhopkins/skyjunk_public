using UnityEngine;

public class LayerManager : MonoBehaviour {

  private static LayerManager _instance;
  public static LayerManager I => _instance;
  void Awake() {
    _instance = this;
  }

  [SerializeField] private LayerMask walkableMask = new LayerMask();
  public LayerMask WalkableMask => walkableMask;

  [SerializeField] private LayerMask groundMask = new LayerMask();
  public LayerMask GroundMask => groundMask;

  [SerializeField] private LayerMask hookTargets = new LayerMask();
  public LayerMask HookTargets => hookTargets;

  [SerializeField] private LayerMask effectMask = new LayerMask();
  public LayerMask EffectMask => effectMask;

  [SerializeField] private LayerMask collectableMask = new LayerMask();
  public LayerMask CollectableMask => collectableMask;

}

public static class LayerExtensions {
  public static int ToLayer(this LayerMask mask) => (int)Mathf.Log(mask.value, 2);
  public static bool Contains(this LayerMask mask, int layer) => ((1 << layer) & mask) != 0;
  public static bool Excludes(this LayerMask mask, int layer) => !Contains(mask, layer);
}