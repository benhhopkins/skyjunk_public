using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Utility;
using System;

using System.Linq; // is this a horrible mistake?

// Sort of subclassing... is this an attachment that allows us
// to put things on it, or an attachment we'd consider as we 
public enum AttachmentDirection {
  InAttach, // this is a point where we can connect our structure
  OutAttach, // this is a point where we'd like to attach a substructure
}

[Flags]
public enum AttachmentKind {
  None = 0, // Designed to induce an error
  Hook = 1,
  Connector = 2, // like a pipe or long platform. We could count "Hook" as a connector, but that's important enough to distinguish

  Doodad = 4, // destructable?
  Platform = 8, // This is what can start a sequence. If this isn't specified, we can't use it as a starting platform.
  FlatPlatform = 16,
  RoundPlatform = 32,
  BoxPlatform = 64,
  Enemy = 128,
}


[Flags]
public enum AttachmentLocation {
  None = 0, // designed to induce error
  Up = 1,
  Down = 2,
  North = 4,
  East = 8,
  South = 16,
  West = 32,
  Side = North | East | South | West,
  Shiftable = 64,
}

[Flags]
public enum StructurePerturbations {
  None = 0,

  // These come up for the global structure
  RotateOverY = 1, // like freighters or square platforms
  RotateSpherical = 2,

  // These come up as we're building things incrementally.
  // Doodads, for instance, can often be placed with a slight 2d perturbation.
  SmallSlide = 4,
}


public class LevelStructure : SerializedMonoBehaviour {
  // What transforms on us are used by connections?
  // (This is to avoid overlapping structures)
  public HashSet<Transform> UsedTransforms = new HashSet<Transform>();
  public int connectionCount = 0;
  public StructurePerturbations structurePerturbations;

  // This is mainly used to find the "root" objects we can start with
  public bool HasAttachmentKindAndDirection(AttachmentKind kind, AttachmentDirection direction) {
    foreach (StructureAttachmentPoint inAttachPoint in this.GetComponentsInChildren<StructureAttachmentPoint>()) {
      if (((inAttachPoint.Kind & kind) != 0) && (inAttachPoint.Direction == direction)) {
        return true;
      }
    }
    return false;
  }

  public bool HasCompatibleInAttachment(StructureAttachmentPoint outAttachPoint) {
    Debug.Assert(outAttachPoint.Direction.HasFlag(AttachmentDirection.OutAttach));
    foreach (StructureAttachmentPoint inAttachPoint in this.GetComponentsInChildren<StructureAttachmentPoint>()) {
      if (inAttachPoint.Compatible(outAttachPoint)) {
        return true;
      }
    }
    return false;
  }

  public StructureAttachmentPoint RandomCompatiblePoint(StructureAttachmentPoint outPoint) {
    var validPoints = GetComponentsInChildren<StructureAttachmentPoint>().Where(subPoint => subPoint.Compatible(outPoint));
    int count = validPoints.Count();
    Debug.Assert(count > 0);
    return validPoints.ElementAt(UnityEngine.Random.Range(0, count));
  }

  public LevelStructure GenerateClone() {
    var newStructure = Instantiate(this);

    // This should be done by Instantiate?
    newStructure.UsedTransforms = new HashSet<Transform>();

    return newStructure;
  }
}


/*
// Place the structure in the object hierarchy
toAdd.transform.parent = newStructure.transform;

// Apply the rotation and translation as needed;
// place the structure in the physical space

// There is something incorrect about this, or how the transforms are encoded.
// In "PipeLarge" the inertion point is pointed inwards, in "walkway small" it's pointed
// outwards. Yet, both work on "Pressure Sphere" and "Hab Cylinder 1". Maybe I have some
// weird sign error?
toAdd.transform.rotation *= attach.transform.rotation;
if (toAdd.insertPoint != null) {
  toAdd.transform.localRotation = toAdd.insertPoint.rotation;
}

if (toAdd.insertPoint != null) {
  toAdd.transform.position = attach.transform.position - toAdd.insertPoint.transform.position;
} else {
  toAdd.transform.position = attach.transform.position;
}

// Just to help debugging.
toAdd.usedInsertPoint = attach.transform;
    }

    return newStructure;
  }

*/