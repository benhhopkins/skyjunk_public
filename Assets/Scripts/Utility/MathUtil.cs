using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility {
  public static class MathUtil {
    public static float Mode(IEnumerable<float> numbers) {
      var count = new Dictionary<float, int>();
      foreach (float n in numbers) {
        if (count.ContainsKey(n))
          count[n]++;
        else
          count[n] = 1;
      }
      float r = 0;
      float amount = 0;
      float modes = 0;
      foreach (var kvp in count.OrderByDescending(kvp => kvp.Value)) {
        if (kvp.Value >= amount) {
          amount = kvp.Value;
          r += kvp.Key;
          modes++;
        } else
          break;
      }
      if (modes == 0)
        return 0;
      return r / modes;
    }

    public static float Average(IEnumerable<float> numbers) {
      float r = 0;
      float divisor = 0;
      foreach (float n in numbers) {
        r += n;
        divisor++;
      }
      if (divisor == 0)
        return 0;
      return r / divisor;
    }

    public static float Range(IEnumerable<float> numbers) {
      var a = numbers.ToArray();
      if (a.Length < 2)
        return 0;

      float min = float.MaxValue;
      float max = float.MinValue;
      foreach (float n in a) {
        min = Mathf.Min(n, min);
        max = Mathf.Max(n, max);
      }
      return max - min;
    }

    public static IEnumerable<(int dx, int dy, T value, bool diagonal)> ArrayNeighbors<T>(T[,] a, int x, int y, bool diagonal) {
      for (int i = -1; i < 2; i++)
        for (int j = -1; j < 2; j++) {
          if (!(i == 0 && j == 0) &&
              x + i >= 0 && x + i < a.GetLength(0) &&
              y + j >= 0 && y + j < a.GetLength(1) &&
              (diagonal || Mathf.Abs(i) + Mathf.Abs(j) <= 1))
            yield return (i, j, a[x + i, y + j], Mathf.Abs(i) == Mathf.Abs(j));
        }
    }

    public static float RoundInDirection(float n, float roundToNearest, float direction) {
      n /= roundToNearest;
      if (direction > 0)
        n = Mathf.Ceil(n);
      else
        n = Mathf.Floor(n);
      return n * roundToNearest;
    }

    public static float DistanceToLineXZ(Vector3 p1, Vector3 p2, Vector3 target) {
      float num = Mathf.Abs((p2.z - p1.z) * target.x - (p2.x - p1.x) * target.z + p2.x * p1.z - p2.z * p1.x);
      float denom = Mathf.Sqrt(Mathf.Pow(p2.z - p1.z, 2) + Mathf.Pow(p2.x - p1.x, 2));
      return num / denom;
    }
  }
}