using System;
using UnityEngine;

namespace Utility {

  static class MeshUtil {
    public static void CreatePlane(out Mesh mesh, Vector2 size, Vector2Int vertexSize, Vector2 uvTile) {
      mesh = new Mesh();

      Vector3[] vertices = new Vector3[(vertexSize.x + 1) * (vertexSize.y + 1)];
      Vector2[] uv = new Vector2[vertices.Length];
      Vector4[] tangents = new Vector4[vertices.Length];
      Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
      for (int i = 0, y = 0; y <= vertexSize.y; y++) {
        for (int x = 0; x <= vertexSize.x; x++, i++) {
          vertices[i] = new Vector3((float)x * (size.x / (float)vertexSize.x), (float)y * (size.y / (float)vertexSize.y), 0);
          uv[i] = new Vector2((float)x / (vertexSize.x / uvTile.x), (float)y / (vertexSize.y / uvTile.y));
          tangents[i] = tangent;
        }
      }
      mesh.vertices = vertices;
      mesh.uv = uv;
      mesh.tangents = tangents;

      int[] triangles = new int[vertexSize.x * vertexSize.y * 6];
      for (int ti = 0, vi = 0, y = 0; y < vertexSize.y; y++, vi++) {
        for (int x = 0; x < vertexSize.x; x++, ti += 6, vi++) {
          triangles[ti] = vi;
          triangles[ti + 3] = triangles[ti + 2] = vi + 1;
          triangles[ti + 4] = triangles[ti + 1] = vi + vertexSize.x + 1;
          triangles[ti + 5] = vi + vertexSize.x + 2;
        }
      }
      mesh.triangles = triangles;

      mesh.RecalculateNormals();
    }

    #  region CreatePlane method overlapping
    public static void CreatePlane(out Mesh mesh, Vector2 size, Vector2Int vertexSize) {
      CreatePlane(out mesh, size, vertexSize, Vector2.one);
    }
    public static void CreatePlane(out Mesh mesh, Vector2 size) {
      CreatePlane(out mesh, size, Vector2Int.one, Vector2.one);
    }
    #endregion

    public static void CurveMesh(ref Mesh mesh, AnimationCurve curve, float curveHeight, bool zCurve = false) {
      float min, max;
      if (!zCurve) {
        min = mesh.bounds.min.x;
        max = mesh.bounds.max.x;
      } else {
        min = mesh.bounds.min.z;
        max = mesh.bounds.max.z;
      }

      float width = max - min;
      if (width == 0)
        return;

      Vector3[] vertices = mesh.vertices;

      for (int i = 0; i < mesh.vertexCount; i++) {
        float position = zCurve ? mesh.vertices[i].z : mesh.vertices[i].x;

        vertices[i] = new Vector3(mesh.vertices[i].x,
            mesh.vertices[i].y + curveHeight * curve.Evaluate((position - min) / width),
            mesh.vertices[i].z);
      }

      mesh.vertices = vertices;
    }

    // Offsets all points in the mesh vertically based on proximity to corner heights
    //  cornerHeights array is clockwise -> TopLeft/TopRight/BottomRight/BottomLeft
    public static void SetPlaneCornerHeights(ref Mesh mesh, float[] cornerHeights) {
      Vector3[] verticies = mesh.vertices;

      Vector2[] corners = new Vector2[4];
      corners[0] = new Vector2(mesh.bounds.min.x, mesh.bounds.max.z); // topLeft
      corners[1] = new Vector2(mesh.bounds.max.x, mesh.bounds.max.z); // topRight
      corners[2] = new Vector2(mesh.bounds.max.x, mesh.bounds.min.z); // bottomRight
      corners[3] = new Vector2(mesh.bounds.min.x, mesh.bounds.min.z); // bottomLeft

      float maxInfluenceDistance = Mathf.Max(mesh.bounds.size.x, mesh.bounds.size.z);
      maxInfluenceDistance *= maxInfluenceDistance; // because we are using sqr magnitude

      for (int i = 0; i < mesh.vertexCount; i++) {
        Vector2 v = new Vector2(verticies[i].x, verticies[i].z);
        float totalInfluence = 0;
        for (int corner = 0; corner < 4; corner++)
          totalInfluence += Mathf.Max(0, maxInfluenceDistance - Vector2.SqrMagnitude(v - corners[corner]));

        for (int corner = 0; corner < 4; corner++) {
          float influence = Mathf.Max(0, maxInfluenceDistance - Vector2.SqrMagnitude(v - corners[corner])) / totalInfluence;
          verticies[i].y += influence * cornerHeights[corner];
          if (cornerHeights[corner] > 0)
            Debug.Log("Influence " + influence + " on " + cornerHeights[corner]);
        }
      }

      mesh.vertices = verticies;
    }

