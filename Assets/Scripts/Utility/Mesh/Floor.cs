using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Utility {
  [RequireComponent(typeof(MeshFilter))]
  [RequireComponent(typeof(MeshRenderer))]
  [ExecuteInEditMode]
  public class Floor : MonoBehaviour {
    public Vector2 m_size = Vector2.one;
    public float m_vertexResolution = 1;
    public float m_UVTile = 1;

    public bool m_useCollider = true;

    [Header("Curves")]
    public bool UseCurves = false;
    public AnimationCurve m_curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0, 0));
    public float m_curveHeight = 1;
    public bool m_zCurve = false;

    private bool m_changed = true;
    private Mesh m_mesh = null;

    void OnValidate() {
      m_changed = true;
    }

    void Generate() {
      Vector2Int vertexSize = new Vector2Int((int)Mathf.Floor(m_vertexResolution * m_size.x), (int)Mathf.Floor(m_vertexResolution * m_size.y));
      if (vertexSize.x == 0 || vertexSize.y == 0) {
        Debug.LogWarning("Invalid mesh settings on Floor: " + this);
        return;
      }

      MeshUtil.CreatePlane(out m_mesh, m_size,
          vertexSize,
          new Vector2(Mathf.Max(m_UVTile, m_UVTile * m_size.x / m_size.y), Mathf.Max(m_UVTile, m_UVTile * m_size.y / m_size.x)));

      if (UseCurves) {
        MeshUtil.CurveMesh(ref m_mesh, m_curve, m_curveHeight, m_zCurve);
      }


      m_mesh.RecalculateNormals();
      m_mesh.RecalculateTangents();
      m_mesh.RecalculateBounds();

      GetComponent<MeshFilter>().mesh = m_mesh;

      if (m_useCollider) {
        MeshCollider collider = GetComponent<MeshCollider>();
        if (collider) {
          collider.sharedMesh = m_mesh;
        } else {
          gameObject.AddComponent(typeof(MeshCollider));
        }
      }

      m_changed = false;
    }

    void Update() {
      if (m_changed) {
        Generate();
      }
    }
  }
}