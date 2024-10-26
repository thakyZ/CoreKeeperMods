using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using NuGet.Packaging;

// ReSharper disable once CheckNamespace
namespace NekoBoiNick.CoreKeeperMods.Shared.Extensions {
  /// <summary>
  /// Extensions for <see cref="string" /> and collections of <see cref="string"/>s.
  /// </summary>
  public static class StringExtensions {
    /// <summary>
    /// Checks if the given string starts with any of the <paramref name="matches"/> values. And compares it with <see cref="StringComparison" />.
    /// </summary>
    /// <param name="string">The given string.</param>
    /// <param name="matches">The set of matches to test against.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
    /// <returns><see langword="true" /> if the string starts with any of the given matches; otherwise, <see langword="false" />.</returns>
    [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWithAny(this string @string, string[] matches, StringComparison comparisonType) {
      return matches.Any((string match) => @string.StartsWith(match, comparisonType));
    }

    /// <summary>
    /// Checks if the given string starts with any of the <paramref name="matches"/> values.
    /// </summary>
    /// <param name="string">The given string.</param>
    /// <param name="matches">The set of matches to test against.</param>
    /// <returns><see langword="true" /> if the string starts with any of the given matches; otherwise, <see langword="false" />.</returns>
    [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWithAny(this string @string, string[] matches) {
      return matches.Any(@string.StartsWith);
    }

    /// <summary>
    /// Gets the remaining text from what would normally be a substring of the text.
    /// </summary>
    /// <param name="string">The given string.</param>
    /// <param name="startIndex">The zero-based starting character position of a substring in this instance.</param>
    /// <param name="length">The number of characters in the substring.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///     <paramref name="startIndex" /> plus <paramref name="length" /> indicates a position not within this instance.
    /// -or-
    ///     <paramref name="startIndex" /> or <paramref name="length" /> is less than zero.</exception>
    /// <returns>
    /// A string that is equivalent to the substring of length <paramref name="length" /> that begins at <paramref name="startIndex" /> in this instance,
    /// or <see cref="System.String.Empty" /> if <paramref name="startIndex" /> is equal to the length of this instance and <paramref name="length" /> is zero.
    /// </returns>
    [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReverseSubstring(this string @string, int startIndex, int length, int count = -1) {
      ArgumentOutOfRangeException.ThrowIfGreaterThan(startIndex + length, @string.Length);
      ArgumentOutOfRangeException.ThrowIfLessThan(startIndex, 0);
      ArgumentOutOfRangeException.ThrowIfLessThan(length, 0);

      return new System.Text.RegularExpressions.Regex(System.Text.RegularExpressions.Regex.Escape(@string.Substring(startIndex, length))).Replace(@string, "", count == -1 ? int.MaxValue : count);
    }

    /// <summary>
    /// Splits a string into a maximum number of substrings based on a specified delimiting character and, optionally, options.
    /// Splits a string into a maximum number of substrings based on the provided character separator, optionally omitting empty substrings from the result.
    /// </summary>
    /// <param name="string"></param>
    /// <param name="match"></param>
    /// <param name="count">The maximum number of elements expected in the array.</param>
    /// <param name="options">A bitwise combination of the enumeration values that specifies whether to trim substrings and include empty substrings.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search.</param>
    /// <returns>An array that contains at most <paramref name="count" /> substrings from this instance that are delimited by <paramref name="separator" />.</returns>
    [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string[] SplitAfter(this string @string, string match, StringComparison comparisonType, int count = -1, StringSplitOptions options = StringSplitOptions.None) {
      List<string> output = [];
      if ((count != -1 && output.Count != count + 1) || count <= -1) {
        var indexOfMatch = @string.IndexOf(match, 0, @string.Length - 1, comparisonType) + match.Length;
        output.Add(@string[..indexOfMatch]);
        output.AddRange(@string.ReverseSubstring(0, indexOfMatch, 1).SplitAfter(match, comparisonType, count - 1, options));
      }
      return [..output];
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="string"></param>
    /// <param name="match"></param>
    /// <returns></returns>
    [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string[] SplitAfter(this string @string, string match) {
      return @string.SplitAfter(match, StringComparison.Ordinal);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="list"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    [DebuggerStepThrough]
    public static List<string> ConcatList(this List<string> list, List<string> other) {
      return [..(from item1 in list from item2 in other select item1 + item2).Distinct()];
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="string"></param>
    /// <returns></returns>
    [DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmptyOrWhiteSpace([NotNullWhen(false)] this string? @string) {
      return string.IsNullOrEmpty(@string) || string.IsNullOrWhiteSpace(@string);
    }

    public static bool ContainsAny(this string source, string[] matches) {
      return matches.Any(source.Contains);
    }

    public static bool ContainsAny(this string source, string[] matches, StringComparison comparisonType) {
      return matches.Any(match => source.Contains(match, comparisonType));
    }
  }
}
