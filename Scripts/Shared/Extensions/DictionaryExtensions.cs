using System.Collections.Generic;

namespace NekoBoiNick.CoreKeeperMods.Shared.Extensions {
  public static class DictionaryExtensions {
    public static bool DictionaryEqual<TKey, TValue>(this IDictionary<TKey, TValue>? first, IDictionary<TKey, TValue>? second) where TKey : notnull
    {
      return first.DictionaryEqual(second, null);
    }

    public static bool DictionaryEqual<TKey, TValue>(this IDictionary<TKey, TValue>? first, IDictionary<TKey, TValue>? second, IEqualityComparer<TValue>? valueComparer) where TKey : notnull
    {
      if (Equals(first, second)) {
        return true;
      }

      if (first == null || second == null) {
        return false;
      }

      if (first.Count != second.Count) {
        return false;
      }

      valueComparer ??= EqualityComparer<TValue>.Default;

      foreach (var kvp in first)
      {
        if (!second.TryGetValue(kvp.Key, out TValue? secondValue)) {
          return false;
        }

        if (!valueComparer.Equals(kvp.Value, secondValue)) {
          return false;
        }
      }
      return true;
    }
  }
}
