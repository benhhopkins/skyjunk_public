using System.Collections.Generic;
using UnityEngine;

namespace Utility {
  public class ObjectPool : MonoBehaviour {
    private PooledObject m_prefab;
    private Queue<PooledObject> m_objects = new Queue<PooledObject>();

    public static ObjectPool GetPool(PooledObject prefab, bool rectTransform) {
      GameObject obj;
      ObjectPool pool;
      obj = new GameObject(prefab.name + " Pool");
      pool = obj.AddComponent<ObjectPool>();
      pool.m_prefab = prefab;

      if (rectTransform)
        obj.transform.SetParent(ObjectPoolRectParent.I.transform);
      else
        obj.transform.SetParent(ObjectPoolParent.I.transform);


      return pool;
    }

    public PooledObject GetObject(Transform objectParent = null) {
      PooledObject obj;
      if (m_objects.Count > 0) {
        obj = m_objects.Dequeue();
        obj.gameObject.SetActive(true);
      } else {
        obj = Instantiate<PooledObject>(m_prefab);
        obj.Pool = this;
      }
      if (objectParent)
        obj.transform.SetParent(objectParent, false);
      else
        obj.transform.SetParent(transform, false);
      return obj;
    }

    public void ReturnToPool(PooledObject obj) {
      if (obj.gameObject.activeSelf) {
        obj.gameObject.SetActive(false);
        obj.gameObject.transform.SetParent(transform);
        m_objects.Enqueue(obj);
      }
    }
  }
}