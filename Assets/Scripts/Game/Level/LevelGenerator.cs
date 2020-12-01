using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Utility;
using System; // pair
using System.Linq; // hashset to list

public class LevelGenerator : SerializedMonoBehaviour {
  //public List<LevelStructure> structurePrefabs = new List<LevelStructure>();
  public StructureGenerator generator;

  [SerializeField]
  private List<LevelStructure> generatedStructures = new List<LevelStructure>();

  public List<LevelStructure> ConnectionStructures;

  public List<Material> Materials = new List<Material>();

  public const float MaxConnectionDistance = 25f;
  public const float MaxConnectionRadius = 1.5f;

  [Button("Clear!")]
  public void Clear() {
    if (generatedStructures != null)
      foreach (var part in generatedStructures)
        if (part != null)
          DestroyImmediate(part.gameObject);

    generatedStructures.Clear();
    if (candidateConnectionPoints != null) {
      candidateConnectionPoints.Clear();
    }
  }

  private bool CollidesWithExistingObject(LevelStructure structure) {
    //Collider[] hitColliders = Physics.OverlapBox(structure.transform.position, structure.transform.localScale * 1.2);
    Collider myCollider = structure.GetComponent<Collider>();
    Collider[] hitColliders = Physics.OverlapBox(structure.transform.position, myCollider.bounds.extents);

    //Debug.Log("Testing bounds at point " + structure.transform.position + " with local scale " + myCollider.bounds.extents);

    int hitCount = 0;
    for (int i = 0; i < hitColliders.Length; i++) {
      Debug.Log("Hit : " + hitColliders[i].name + i);

      if (ObjectIsDescendant(structure.transform, hitColliders[i].gameObject.transform)) {
        Debug.Log("Not counting descendant collision" + hitColliders[i].name + i);
        continue;
      }

      hitCount++;
    }

    Debug.Log("Total hit : " + hitColliders.Length);
    Debug.Log("Hit count : " + hitCount);
    Debug.Log("My children: (this is getting weird): " + structure.transform.childCount);
    return hitCount > 0;
  }

  private bool AnyOverlap() {
    foreach (var structure in generatedStructures) {
      if (CollidesWithExistingObject(structure)) {
        return true;
      }
    }
    return false;
  }

  static private bool ObjectIsDescendant(Transform parent, Transform child) {
    for (Transform it = child; it != null; it = it.parent) {
      if (it == parent) return true;
    }
    return false;
  }

  bool RadiusHitsOtherStructure(LevelStructure structure, float radius) {
    // TODO: Can we just make this a constant somewhere?
    Collider[] hitColliders = new Collider[structure.transform.childCount + 2];
    //Debug.Log("RadiusHitsOtherStructures Start: " + hitColliders.Length + " radius: " + radius);

    // How many things do we hit?
    int overlapCount = Physics.OverlapSphereNonAlloc(structure.transform.position, radius, hitColliders);
    //Debug.Log("RadiusHitsOtherStructures: " + overlapCount + structure);

    // Filter out ourselves
    foreach (Collider c in hitColliders) {
      if (c == null) continue;
      if (ObjectIsDescendant(structure.transform, c.gameObject.transform)) {
        overlapCount--;
        //Debug.Log("Decrement Child: " + overlapCount + structure);
      } else {
        //Debug.Log("New object:" + c.gameObject + " " + c.gameObject.transform);
      }
    }

    //Debug.Log("RadiusHitsOtherStructures: " + overlapCount + structure);

    return overlapCount > 0;
  }

  // Find the ballon from this center until it hits another object...
  float CenteredBalloonRadius(LevelStructure structure, int desiredCount = 1) {
    Vector3 center = structure.transform.position;
    float smallestOneHit = 1;
    float smallestTwoHit = 1000000;
    float eps = 1;

    float radius = 1;

    Collider[] hitColliders = new Collider[desiredCount + 2];

    int bailCount = 1000;

    while (!RadiusHitsOtherStructure(structure, radius)) {
      smallestOneHit = radius;
      radius *= 2;

      if (bailCount-- < 1) {
        Debug.Log("BAILING IN CENTERED 1!\n");
        break;
      }
    }

    // OK, we just found the first radius where we hit something.
    smallestTwoHit = radius;

    float experiment = (smallestTwoHit + smallestOneHit) / 2;

    // avoid infinite loop in weird... concave... cases? Not sure when this is true, exactly.
    while (smallestTwoHit > (smallestOneHit + eps)) {

      if (bailCount-- < 1) {
        Debug.Log("BAILING IN CENTERED 2!\n");
        break;
      }

      experiment = (smallestTwoHit + smallestOneHit) / 2;

      if (!RadiusHitsOtherStructure(structure, experiment)) {
        smallestOneHit = experiment;
      } else {
        smallestTwoHit = experiment;
      }
    }

    return experiment;
  }

