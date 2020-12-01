using System.Collections.Generic;
using UnityEngine;
using Utility;

public class ContainerGenerator : MonoBehaviour {
  public List<GameObject> containers = new List<GameObject>();

  void Start() {
    int amount = Random.Range(1, 5);
    for (int i = 0; i < amount; i++)
      Instantiate(containers.PickRandom(), transform.position +
        new Vector3(Random.Range(-2f, 2f), Random.Range(0f, 2f), Random.Range(-2f, 2f)),
        Random.rotation, transform.parent);

    Destroy(gameObject);
  }
}