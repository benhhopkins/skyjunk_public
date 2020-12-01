using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility {
  public static class CollectionUtil {

    #region  PickRandom
    public static T PickRandom<T>(this IEnumerable<T> a) where T : class {
      var array = a.ToArray();
      return PickRandom<T>(array);
    }

    public static T PickRandomValue<T>(this IEnumerable<T> a) where T : struct {
      var array = a.ToArray();
      return PickRandomValue<T>(array);
    }

    public static T PickRandom<T>(params T[] items) where T : class {
      if (items.Length == 0)
        return null;
      return items[Random.Range(0, items.Length)];
    }

    public static T PickRandomValue<T>(params T[] items) where T : struct {
      if (items.Length == 0)
        return new T();
      return items[Random.Range(0, items.Length)];
    }
    #endregion

    #region PickRandomOdds
    public static T PickRandomOddsValue<T>(params (T item, int odds)[] items) where T : struct {
      int totalOdds = 0;
      foreach (var item in items)
        totalOdds += item.odds;
      int pick = Random.Range(0, totalOdds);
      int accumulatedOdds = 0;
      foreach (var item in items) {
        accumulatedOdds += item.odds;
        if (accumulatedOdds >= pick)
          return item.item;
      }
      return new T();
    }

    public static T PickRandomOdds<T>(params (T item, int odds)[] items) where T : class {
      int totalOdds = 0;
      foreach (var item in items)
        totalOdds += item.odds;
      int pick = Random.Range(0, totalOdds);
      int accumulatedOdds = 0;
      foreach (var item in items) {
        accumulatedOdds += item.odds;
        if (accumulatedOdds >= pick)
          return item.item;
      }
      return null;
    }
    #endregion

    #region ChooseRandom
    public static IEnumerable<T> ChooseRandom<T>(int n, IEnumerable<T> items) {
      return ChooseRandom<T>(n, items.ToArray());
    }

    public static IEnumerable<T> ChooseRandom<T>(int n, params T[] items) {
      for (int i = 0; i < items.Length; i++) {
        if (Random.value <= (float)n / (float)(items.Length - i)) {
          yield return items[i];
          n--;
          if (n == 0)
            break;
        }
      }
    }
    #endregion

    #region Shuffle
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items) {
      var itemsArray = items.ToArray();
      T temp;
      for (int i = 0; i < itemsArray.Length; i++) {
        var target = Random.Range(0, itemsArray.Length);
        temp = itemsArray[target];
        itemsArray[target] = itemsArray[i];
        itemsArray[i] = temp;
      }
      return itemsArray;
    }
    #endregion
  }

  public class Multimap<TKey, TValue> :
    IEnumerable<(TKey key, TValue value)> {

    [SerializeField]
    private Dictionary<TKey, List<TValue>> dictionary;

    [SerializeField]
    private int count;
    public int Count => count;

    public ICollection<TKey> Keys => dictionary.Keys;

    public ICollection<TValue> Values {
      get {
        var values = new List<TValue>();
        foreach (var list in dictionary.Values)
          foreach (var v in list)
            values.Add(v);
        return values;
      }
    }

    public Multimap() {
      dictionary = new Dictionary<TKey, List<TValue>>();
    }

    public void Add(TKey key, TValue value) {
      if (dictionary.ContainsKey(key))
        dictionary[key].Add(value);
      else {
        var list = new List<TValue>();
        list.Add(value);
        dictionary[key] = list;
      }
      count++;
    }

    public List<TValue> ListAt(TKey key) {
      if (dictionary.ContainsKey(key))
        return dictionary[key];
      return null;
    }

    public IEnumerable<TValue> At(TKey key) {
      if (dictionary.ContainsKey(key))
        return dictionary[key];
      return Enumerable.Empty<TValue>();
    }

    public IEnumerator<(TKey key, TValue value)> GetEnumerator() {
      foreach (var kvp in dictionary)
        foreach (var value in kvp.Value)
          yield return (kvp.Key, value);
    }

    IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    public void Clear() {
      if (dictionary == null)
        dictionary = new Dictionary<TKey, List<TValue>>();
      dictionary.Clear();
      count = 0;
    }
  }

  public static class DictionaryExtension {
    public static TV Get<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV defaultValue = default(TV)) {
      TV value;
      if (key == null)
        return defaultValue;
      return dict.TryGetValue(key, out value) ? value : defaultValue;
    }
  }
}