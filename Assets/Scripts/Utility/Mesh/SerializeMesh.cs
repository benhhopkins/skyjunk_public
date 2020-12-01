#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Utility {

  [ExecuteInEditMode]
  [RequireComponent(typeof(MeshFilter))]
  public class SerializeMesh : MonoBehaviour {
    [HideInInspector] [SerializeField] Vector2[] uv;
    [HideInInspector] [SerializeField] Vector3[] verticies;
    [HideInInspector] [SerializeField] Color[] vertexColors;
    [HideInInspector] [SerializeField] int[] triangles;
    [HideInInspector] [SerializeField] bool serialized = false;
    // Use this for initialization

    void Awake() {
      if (serialized) {
        GetComponent<MeshFilter>().sharedMesh = Rebuild();
        var meshCollider = GetComponent<MeshCollider>();
        if (meshCollider)
          meshCollider.sharedMesh = GetComponent<MeshFilter>().sharedMesh;
      }
    }

    void Start() {
      if (serialized) return;

      Serialize();
    }

    public void Serialize() {
      var mesh = GetComponent<MeshFilter>().sharedMesh;

      uv = mesh.uv;
      verticies = mesh.vertices;
      vertexColors = mesh.colors;
      triangles = mesh.triangles;

      serialized = true;
    }

    public Mesh Rebuild() {
      Mesh mesh = new Mesh();
      mesh.vertices = verticies;
      mesh.colors = vertexColors;
      mesh.triangles = triangles;
      mesh.uv = uv;

      mesh.RecalculateNormals();
      mesh.RecalculateBounds();

      return mesh;
    }
  }

#if UNITY_EDITOR
  [CustomEditor(typeof(SerializeMesh))]
  class SerializeMeshEditor : Editor {
    SerializeMesh obj;

    void OnSceneGUI() {
      obj = (SerializeMesh)target;
    }

    public override void OnInspectorGUI() {
      base.OnInspectorGUI();

      if (GUILayout.Button("Rebuild")) {
        if (obj) {
          obj.gameObject.GetComponent<MeshFilter>().sharedMesh = obj.Rebuild();
          var meshCollider = obj.gameObject.GetComponent<MeshCollider>();
          if (meshCollider)
            meshCollider.sharedMesh = obj.gameObject.GetComponent<MeshFilter>().sharedMesh;
        }
      }

      if (GUILayout.Button("Serialize")) {
        if (obj) {
          obj.Serialize();
        }
      }
    }
  }
#endif
}