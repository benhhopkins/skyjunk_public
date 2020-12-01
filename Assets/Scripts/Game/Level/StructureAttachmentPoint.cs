using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureAttachmentPoint : MonoBehaviour {
  // These fields determine compatibility, "what can go there".
  public AttachmentDirection Direction;
  public AttachmentLocation Location;
  public AttachmentKind Kind;
  public List<Transform> AdditionalCoveredTransforms; // Do we overlap over transforms?
  public List<Transform> KilledTransforms;
  public float Size;
  // Start is called before the first frame update
  void Start() {

  }

  AttachmentDirection ReverseDirection(AttachmentDirection direction) {
    if (direction == AttachmentDirection.InAttach) {
      return AttachmentDirection.OutAttach;
    }
    return AttachmentDirection.InAttach;
  }

  // Maybe a smarter way? Whatever.
  public static bool LocationsCompatible(StructureAttachmentPoint a, StructureAttachmentPoint b) {
    return (a.Location & b.Location) != 0;
  }

  public static bool KindsCompatible(StructureAttachmentPoint a, StructureAttachmentPoint b) {
    return (a.Kind & b.Kind) != 0;
  }

  // Can StructureAttachmentPoint be attached to our point?
  public bool Compatible(StructureAttachmentPoint outAttachPoint) {
    if (ReverseDirection(Direction) != outAttachPoint.Direction) {
      //Debug.Log("incompatible for direction" + this + " " + outAttachPoint);
      return false;
    }
    if (!LocationsCompatible(this, outAttachPoint)) {
      //Debug.Log("incompatible for locations" + this + " " + outAttachPoint);
      return false;
    }
    if (!KindsCompatible(this, outAttachPoint)) {
      //Debug.Log("incompatible for kinds" + this + " " + outAttachPoint);
      return false;
    }
    if (Size > outAttachPoint.Size) {
      //Debug.Log("incompatible for size" + this + " " + outAttachPoint + " " + this.Size + " " + outAttachPoint.Size);
      return false;
    }
    //Debug.Log("compatible!");
    return true;
  }

  // Are all the transforms we need to cover available?
  public bool TransformsAvailable(HashSet<Transform> usedTransforms) {
    if (usedTransforms.Contains(transform)) {
      return false;
    }
    if (AdditionalCoveredTransforms != null) {
      foreach (Transform t in AdditionalCoveredTransforms) {
        if (usedTransforms.Contains(t)) {
          return false;
        }
      }
    }
    return true;
  }

  // Use the transforms we need, and kill those we also kill.
  public void ConsumeTransforms(HashSet<Transform> usedTransforms) {
    Debug.Assert(TransformsAvailable(usedTransforms));
    usedTransforms.Add(transform);
    if (AdditionalCoveredTransforms != null) {
      foreach (Transform t in AdditionalCoveredTransforms) {
        usedTransforms.Add(t);
      }
    }
    if (KilledTransforms != null) {
      foreach (Transform t in KilledTransforms) {
        usedTransforms.Add(t);
      }
    }
  }

  // Update is called once per frame
  void Update() {

  }
}
