using UnityEngine;
using Utility;

namespace UI {
  [RequireComponent(typeof(RectTransform))]
  public class UIGfx : PooledObject {

    protected RectTransform rectTransform = null;
    protected Canvas parentCanvas = null;

    public T CreatePooledInstance<T>(Canvas parent) where T : UIGfx {
      T gfx = GetPooledInstance<T>(parent.transform, true);
      gfx.parentCanvas = parent;
      gfx.Start();
      return gfx;
    }

    void Awake() {
      rectTransform = GetComponent<RectTransform>();
    }

    public virtual void Start() {
      transform.localScale = Vector3.one;
    }
  }
}