    // Offsets all points in a mesh depending on how close they are to the x-bounds
    public static void SetWallHeights(ref Mesh mesh, float height1, float height2) {
      Vector3[] verticies = mesh.vertices;
      float length = mesh.bounds.size.x;
      for (int i = 0; i < mesh.vertexCount; i++) {
        verticies[i].y += Mathf.Lerp(height1, height2, verticies[i].x / length);
      }
      mesh.vertices = verticies;
    }

    public static void RotateVerticies(ref Mesh mesh, Quaternion rotation, Vector3 center) {
      Vector3[] verticies = mesh.vertices;
      for (int i = 0; i < mesh.vertexCount; i++) {
        verticies[i] = rotation * (verticies[i] - center) + center;
      }
      mesh.vertices = verticies;
    }

    public static void FlipNormals(ref Mesh mesh) {
      int[] triangles = mesh.triangles;
      Array.Reverse(triangles);
      mesh.triangles = triangles;
    }

    // Stretches verticies based on proximity to grid points, optionally ignoring verticies above a certain height
    //  Grid points are 1 unit long, and points at y == -1 are stretched to the maximum amount as defined by the grid value
    public static void StretchMeshGrid(ref Mesh mesh, int[,] stretchHeights, float yTileSize, float ignoreAbove = 0) {
      Vector3[] verticies = mesh.vertices;

      for (int i = 0; i < mesh.vertexCount; i++) {
        float verticalInfluence = ignoreAbove - verticies[i].y;
        if (verticalInfluence < 0)
          continue;

        float totalInfluence = 0;
        Vector2 v = new Vector2(verticies[i].x, verticies[i].z);
        for (int row = 0; row < stretchHeights.GetLength(1); row++)
          for (int col = 0; col < stretchHeights.GetLength(0); col++) {
            Vector2 stretchPoint = new Vector2(col, stretchHeights.GetLength(1) - row - 1);
            totalInfluence += Mathf.Max(0, 1 - (v - stretchPoint).magnitude);
          }

        for (int row = 0; row < stretchHeights.GetLength(1); row++)
          for (int col = 0; col < stretchHeights.GetLength(0); col++) {
            Vector2 stretchPoint = new Vector2(col, stretchHeights.GetLength(1) - row - 1);
            float influence = Mathf.Max(0, 1 - (v - stretchPoint).magnitude) / totalInfluence;
            verticies[i].y += influence * verticalInfluence * (float)(stretchHeights[col, row] + 2) * yTileSize;
          }
      }

      mesh.vertices = verticies;
    }

    // Sets the vertex colors of the mesh by proximity to the corners.
    // Verticies should have normalized x,z positions from 0-1
    //  If keepGreen is true, the green component of verticies from the original mesh is not touched,
    //   and the red and blue components are scaled so that R+G+B=1
    public static void SetVertexColors(ref Mesh mesh, Color[,] cornerColors, bool keepGreen = false) {
      Vector3[] verticies = mesh.vertices;
      Vector3[] colorValues = new Vector3[verticies.Length];

      for (int i = 0; i < mesh.vertexCount; i++) {
        Vector2 v = new Vector2(verticies[i].x, verticies[i].z);
        float totalInfluence = 0;
        for (int row = 0; row < cornerColors.GetLength(1); row++)
          for (int col = 0; col < cornerColors.GetLength(0); col++)
            totalInfluence += Mathf.Max(0, 1 - (v - new Vector2(col, 1 - row)).magnitude);

        for (int row = 0; row < cornerColors.GetLength(1); row++)
          for (int col = 0; col < cornerColors.GetLength(0); col++) {
            float influence = Mathf.Max(0, 1 - (v - new Vector2(col, 1 - row)).magnitude) / totalInfluence;

            if (keepGreen) {
              if (colorValues[i].y > colorValues[i].x && colorValues[i].y > colorValues[i].z)
                influence = Mathf.Max(0, 1 - colorValues[i].y - .5f);
              colorValues[i] += influence * cornerColors[col, row].ToVector3();
            } else
              colorValues[i] += influence * cornerColors[col, row].ToVector3();
          }
      }

      Color[] colors = new Color[verticies.Length];
      for (int i = 0; i < mesh.vertexCount; i++) {
        colors[i] = colorValues[i].ToColor();
      }
      mesh.colors = colors;
    }

    private static Vector3 ToVector3(this Color color) => new Vector3(color.r, color.g, color.b);
    private static Color ToColor(this Vector3 v) => new Color(v.x, v.y, v.z);
  }
}
