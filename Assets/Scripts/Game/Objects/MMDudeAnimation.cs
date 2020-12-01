using UnityEngine;

// main menu dude animation helper
public class MMDudeAnimation : MonoBehaviour {

  void Start() {
    var animator = GetComponent<Animator>();
    animator.Play("Idle");
  }
}