  public int NumStructures = 1;

  // Why hashset to list? I think I'm not iterating through these properly, and so
  // a lot of redundant points are added. This is the quick-and-easy way of making
  // sure we're not creating multiple copies.
  private List<Transform> AllConnectionPoints(List<LevelStructure> allStructures) {
    HashSet<Transform> result = new HashSet<Transform>();
    foreach (LevelStructure structure in allStructures) {

      foreach (LevelStructure component in GetComponentsInChildren<LevelStructure>(structure)) {
        foreach (StructureAttachmentPoint subPoint in component.GetComponentsInChildren<StructureAttachmentPoint>()) {

          // Is that attach point already used?
          if (component.UsedTransforms == null) {
            Debug.Log("ERROR IN CASE: " + component);
            component.UsedTransforms = new HashSet<Transform>();
          }
          if (!subPoint.TransformsAvailable(component.UsedTransforms)) {
            continue;
          }
          if (subPoint.Kind.HasFlag(AttachmentKind.Connector) && subPoint.Direction == AttachmentDirection.OutAttach) {
            result.Add(subPoint.transform);
          }
        }
      }
    }
    return result.ToList();
  }


  public void GenerateConnections() {

    // Let's randomize the order... there seems to be some persistence in the RNG
    // that is probably something my fault (...).
    // This is terrible.
    // candidateConnectionPoints = candidateConnectionPoints.OrderBy(x => UnityEngine.Random.value).ToList();


    foreach (Tuple<Transform, Transform> pair in candidateConnectionPoints) {
      Transform from = pair.Item1;
      Transform to = pair.Item2;
      Vector3 direction = to.position - from.position;

      //Debug.Log("DEBUGGING ANGLE: " + Vector3.Angle(from.position, to.position));

      // Flip a coin or whatever.
      float coinFlip = UnityEngine.Random.value;
      if (coinFlip < 0.60f) {
        continue;
      }


      // Have either of these points since been used?
      if (to.GetComponentInParent<LevelStructure>().UsedTransforms.Contains(to)) {
        continue;
      }
      if (from.GetComponentInParent<LevelStructure>().UsedTransforms.Contains(from)) {
        continue;
      }
      // This is a quick (for me, not the computer) way of avoiding ludicrously dense paths
      if (!SphereCastConnects(from, to, MaxConnectionRadius)) {
        continue;
      }

      int oldFromCount = from.GetComponentInParent<LevelStructure>().connectionCount;
      int oldToCount = to.GetComponentInParent<LevelStructure>().connectionCount;
      if (oldFromCount > 1) { continue; }
      if (oldToCount > 1) { continue; }
      from.GetComponentInParent<LevelStructure>().connectionCount++;
      to.GetComponentInParent<LevelStructure>().connectionCount++;

      Debug.Log("Adding connection!");


      // TODO Take into account the angle -- maybe we want a pathway, maybe we want a rail
      Debug.Assert(ConnectionStructures != null);

      LevelStructure connection = ConnectionStructures.PickRandom<LevelStructure>().GenerateClone();

      Collider connectionCollider = connection.GetComponent<Collider>();
      float stretchFactor = direction.magnitude / connectionCollider.bounds.extents.z;

      // Warp it, stretch it, (bop it?)
      connection.transform.localScale = new Vector3(1, 1, stretchFactor / 2);
      connection.transform.position = (to.position + from.position) / 2;

      // Set rotation
      //connection.transform.rotation = Quaternion.FromToRotation(from.position, to.position);
      connection.transform.rotation = Quaternion.LookRotation(from.position - to.position, Vector3.forward);

      // The rotation also twists the platform. Basically I want to reset the z rotation to 0.
      // Maybe there's a nice way of not needing this?
      // Why can't we just set "eulerAngles" directly? Simply prevented by properties rules, I guess.
      // See https://docs.unity3d.com/ScriptReference/Quaternion-eulerAngles.html to show that this is the right way.
      Vector3 nextEuler = connection.transform.rotation.eulerAngles;
      nextEuler.z = 0;
      Quaternion nextRotation = new Quaternion();
      nextRotation.eulerAngles = nextEuler;
      connection.transform.rotation = nextRotation;

      // Set hierarchy plumbing
      connection.transform.parent = transform;
      generatedStructures.Add(connection);
    }

  }

