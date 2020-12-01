using UnityEngine;
using Utility;

public class FootstepPlayer : StateMachineBehaviour {

  public SoundClip sound = null;
  private bool played1 = false;
  private bool played2 = false;

  public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    float fracPart = stateInfo.normalizedTime - (float)System.Math.Truncate(stateInfo.normalizedTime);
    if (!played1 && fracPart > 0.4f) {
      played1 = true;
      sound.Play(animator.transform.position);
    } else if (played1 && fracPart < 0.4f)
      played1 = false;

    if (!played2 && fracPart > .9f) {
      played2 = true;
      sound.Play(animator.transform.position);
    } else if (played2 && fracPart < .9f)
      played2 = false;
  }

}