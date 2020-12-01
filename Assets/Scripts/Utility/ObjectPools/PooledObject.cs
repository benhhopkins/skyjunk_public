using UnityEngine;

namespace Utility {
  public class PooledObject : MonoBehaviour {
    public ObjectPool Pool { get; set; }

    [System.NonSerialized]
    ObjectPool poolInstanceForPrefab;

    protected T GetPooledInstance<T>(Transform parent = null, bool rectTransform = false) where T : PooledObject {
      if (!poolInstanceForPrefab) {
        poolInstanceForPrefab = ObjectPool.GetPool(this, rectTransform);
      }
      return (T)poolInstanceForPrefab.GetObject(parent);
    }

    public void ReturnToPool() {
      ReturnedToPool();
      if (Pool) {
        Pool.ReturnToPool(this);
      } else {
        Destroy(gameObject);
      }
    }

    protected virtual void ReturnedToPool() { }
  }
}