  void ComputeConnectionsCandidates() {
    List<Transform> allConnectionPoints = AllConnectionPoints(generatedStructures);

    // Foreach pair (????? expensive! We can do smarter things, here.)
    candidateConnectionPoints = new List<Tuple<Transform, Transform>>();


    for (int i = 0; i < allConnectionPoints.Count; i++) {
      for (int j = i + 1; j < allConnectionPoints.Count; j++) {

        Transform fromTransform = allConnectionPoints[i].transform;
        Transform toTransform = allConnectionPoints[j].transform;

        // Are they on the same object? We only care about the immediate parent because
        // maybe we do want "intra-structure" connections

        if (fromTransform.GetComponentInParent<LevelStructure>() != null && // this should always be true, but just in case
          fromTransform.GetComponentInParent<LevelStructure>() == toTransform.GetComponentInParent<LevelStructure>()) {
          continue;
        }

        if (Vector3.Distance(fromTransform.position, toTransform.position) > MaxConnectionDistance) {
          continue; // too far!
        }

        if (Vector3.Distance(fromTransform.position, toTransform.position) < 4) {
          continue; // too close!
        }

        // Angle too high?
        Debug.Log("FILTERING WITH ANGLE: " + Math.Abs(Vector3.Angle(fromTransform.position, toTransform.position)));

        if (Math.Abs(Vector3.Angle(fromTransform.position, toTransform.position)) > 3f) {
          continue; // too steep!
        }

        // Something's in the way!
        if (!SphereCastConnects(allConnectionPoints[i], allConnectionPoints[j], MaxConnectionRadius)) {
          continue;
        }


        //Debug.Log("Saving connection point: " + "(" + i + ", " + j + "), " + allConnectionPoints[i] + ", " + allConnectionPoints[j] + ": " + allConnectionPoints[i].position + ", " + allConnectionPoints[j].position);
        Debug.Log("Saving connection point: " + allConnectionPoints[i].position + ", " + allConnectionPoints[j].position);
        candidateConnectionPoints.Add(new Tuple<Transform, Transform>(allConnectionPoints[i], allConnectionPoints[j]));

      }
    }
  }

  public float ClusterRadius = 30;
  public float ClusterXStretch = 2;

  void GenerateCluster(Vector3 center) {
    // Is this... insane?
    StructureGenerator gen = this.GetComponent(typeof(StructureGenerator)) as StructureGenerator;

    for (int i = 0; i < NumStructures; i++) {

      LevelStructure structure = gen.Generate(1.0f, null);
      gen.CurrentGenerated = null; // look at me. I'm the owner now.

      if (structure.structurePerturbations.HasFlag(StructurePerturbations.RotateSpherical)) {
        if (UnityEngine.Random.value < 0.5f) {
          // Apply a fun perturbation. Maybe this should be in the structure generation?
          // https://docs.unity3d.com/ScriptReference/Random-rotation.html just, like, purely random?
          // Ah this is nice:https://www.youtube.com/watch?v=paJPbqpQIQc&feature=emb_logo
          Vector3 perturbation = UnityEngine.Random.insideUnitSphere; // between -1 and 1, apparently.
          perturbation *= 10;
          structure.transform.Rotate(perturbation);
        }
      } else if (structure.structurePerturbations.HasFlag(StructurePerturbations.RotateOverY)) {
        float coin = UnityEngine.Random.value;
        // Let's flip 90 degrees
        if (coin < 0.4f) {
          structure.transform.Rotate(0, 90, 0);
        } else if (coin < 0.5) {
          structure.transform.Rotate(0, UnityEngine.Random.Range(-179f, 180f), 0);
        }
      }

      //var structure = structurePrefabs.PickRandom<LevelStructure>().GenerateClone();

      structure.transform.parent = transform;
      int bailCount = 0;
      do {
        var newVector = 30 * UnityEngine.Random.insideUnitSphere;
        newVector.x *= 2;
        newVector += center;
        structure.transform.position = newVector;
        Debug.Log("ITERATING");

        Physics.SyncTransforms();

        if (bailCount++ > 100) {
          break;
        }

        //} while (CollidesWithExistingObject(structure));
        float radius = CenteredBalloonRadius(structure, structure.transform.childCount);
        Debug.Log("Found radius: " + radius);

      } while (CollidesWithExistingObject(structure) && CenteredBalloonRadius(structure, structure.transform.childCount) < 15);

      Debug.Log("ADDED!" + structure.transform.position);
      generatedStructures.Add(structure);
    }
  }

