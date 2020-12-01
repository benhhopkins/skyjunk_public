using UnityEngine;

public class ObjectAnimator : MonoBehaviour {

  [SerializeField] protected Animator animator = null;
  [SerializeField] protected string defaultAnimationName = "Idle";

  private string currentClip = "";
  private string nextClip = "Idle";
  private float clipTime = 0;
  private float fadeTime = 0.1f;

  protected static readonly int idAnimScale = Animator.StringToHash("AnimScale");

  private void Update() {
    UpdateAnimationVariables();

    // we are playing a special animation if clipTime > 0
    if (clipTime > 0) {
      UpdateClip();
      clipTime -= Time.deltaTime;
    } else { // play the appropriate object animation if there is no special clip
      UpdateDefaultAnimations();
      UpdateClip();
    }
  }

  public void PlayAnimation(string clipName, float clipTime, float fadeTime = 0.1f) {
    nextClip = clipName;
    this.clipTime = clipTime;
    this.fadeTime = fadeTime;
  }

  // override this to set variables on the animator (e.g. move speed, grounded, etc.)
  protected virtual void UpdateAnimationVariables() { }

  // override this to play default animations when no clip is playing
  protected virtual void UpdateDefaultAnimations() {
    PlayAnimation(defaultAnimationName, 0);
  }

  private void UpdateClip() {
    if (nextClip != currentClip) {
      currentClip = nextClip;

      if (clipTime > 0)
        animator.SetFloat(idAnimScale, 1 / clipTime);
      else
        animator.SetFloat(idAnimScale, 1);

      animator.CrossFadeInFixedTime(nextClip, fadeTime);
    }
  }

}