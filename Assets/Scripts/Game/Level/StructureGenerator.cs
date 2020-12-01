using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using Sirenix.OdinInspector; // for "Button"

using System.Linq; // is this a horrible mistake?

public class StructureGenerator : MonoBehaviour {
  // This is our root list of *everything*!
  // From core platforms to doodads to hooks
  public List<LevelStructure> Components;
  public float DensityValue = 0.5f;

  // Start is called before the first frame update
  // Is this the right place to compute some constant?
  void Start() {

  }

  private LevelStructure RandomKind(AttachmentKind kind) {
    var validComponents = Components.Where(structure => (structure.HasAttachmentKindAndDirection(kind, AttachmentDirection.InAttach)));
    int count = validComponents.Count();
    Debug.Assert(count > 0);
    return validComponents.ElementAt(UnityEngine.Random.Range(0, count));
  }

  // Get a random structure that is valid for this attach outPoint.
  // Note that every structure has a "Kind", which is what we test here, but
  // the actual location is an attach outPoint on that structure.
  private LevelStructure RandomCompatibleStructure(StructureAttachmentPoint outPoint) {
    var validComponents = Components.Where(structure => structure.HasCompatibleInAttachment(outPoint));
    int count = validComponents.Count();
    if (count <= 0) {
      Debug.Log(outPoint + " " + outPoint.Kind);
    }
    Debug.Assert(count > 0);
    return validComponents.ElementAt(UnityEngine.Random.Range(0, count));
  }

  public LevelStructure CurrentGenerated = null;

  public LevelStructure DefaultStart = null;

  public LevelStructure Generate(float weight, StructureAttachmentPoint outPoint) {
    if (UnityEngine.Random.Range(0f, 0.99f) > weight) {
      return null;
    }

    Debug.Log("Generating Structure");

    LevelStructure root = null;
    if (outPoint == null) {
      // Base case:
      root = RandomKind(AttachmentKind.Platform).GenerateClone();
      Debug.Log("Base case, got " + root);
    } else {

      // Recursive case: create a structure compatible with that outPoint
      Debug.Log("Recursive case, looking for compatible with " + outPoint.Kind);
      root = RandomCompatibleStructure(outPoint).GenerateClone();
      Debug.Log("Recursive case, got " + root);

      // And on that new structure, mark the outPoint we're using.
      StructureAttachmentPoint usedPoint = root.RandomCompatiblePoint(outPoint);
      usedPoint.ConsumeTransforms(root.UsedTransforms);

      Debug.Log("Done marking used transforms: " + root.UsedTransforms.Count);

      // Because this is the time where we know (physically) where
      // we belong, we'll also place ourselves in space.

      Debug.Assert(outPoint != null);
      Debug.Assert(usedPoint != null);
      Debug.Assert(root != null);

      // We're gluing together "usedPoint" and "outPoint".
      // "outPoint" is (presumably) the point on our parent
      // "usedPoint" is the point on our structure "root".


      /*
            Vector3 originalOffset = usedPoint.transform.position;
            var originalRotation1 = outPoint.transform.localRotation;
            var originalRotation2 = usedPoint.transform.localRotation;
            //root.transform.localRotation = outPoint.transform.localRotation * usedPoint.transform.localRotation;
            //root.transform.localPosition += root.transform.localRotation * originalOffset;
            root.transform.parent = outPoint.transform;
            root.transform.localPosition = new Vector3(0, 0, 0);
            root.transform.localRotation = originalRotation2;
            Vector3 slideAmount = usedPoint.transform.position - root.transform.position;
            root.transform.position -= slideAmount;
            */

      // this is close, but not quite.
      root.transform.parent = outPoint.transform;
      root.transform.localRotation = usedPoint.transform.localRotation;
      root.transform.position -= (usedPoint.transform.position - outPoint.transform.position);

      // We're gluing together "usedPoint" and "outPoint".
      // "outPoint" is (presumably) the point on our parent
      // "usedPoint" is the point on our structure "root".
      //root.transform.localRotation = usedPoint.transform.localRotation;


      // We adjust by (where the root expects to be connected) and (where the parent expects the root to be connected)

      // TODO: Did we just induce a collision with the parent?
      // Maybe need to look "up" the whole tree.
      // If so, just bail and do nothing.

      if (root.structurePerturbations.HasFlag(StructurePerturbations.SmallSlide)) {
        root.transform.position += new Vector3((UnityEngine.Random.value - 0.5f) * 4, 0, (UnityEngine.Random.value - 0.5f) * 4);
        //root.transform.lposition.x += UnityEngine.Random.value- 0.5f;
        //root.transform.position.y += UnityEngine.Random.value- 0.5f;
      }
    }

    // root is the structure we've just created.
    var renderer = root.GetComponent<Renderer>();
    if (renderer != null) {
      //renderer.material = LevelGenerator.I.Materials.ChooseRandom();
    }

    // Generate sub-structures if possible
    weight *= DensityValue;

    foreach (StructureAttachmentPoint subPoint in root.GetComponentsInChildren<StructureAttachmentPoint>()) {
      if (subPoint.Direction == AttachmentDirection.OutAttach) {

        // Is every transform is uses available?
        if (!subPoint.TransformsAvailable(root.UsedTransforms)) {
          continue;
        }

        // OK, great:
        LevelStructure newStructure = Generate(weight, subPoint);
        if (newStructure != null) {
          subPoint.ConsumeTransforms(root.UsedTransforms);
        }

      }
    }

    return root;
  }

  [Button("Test Generate!")]
  public void TestGenerate() {
    if (CurrentGenerated) {
      DestroyImmediate(CurrentGenerated.gameObject);
    }
    // TODO: 1.0 should be "InitialGenWeight"
    CurrentGenerated = Generate(1.0f, null);
    CurrentGenerated.transform.position = new Vector3(0, 0, 0); // place at origin
    CurrentGenerated.transform.parent = transform;
  }

}
