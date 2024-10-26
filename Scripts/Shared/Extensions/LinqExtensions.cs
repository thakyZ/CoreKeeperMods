using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NekoBoiNick.CoreKeeperMods.Shared.Extensions {
  /// <summary>
  /// Extenion methods to expand <see cref="System.Linq" />.
  /// </summary>
  internal static class LinqExtensions {
    /// <summary>Determines whether all elements of a sequence satisfy a condition.</summary>
    /// <param name="source">An <see cref="IEnumerable{T}" /> that contains the elements to apply the predicate to.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="source" /> or <paramref name="predicate" /> is <see langword="null" />.</exception>
    /// <returns>
    /// <see langword="true" /> if every element of the source sequence passes the test in the specified predicate, or if the sequence is empty; otherwise, <see langword="false" />.</returns>
    [DebuggerStepThrough]
    public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate) {
      int index = 0;

      // ReSharper disable once LoopCanBeConvertedToQuery
      foreach (TSource element in source) {
        if (predicate(element, index++)) {
          return true;
        }
      }

      return false;
    }

    /// <summary>Determines whether any element of a sequence satisfies a condition.</summary>
    /// <param name="source">An <see cref="IEnumerable{T}" /> whose elements to apply the predicate to.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="source" /> or <paramref name="predicate" /> is <see langword="null" />.</exception>
    /// <returns>
    /// <see langword="true" /> if the source sequence is not empty and at least one of its elements passes the test in the specified predicate; otherwise, <see langword="false" />.</returns>
    [DebuggerStepThrough]
    public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate) {
      int index = 0;

      // ReSharper disable once LoopCanBeConvertedToQuery
      foreach (TSource element in source) {
        if (predicate(element, index++)) {
          return true;
        }
      }

      return false;
    }
  }
}
