using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility {

  public interface ISpatialObject {
    Vector3 SpatialPosition { get; }
    float SpatialSize { get; }
  }

  // 3d map not used, therefore not maintained with 2d map improvements
  // public class SpatialMap<T> where T : ISpatialObject {

  //   private static readonly int[] cellSizes = { 2, 8, 32 };
  //   private Multimap<int, T>[] cells;
  //   private List<T> values;
  //   public IEnumerable<T> Values => values;

  //   public SpatialMap() {
  //     for (int i = 0; i < cellSizes.Length; i++)
  //       cells[i] = new Multimap<int, T>();
  //   }

  //   public void Add(T item) {
  //     values.Add(item);
  //     Insert(item);
  //   }

  //   public void Remove(T item) {
  //     values.Remove(item);
  //   }

  //   public void Clear() {
  //     values.Clear();
  //     for (int i = 0; i < cellSizes.Length; i++)
  //       cells[i].Clear();
  //   }

  //   public void Recalculate() {
  //     for (int i = 0; i < cellSizes.Length; i++)
  //       cells[i].Clear();

  //     foreach (var o in values)
  //       Insert(o);
  //   }

  //   // note: radius must be non-negative
  //   public IEnumerable<T> FindAll(Vector3 position, Vector3 radius) {
  //     int cellSizeIndex = 0;
  //     var maxRadius = Mathf.Max(radius.x, radius.y, radius.z);
  //     for (int i = 0; i < cellSizes.Length; i++)
  //       if (maxRadius > cellSizes[i] && cellSizes[i] > cellSizes[cellSizeIndex]) {
  //         cellSizeIndex = i;
  //       }

  //     var minPosition = Vector3Int.FloorToInt((position - radius) / cellSizes[cellSizeIndex]);
  //     var maxPosition = Vector3Int.CeilToInt((position + radius) / cellSizes[cellSizeIndex]);

  //     var found = new List<T>();
  //     for (int x = minPosition.x; x <= maxPosition.x; x++)
  //       for (int y = minPosition.y; y <= maxPosition.y; y++)
  //         for (int z = minPosition.z; z <= maxPosition.z; z++) {
  //           found.AddRange(cells[cellSizeIndex].At(GetHashedPosition(new Vector3(x, y, z), cellSizes[cellSizeIndex])));
  //         }

  //     return found;
  //   }

  //   private void Insert(T item) {
  //     for (int i = 0; i < cellSizes.Length; i++) {
  //       cells[i].Add(GetHashedPosition(item.SpatialPosition, cellSizes[i]), item);
  //     }
  //   }

  //   private static int GetHashedPosition(Vector3 position, float cellRadius) {
  //     var v = Vector3Int.FloorToInt(position / cellRadius);
  //     return (v.z * 131 + v.y) * 31 + v.x;
  //   }

  // }

  public class SpatialMap2D<T> where T : ISpatialObject {

    private static readonly int[] cellSizes = { 2, 8, 32 };
    private Multimap<int, T>[] cells;
    private List<T> values;
    public IEnumerable<T> Values => values;

    // persistant lists to avoid memory alloc/dealloc
    // be sure to clear() before each use
    private HashSet<T> m_hashSet = new HashSet<T>();

    public SpatialMap2D() {
      cells = new Multimap<int, T>[cellSizes.Length];
      for (int i = 0; i < cellSizes.Length; i++)
        cells[i] = new Multimap<int, T>();

      values = new List<T>();
    }

    public void Add(T item) {
      values.Add(item);
      Insert(item);
    }

    public void Remove(T item) {
      values.Remove(item);
    }

    public void Clear() {
      values.Clear();
      for (int i = 0; i < cellSizes.Length; i++)
        cells[i].Clear();
    }

    public void Recalculate() {
      for (int i = 0; i < cellSizes.Length; i++)
        cells[i].Clear();

      foreach (var o in values)
        if (o != null)
          Insert(o);
    }

    // note: rectRadius must be positive
    public IEnumerable<T> FindAll(Vector3 center, float radius) {
      float centerX = center.x;
      float centerZ = center.z;
      if (radius < 0)
        return Enumerable.Empty<T>();

      int cellSizeIndex = 0;
      for (int i = 0; i < cellSizes.Length; i++)
        if (radius > cellSizes[i] && cellSizes[i] > cellSizes[cellSizeIndex]) {
          cellSizeIndex = i;
        }

      int minX = Mathf.FloorToInt((centerX - radius) / cellSizes[cellSizeIndex]);
      int minZ = Mathf.FloorToInt((centerZ - radius) / cellSizes[cellSizeIndex]);
      int maxX = Mathf.CeilToInt((centerX + radius) / cellSizes[cellSizeIndex]);
      int maxZ = Mathf.CeilToInt((centerZ + radius) / cellSizes[cellSizeIndex]);

      m_hashSet.Clear();
      for (int x = minX; x <= maxX; x++)
        for (int z = minZ; z <= maxZ; z++) {
          var found = cells[cellSizeIndex].ListAt(GetHashedPosition(x, z));
          if (found != null)
            for (int i = 0; i < found.Count; i++)
              m_hashSet.Add(found[i]);
        }

      return m_hashSet;
    }

    private void Insert(T item) {
      for (int i = 0; i < cellSizes.Length; i++) {
        int x = Mathf.FloorToInt(item.SpatialPosition.x / cellSizes[i]);
        int z = Mathf.FloorToInt(item.SpatialPosition.z / cellSizes[i]);
        cells[i].Add(GetHashedPosition(x, z), item);
      }
    }

    private static int GetHashedPosition(int x, int z) {
      return z * 617 + x;
    }
  }

}