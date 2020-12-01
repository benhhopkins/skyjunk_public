using System.Collections.Generic;
using UnityEngine;

namespace Utility {

  // a helper class to reuse memory when modifying meshes,
  //  instead of creating a ton of garbage
  public class MeshSetter {

    private Mesh mesh;

    public int VertexCount { get; private set; }
    public int TrisCount { get; private set; }
    public readonly List<Vector3> Verticies;
    public readonly List<Vector2> UVs;
    public readonly List<int> Triangles;

    public MeshSetter(int vertexCount, int trisCount) {
      mesh = new Mesh();

      VertexCount = vertexCount;
      Verticies = new List<Vector3>(vertexCount);
      UVs = new List<Vector2>(vertexCount);
      Triangles = new List<int>(trisCount);
    }

    public void SetCapacities(int vertexCount, int trisCount) {
      VertexCount = vertexCount;
      TrisCount = trisCount;

      if (vertexCount > Verticies.Count) {
        Verticies.AddRange(new Vector3[vertexCount - Verticies.Count]);
        UVs.AddRange(new Vector2[vertexCount - UVs.Count]);
      }

      if (trisCount > Triangles.Count) {
        Triangles.AddRange(new int[trisCount - Triangles.Count]);
      }
    }

    public Mesh CreateMesh(bool calculateBounds = true, bool recalculateNormals = true) {
      mesh.Clear();
      mesh.SetVertices(Verticies, 0, VertexCount);
      mesh.SetUVs(0, UVs, 0, VertexCount);
      mesh.SetTriangles(Triangles, 0, TrisCount, 0, calculateBounds);

      if (recalculateNormals)
        mesh.RecalculateNormals();

      return mesh;
    }

  }

}