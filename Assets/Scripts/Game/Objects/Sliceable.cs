using UnityEngine;
using EzySlice;

public class Sliceable : MonoBehaviour {

  public SkinnedMeshRenderer skinnedMeshRenderer = null;
  public Material hullMaterial = null;

  public void SliceAndDestroy(HitTrigger hitTrigger = null) {
    GameObject objectToSlice = new GameObject();
    objectToSlice.transform.position = transform.position;
    objectToSlice.transform.rotation = transform.rotation;

    var meshFilter = objectToSlice.AddComponent<MeshFilter>();
    var meshRenderer = objectToSlice.AddComponent<MeshRenderer>();

    if (skinnedMeshRenderer) {
      Mesh mesh = new Mesh();
      skinnedMeshRenderer.BakeMesh(mesh);
      meshFilter.sharedMesh = mesh;
      meshRenderer.sharedMaterial = skinnedMeshRenderer.sharedMaterial;
    } else if (GetComponent<MeshRenderer>()) {
      meshRenderer.sharedMaterial = GetComponent<MeshRenderer>().sharedMaterial;
      meshFilter.sharedMesh = GetComponent<MeshFilter>().sharedMesh;
      objectToSlice.transform.localScale = transform.localScale;
    } else {
      Debug.Log("Error>> No MeshRenderer or SkinnedMeshRenderer on DestroyableObject" + gameObject);
      Destroy();
      return;
    }

    var meshCollider = objectToSlice.AddComponent<MeshCollider>();
    meshCollider.convex = true;

    SliceInternal(objectToSlice, 2, new Vector3(Random.value, Random.value, Random.value), hullMaterial, hitTrigger);

    Destroy();
  }

  private void Destroy() {
    Destroy(gameObject);
  }

  private void SliceInternal(GameObject o, int recurse, Vector3 direction, Material crossSectionMat, HitTrigger hitTrigger = null) {
    var center = o.transform.position;
    var collider = o.GetComponent<Collider>();
    if (collider)
      center = collider.bounds.center;

    var slicedObjects = o.SliceInstantiate(
              center,
              direction,
              crossSectionMat);
    if (slicedObjects != null) {
      Destroy(o);

      foreach (var sliced in slicedObjects) {
        sliced.transform.position = transform.position;
        sliced.transform.rotation = transform.rotation * Quaternion.Euler(0, -90, 0);
        // because the mesh was baked from the scaled skinnedMeshRenderer, we don't need to scale it
        sliced.transform.localScale = Vector3.one;

        if (recurse > 0) {
          SliceInternal(sliced, recurse - 1, new Vector3(Random.value, Random.value, Random.value), crossSectionMat, hitTrigger);
          Destroy(sliced.gameObject);
        } else {
          sliced.layer = LayerManager.I.EffectMask.ToLayer();
          var hitRigidbody = sliced.AddComponent<Rigidbody>();
          var meshCollider = sliced.AddComponent<MeshCollider>();
          meshCollider.convex = true;

          sliced.AddComponent<Debris>();

          if (hitTrigger != null) {
            foreach (var effect in hitTrigger.hitRigidbodyEffects) {
              effect.Hit(hitTrigger, hitRigidbody);
            }
          }
        }
      }
    }
  }
}