  public List<Vector3> ClusterCenters = new List<Vector3>();

  [Button("Generate!")]
  public void Generate() {
    Clear();

    // First, create a platform underneath the player


    // Create the clusters
    foreach (Vector3 clusterCenter in ClusterCenters) {
      GenerateCluster(clusterCenter);
    }


    if (AnyOverlap()) {
      Debug.Log("At least found one issue!!!!");
    }

    // Great, now we've added all our structures.
    // Let's try to draw interesting connections between them.

    // Collect all connection points.
    // This populates candidateConnectionPoints, which is consumed
    // by GenerateConnections
    ComputeConnectionsCandidates();


    // We've built all the connection points. Now let's draw something.
    GenerateConnections();
  }

  static public bool SphereCastConnects(Transform connectionFrom, Transform connectionTo, float radius) {
    LevelStructure fromPlatform = connectionFrom.GetComponentInParent<LevelStructure>();
    LevelStructure toPlatform = connectionTo.GetComponentInParent<LevelStructure>();

    // Compute the direction:
    Vector3 fromPosition = connectionFrom.position;
    Vector3 toPosition = connectionTo.position;
    Vector3 direction = toPosition - fromPosition;

    float newMaxDistance = Math.Min(MaxConnectionDistance, Vector3.Distance(fromPosition, toPosition));

    // Performance: First, see if a raycast works:
    RaycastHit[] results = new RaycastHit[1];
    Ray raycastRay = new Ray(fromPosition, direction);
    if (Physics.RaycastNonAlloc(raycastRay, results, newMaxDistance) == 0) {
      return false;
    }
    // And we better have hit the toPlatform
    if (results[0].transform.GetComponentInParent<LevelStructure>() != toPlatform) {
      return false;
    }

    /////////////////////////////////////
    // This is expensive, but is necessary...?
    // Yes. It really does actually eliminate a lot of (bad) cases
    RaycastHit[] hitInfo = Physics.SphereCastAll(fromPosition, radius, direction, newMaxDistance);

    // Analyze the hitinfo to see if we've hit what we want.
    foreach (RaycastHit hit in hitInfo) {
      if (!ObjectIsDescendant(fromPlatform.transform, hit.transform) && !ObjectIsDescendant(toPlatform.transform, hit.transform)) {
        //Debug.Log("From: " + from + " to: " + to + " fails for " + hit);
        return false;
      }
    }
    // End expensive filtering
    /////////////////////////////////////

    return true;
  }

  HashSet<Transform> allConnectionPoints;
  List<Tuple<Transform, Transform>> candidateConnectionPoints; // ones we're interested in.

  void OnDrawGizmos() {

    Gizmos.color = Color.red;
    for (int i = 0; i < generatedStructures.Count; i++) {
      float r = CenteredBalloonRadius(generatedStructures[i], generatedStructures[i].transform.childCount);
      Gizmos.DrawWireSphere(generatedStructures[i].transform.position, r);
    }

    if (candidateConnectionPoints != null) {
      Gizmos.color = Color.blue;
      // Foreach pair (????? expensive! We can do smarter things, here.)
      foreach (Tuple<Transform, Transform> pair in candidateConnectionPoints) {
        Transform from = pair.Item1;
        Transform to = pair.Item2;
        Vector3 direction = to.position - from.position;
        Gizmos.DrawRay(from.position, direction);
      }
    }
  }
}