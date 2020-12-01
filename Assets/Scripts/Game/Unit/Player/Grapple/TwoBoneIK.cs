using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEngine.Experimental.Animations;

public class TwoBoneIK : MonoBehaviour {

  public Transform targetTransform;

  public Transform endJoint;
  public Animator animator;

  private PlayableGraph graph;
  private AnimationScriptPlayable playable;

  void OnEnable() {
    Transform midJoint = endJoint.parent;
    if (midJoint == null)
      return;
    Transform topJoint = midJoint.parent;
    if (topJoint == null)
      return;

    graph = PlayableGraph.Create("TwoBoneIK");
    graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
    var output = AnimationPlayableOutput.Create(graph, "ouput", animator);

    //animator.fireEvents = false;

    var twoBoneIKJob = new TwoBoneIKJob();
    twoBoneIKJob.Setup(animator, topJoint, midJoint, endJoint, targetTransform);

    playable = AnimationScriptPlayable.Create(graph, twoBoneIKJob);
    //m_LookAtPlayable.AddInput(AnimationClipPlayable.Create(m_Graph, idleClip), 0, 1.0f);

    output.SetSourcePlayable(playable);
    graph.Play();
  }

  void OnDisable() {
    graph.Destroy();
  }
}
